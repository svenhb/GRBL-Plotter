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
 * 2026-05-19 add inhibit flag processAllElements to avoid unwanted click event
 * 2026-06-11 l:516 f:BtnReloadGraphic_Click	set cursor to WaitCursor
*/

using GrblPlotter.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static GrblPlotter.Graphic;
using static GrblPlotter.XmlMarker;

namespace GrblPlotter.UserControls
{
    public partial class UCToolList : UserControl
    {
        private float DpiScaling;
        private int _tabDevice = 0;
        private int _tabPlotterMode = 0;
        private bool reloadNeded = false;
        private static readonly System.Timers.Timer timerBlink = new System.Timers.Timer();
        private static bool highlightReload = false;
        private static bool blinkReload = false;
        private int groupBy = 0;
        private int invalidateIsNeeded = 0;
        private int lastClickedElement = -1;
        Size toolElementSize = new Size(366, 25);
        private readonly Color btnDefColor = Color.WhiteSmoke;
        private bool stateShowToolTable = false;
        private bool processAllElements = false;

        private bool log = true;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        public event EventHandler<UserControlGuiControlEventArgs> RaiseGuiControlEvent;
        protected virtual void OnRaiseGuiControlEvent(UserControlGuiControlEventArgs e)
        { RaiseGuiControlEvent?.Invoke(this, e); }

        public UCToolList()
        {
            InitializeComponent();
            DpiScaling = (float)DeviceDpi / 96;

            timerBlink.Elapsed += TimerBlink_Tick;
            timerBlink.Interval = 330;

            this.FlpToolList.DragEnter += new DragEventHandler(FlowLayoutPanel_DragEnter);
            this.FlpToolList.DragDrop += new DragEventHandler(FlowLayoutPanel_DragDrop);
            MyControl.InitToolProperties();
            MyControl.SettingWasChanged(false);
            Logger.Trace("Init");
        }

        private void UCToolList_Load(object sender, EventArgs e)
        {
            Logger.Trace("Load");
            this.Visible = false;
            HeadlineLaser.Top = 1;
            HeadlinePlotter.Top = 1;
            HeadlineRouter.Top = 1;
            HeadlineCoordinate.Top = 1;
            FlpToolList.Top = 20;
            FlpToolList.Height = Height - 50;
            PanelNoXML.Top = 30;
            PanelNoXML.Height = FlpToolList.Height - 20;
            PanelNoXML.Left = FlpToolList.Left + 10;

            MyControl.SetSetupBtnAppearance(BtnSetup);
            MyControl.ApplyToolList = CbApplyToolList.Checked;
            ToolList.Init("(UCToolList Load)");
            BtnApplyToolListHighlight();
            timerBlink.Start();
            this.Visible = true;
        }

        public void SwitchView(int tabDevice, int tabPlotterMode)
        {
            /* go through all tool-list-element tags and switch view */
            _tabDevice = tabDevice;
            _tabPlotterMode = tabPlotterMode;
            if (log) Logger.Trace("SwitchView  {0}  {1}", tabDevice, tabPlotterMode);
            if (tabDevice <= 3)   // Laser
            {
                HeadlineLaser.Visible = (tabDevice == 0);
                HeadlinePlotter.Visible = (tabDevice == 1);
                HeadlineRouter.Visible = (tabDevice == 2);
                HeadlineCoordinate.Visible = (tabDevice == 3);

                LblPlotterS.Visible = (tabPlotterMode == 0);
                LblPlotterZ.Visible = (tabPlotterMode == 1);
                LblPlotterLaser.Visible = (tabPlotterMode == 1);

                if (FlpToolList.Controls.Count > 0)
                {
                    foreach (Control control in FlpToolList.Controls)
                    {
                        ((UCToolListElement)control).SwitchView(tabDevice, tabPlotterMode);
                    }
                }
                reloadNeded = true;
            }
        }

        private UCToolListElement CreateElement(Size size, ToolProperty toolProperties, int id)
        {
            /* create single tool-list-element tag */
            UCToolListElement tle = new UCToolListElement(toolProperties, groupBy)
            {
                Id = id,
                Size = size,
                Padding = new Padding(0),
                Margin = new Padding(1)
            };
            tle.Click += Tle_Click;
            tle.RaiseXYZEvent += MoveToPosition;
            tle.SetEnable(CbApplyToolList.Checked);
            tle.SwitchView(_tabDevice, _tabPlotterMode, true);
            return tle;
        }

        private void MoveToPosition(object sender, XyzEventArgs e)
        {
            // Tool nr is given via X
            int toolNR = (int)e.PosX;
            ToolPosition tp = ToolChanger.GetToolPosition(toolNR);
            var psd = Properties.Settings.Default;
            double x = tp.Position.X + (double)psd.toolTableOffsetX;
            double y = tp.Position.Y + (double)psd.toolTableOffsetY;
            double z = tp.Position.Z + (double)psd.toolTableOffsetZ;

            Logger.Trace("MoveToPosition ToolNr:{0}  X:{1}  Y:{2}  Z:{3}", toolNR, x, y, z);
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G53 G91 G0 Z{0:0.000}", z).Replace(",", "."), 0, sender, e));
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G53 G91 G0 X{0:0.000} Y{1:0.000}", x, y).Replace(",", "."), 0, sender, e));
        }

        /*********************************************************
         * Click on tool, highlight group
         *********************************************************/
        private void Tle_Click(object sender, EventArgs e)
        {
            if (!processAllElements)
            {
                int toolNr = ((UCToolListElement)sender).toolProp.ToolNr;
                int cntrlNr = ((UCToolListElement)sender).Id;
                lastClickedElement = cntrlNr;

                bool settingChanged = ((UCToolListElement)sender).SettingChanged;
                if (log) Logger.Trace("Tle clicked {0} toolNr {1} changed:{2}", cntrlNr, toolNr, settingChanged);
                if (!settingChanged)
                    OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.highligh, toolNr));
                ((UCToolListElement)sender).SettingChanged = false;
            }
        }

        // https://www.codeproject.com/articles/Using-the-FlowLayoutPanel-and-Reordering-with-Drag#comments-section
        void FlowLayoutPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /*********************************************************
        * Drag drop tool
        *********************************************************/
        void FlowLayoutPanel_DragDrop(object sender, DragEventArgs e)
        {
            UCToolListElement listElelement = (UCToolListElement)e.Data.GetData(typeof(UCToolListElement));
            FlowLayoutPanel _destination = (FlowLayoutPanel)sender;

            Point p = _destination.PointToClient(new Point(e.X, e.Y));
            var item = _destination.GetChildAtPoint(p);
            int index = _destination.Controls.GetChildIndex(item, false);
            _destination.Controls.SetChildIndex(listElelement, index);
            _destination.Invalidate();
            if (log) Logger.Trace("DragDrop index:{0}  item:{1}", index, item);

            if (CbApplyToolList.Checked)
                ReloadNeded();
        }

        private void TableLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            TableLayoutPanel _destination = (TableLayoutPanel)sender;
            Point p = _destination.PointToClient(new Point(e.X, e.Y));
            var item = _destination.GetChildAtPoint(p);
            int index = _destination.Controls.GetChildIndex(item, false);
            if (log) Logger.Trace("DragDrop mouse index:{0}  item:{1}", index, item);
        }

        internal void ReloadNeded(bool setNotify = true)
        {
            if (log) Logger.Trace("ReloadNeded setNotify:{0}  MyControl.GraphicImported:{1}", setNotify, MyControl.GraphicImported);
            blinkReload = setNotify;
            if (setNotify && MyControl.GraphicImported)
            {
                SetFlpToolListColor(Color.Fuchsia);
            }
            else
            {
                BtnReloadGraphic.BackColor = btnDefColor;
                BtnReloadGraphic.ForeColor = Colors.ContrastColor(btnDefColor);
                if (MyControl.GraphicImported)
                    SetFlpToolListColor(Color.LightGreen);
                else
                    SetFlpToolListColor(Color.White);
            }
            LblDevice.Text = MyControl.GetSelectedDeviceName();
        }
        private void SetFlpToolListColor(Color col)
        {
            if (log) Logger.Trace("SetFlpToolListColor color:{0}", col);
            FlpToolList.BackColor = col;
            foreach (Control control in FlpToolList.Controls)
            {
                ((UCToolListElement)control).BackColor = col;
            }
            Invalidate();
        }
        internal void UpdateToolListElements()
        {
            Logger.Trace("###### All devices updated: {0}", MyControl.AllDevicesInitialized);
            FillToolListElements();
            //    BtnApplyToolListHighlight();
        }
        private void TimerBlink_Tick(object sender, EventArgs e)
        {
            if (MyControl.GetSettingWasChanged())
            {
                ReloadNeded();
                MyControl.SettingWasChanged(false);
            }
            if (invalidateIsNeeded > 0)
            {
                VisuGCode.DrawMachineLimit();
                OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.invalidate));
                invalidateIsNeeded--;
            }
            if (blinkReload)
            {
                if (highlightReload)
                {
                    BtnReloadGraphic.BackColor = btnDefColor;
                    BtnReloadGraphic.ForeColor = Colors.ContrastColor(btnDefColor);
                }
                else
                {
                    BtnReloadGraphic.BackColor = MyControl.NotifyYellow;
                    BtnReloadGraphic.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                }
                highlightReload = !highlightReload;
            }
        }

        /******************************************************
         * clear tool-list-elements, create new tool-list-element-tags from tool-list 
         ******************************************************/
        internal void FillToolListElements()
        {
            Logger.Trace("####### FillToolListElements  Count: {0}  Device: {1}", ToolList.toolListArray.Count, MyControl.SelectedDevice);
            processAllElements = false;
            if (ToolList.toolListArray.Count > 0)
            {
                FlpToolList.Visible = false;

                /* clear flow-layout-panel */
                if (FlpToolList.Controls.Count > 0)
                {
                    foreach (Control control in FlpToolList.Controls)
                    {
                        FlpToolList.Controls.Remove(control);
                        control.Dispose();
                    }
                    FlpToolList.Controls.Clear();
                }
                /* add actual tool-list-element-tags */
                float w = (float)MyControl.GetActualToolDiameter();
                bool replace = groupBy > 0;
                /* fill flow-layout-panel with tool list */
                int id = 0;
                Logger.Trace("###### FillToolListElements  after clear Count: {0}  Device: {1}", ToolList.toolListArray.Count, MyControl.SelectedDevice);
                foreach (ToolProperty p in ToolList.toolListArray)
                {
                    p.GroupWidth = w;

                    if (log) Logger.Trace("##### FillToolListElement {0}  {1}  {2}  {3}", p.ToolNr, p.ToolName, p.GroupWidth, p.GroupLayer);
                    FlpToolList.Controls.Add(CreateElement(toolElementSize, p, id++));
                }
                FlpToolListSetWidth(FlpToolList.VerticalScroll.Visible ? (toolElementSize.Width + 24) : (toolElementSize.Width + 4));

                if (!MyControl.ApplyToolList && (MyControl.SelectedDevice == DeviceSelection.Plotter))
                {
                    Logger.Trace("#### FillToolListElement set Laser.FinalS = 0");
                    processAllElements = true;
                    foreach (Control control in FlpToolList.Controls)
                    {
                        ((UCToolListElement)control).SetLaserIfPlotter(0);
                    }
                    processAllElements = false;
                }

                FlpToolList.Visible = true;
            }
        }
        internal void ClearToolListElements()
        {
            FlpToolList.Visible = false;

            /* clear flow-layout-panel */
            foreach (Control control in FlpToolList.Controls)
            {
                FlpToolList.Controls.Remove(control);
                control.Dispose();
            }
            FlpToolList.Controls.Clear();
            FlpToolList.Visible = true;
        }

        /******************************************************
         * after loading GCode, create tool-list from XML-Tags (if actual tool-list is not applied)
         ******************************************************/
        private void CreateToolListFromGraphic()
        {
            Logger.Info("####### CreateToolListFromGraphic XmlMarker count:{0} ", XmlMarker.GetGroupCount());

            /* delete old tool list */
            FlpToolList.Visible = false;
            foreach (Control control in FlpToolList.Controls)
            {
                FlpToolList.Controls.Remove(control);
                control.Dispose();
            }
            FlpToolList.Controls.Clear();
            ToolList.Reset();
            ToolTable.Clear();

            /* Get Group properties from already loaded graphic */
            if (XmlMarker.GetGroupCount() > 0)
            {
                PanelNoXML.Visible = false;
                BlockData bd = new BlockData();
                ToolProperty tp = MyControl.GetToolsProperties().Copy();
                bool replace = groupBy == 1;

                int maxAmount = Math.Min(XmlMarker.GetGroupCount(), 64);
                if (maxAmount != XmlMarker.GetGroupCount())
                    Logger.Warn("ToolListElemnents amount limited!!! Amount:{0}  Limit:{1}", XmlMarker.GetGroupCount(), 64);

                double y = 0;
                for (int i = 0; i < maxAmount; i++)
                {
                    if (XmlMarker.GetGroupByIndex(ref bd, i))   // get properties of group
                    {
                        if (log) Logger.Trace("from XML-BlockData id:{0}  color:{1}   width:{2}   layer:{3}  name:{4}", bd.Id, bd.PenColor, bd.PenWidth, bd.Layer, bd.ToolName);
                        tp.GroupColor = Colors.TryConvertColor(bd.PenColor);
                        tp.ToolNr = (short)bd.Id;
                        tp.GroupWidth = (float)bd.PenWidth;
                        tp.GroupLayer = bd.Layer;
                        tp.ToolName = bd.ToolName;

                        //   if (replace)
                        {
                            tp.Laser.Diameter = tp.Plotter.Diameter = tp.Router.Diameter = tp.GroupWidth;
                        }
                        ToolList.Add(tp);
                        FlpToolList.Controls.Add(CreateElement(toolElementSize, tp, i));
                    }
                }

                FlpToolListSetWidth(FlpToolList.VerticalScroll.Visible ? (toolElementSize.Width + 24) : (toolElementSize.Width + 4));
                ReloadNeded(false);
                ToolList.WriteXML(); // save as default
            }
            else
            {
                if (lastClickedElement > -2)
                    PanelNoXML.Visible = true;
            }
            FlpToolList.Visible = true;
        }

        /******************************************************
         * after loading GCode, compare XML-Tags with tool-list (if a tool-list is applied)
         ******************************************************/
        private void CheckToolListMatch()
        {
            Logger.Info("####### CheckToolListMatch XmlMarker count:{0} ", XmlMarker.GetGroupCount());
            /* Get Group properties from already loaded graphic */
            if (XmlMarker.GetGroupCount() > 0)
            {
                FlpToolList.Visible = false;
                ToolListElementsSetVisible(false);

                BlockData bd = new BlockData();
                for (int i = 0; i < XmlMarker.GetGroupCount(); i++)
                {
                    if (XmlMarker.GetGroupByIndex(ref bd, i))   // get properties of group
                    {
                        if (log) Logger.Trace("from XML-BlockData id:{0}  color:{1}   width:{2}   toolNr:{3}", bd.Id, bd.PenColor, bd.PenWidth, bd.ToolNr);
                        foreach (Control control in FlpToolList.Controls)
                        {
                            if (((UCToolListElement)control).toolProp.ToolNr == bd.ToolNr)
                            {
                                ((UCToolListElement)control).Visible = true;
                            }
                        }
                    }
                }
                FlpToolList.Visible = true;
            }
        }

        private void FlpToolListSetWidth(int w)
        {
            /* align controls on panel */
            w = (int)(w * DpiScaling);
            FlpToolList.Width = w;
            PanelNoXML.Width = w - 20;
            PanelNoXML.Height = FlpToolList.Height - 20;
            LblNoXML.Width = PanelNoXML.Width - 20;
            int l = FlpToolList.Left + w + 3;
            CbApplyToolList.Left = l;
            BtnLoadToolList.Left = l;
            BtnSaveToolList.Left = l + BtnLoadToolList.Width;
            BtnPresetFromColorPalette.Left = l;
            BtnReloadGraphic.Left = l;
            CbGroupSelection.Left = l + 1;
            BtnSetupImageImport.Left = l;
            BtnHelp.Left = l - (int)(42 * DpiScaling);
            BtnSetup.Left = l - (int)(21 * DpiScaling);
            LblDevice.Left = l;
        }
        private void UCToolList_Resize(object sender, EventArgs e)
        {
            FlpToolList.Height = this.Height - 30;
        }

        /******************************************************
         * copy tool-list-element-tag-values to tool list 
         ******************************************************/
        private void UpdateToolListClass(bool renumber = false)
        {
            /* Copy UCToolListElement-ToolProperty to ToolList */
            if (log) Logger.Trace("UpdateToolListClass");
            ToolList.Reset();
            ToolProperty tmp;
            bool replace = groupBy == 0;    /* UpdateToolTip visible tool diameter */
            DeviceSelection dev = MyControl.SelectedDevice;
            foreach (Control control in FlpToolList.Controls)
            {
                tmp = ((UCToolListElement)control).GetToolProperty().Copy();
                if (replace)
                {
                    if (dev == DeviceSelection.Laser)
                        tmp.GroupWidth = tmp.Laser.Diameter;
                    else if (dev == DeviceSelection.Plotter)
                        tmp.GroupWidth = tmp.Plotter.Diameter;
                    else if (dev == DeviceSelection.Router)
                        tmp.GroupWidth = tmp.Router.Diameter;
                }
                if (log) Logger.Trace("UpdateToolList name:{0}  width:{1}  d0:{2}  d1:{3}  d2:{4}", tmp.ToolName, tmp.GroupWidth, tmp.Laser.Diameter, tmp.Plotter.Diameter, tmp.Router.Diameter);
                ToolList.Add(tmp);
            }
            if (renumber)
            {
                ToolList.ReNumberToolNr();
                FillToolListElements();
            }
            ToolList.WriteXML(); // save as default
        }

        /******************************************************
         * Save tool list 
         ******************************************************/
        private void BtnSaveToolList_Click(object sender, EventArgs e)
        {
            if (log) Logger.Trace("BtnSaveToolList_Click");
            if (FlpToolList.Controls.Count < 1)
                return;
            UpdateToolListClass(); //UpdateToolListElements();
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Datapath.Tools,
                Filter = "currentToolList (*.xml)|*.xml",
                FileName = "_currentToolList.xml"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Logger.Trace("BtnSaveToolList_Click Save actual tool list: {0}", sfd.FileName);
                ToolList.WriteXML(sfd.FileName);
            }
            sfd.Dispose();
        }

        /******************************************************
         * Load tool list
         ******************************************************/
        private void BtnLoadToolList_Click(object sender, EventArgs e)
        {
            if (log) Logger.Trace("BtnLoadToolList_Click");
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = Datapath.Tools,
                Filter = "ToolList (*.xml)|*.xml|ToolTable (*.csv)|*.csv",
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Logger.Trace("BtnLoadToolList_Click Load tool list: {0}", ofd.FileName);
                if (ofd.FileName.ToLower().EndsWith("xml"))
                    ToolList.ReadXML(ofd.FileName);
                else
                {
                    ToolTable.ReadCSV(ofd.FileName);
                    ToolList.GetFromToolTable(ToolTable.toolTableArray);
                }
                CbApplyToolList.Checked = true;
            }
            ofd.Dispose();
            if (ofd.FileName != Datapath.Tools + "\\_currentToolList.xml")
                ToolList.WriteXML(); // save as default
            FillToolListElements();
            ReloadNeded();
        }


        /*********************************************************************
         *  RELOAD graphic and apply or UpdateToolTip tool-list
         *********************************************************************/
        private void BtnReloadGraphic_Click(object sender, EventArgs e)
        {
            lastClickedElement = -1;
            Logger.Info("▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀ Reload from Tool list ApplyToolList:{0}   SettingWasChanged:{1}", CbApplyToolList.Checked, MyControl.GetSettingWasChanged());
            Cursor.Current = Cursors.WaitCursor;
            MyControl.ProcessAutomationRunning = false;

            if (CbApplyToolList.Checked)	// apply tool-list
            {
                UpdateToolListClass(true);   	// copy tool-list-element-tag-values to tool list 
            }
            else                            // UpdateToolTip tool-list from raw graphic-impport
            {
                //		if (MyControl.GetSettingWasChanged())	// changes made by device-specific panels?
                {
                    ToolList.Reset();
                    ToolList.Add(MyControl.GetToolsProperties());   // default settings
                    ClearToolListElements();
                    //FillToolListElements(); // clear tool-list-elements, create new tool-list-element-tags from tool-list 			
                }
            }
            ToolList.Log(MyControl.SelectedDevice);
            PanelNoXML.Visible = false;
            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.reloadGraphic));
        }
        internal void BtnReloadGraphicDelayed()
        {
            lastClickedElement = -1;
            if (log) Logger.Trace("BtnReloadGraphicDelayed");
            if (!CbApplyToolList.Checked)
            { CreateToolListFromGraphic(); }
            else
            {
                if (Properties.Settings.Default.importGCToolListHideUnused)
                    CheckToolListMatch();
            }
            ReloadNeded(false);
            if (MyControl.SourceType == SourceType.Image)
            {
                BtnSetupImageImport.BackColor = MyControl.NotifyGreen;
                BtnSetupImageImport.Visible = true;
            }
            else
            { BtnSetupImageImport.BackColor = MyControl.ButtonBackColor; }
        }

        private void TsmiByNr_Click(object sender, EventArgs e)
        { SortByToolNr(false); }
        private void TsmiByNrDescending_Click(object sender, EventArgs e)
        { SortByToolNr(true); }
        private void SortByToolNr(bool invert)
        {
            ToolList.SortByToolNr(invert);
            ToolList.WriteXML(); // save as default
            FillToolListElements();
        }

        private void TsmiByColor_Click(object sender, EventArgs e)
        { SortByColor(false); }
        private void TsmiByColorDescending_Click(object sender, EventArgs e)
        { SortByColor(true); }
        private void SortByColor(bool invert)
        {
            ToolList.SortByColor(invert);
            ToolList.ReNumberToolNr();
            ToolList.WriteXML(); // save as default
            FillToolListElements();
        }

        private void TsmiDelete_Click(object sender, EventArgs e)
        {
            if (lastClickedElement >= 0)
            {
                FlpToolList.Controls.Remove(FlpToolList.Controls[lastClickedElement]);
                UpdateToolListElements();
                //		ToolList.Delete(ToolList);
                ReloadNeded();
            }
            lastClickedElement = -1;
        }

        private void TsmiRenumber_Click(object sender, EventArgs e)
        {
            ToolList.ReNumberToolNr();
            ToolList.WriteXML(); // save as default
            FillToolListElements();
        }

        private void CbApplyToolTable_CheckedChanged(object sender, EventArgs e)
        {
            BtnApplyToolListHighlight();
            //    ReloadNeded();
        }
        private void BtnApplyToolListHighlight()
        {
            if (log) Logger.Trace("BtnApplyToolListHighlight ");
            bool useToolList = CbApplyToolList.Checked;
            MyControl.ApplyToolList = useToolList;
            ToolListElementsSetEnable(useToolList);
            if (useToolList)
            {
                CbApplyToolList.BackColor = MyControl.NotifyYellow;
                CbApplyToolList.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                //    LblDevice.Visible = true;
            }
            else
            {
                CbApplyToolList.BackColor = CheckBox.DefaultBackColor;//  MyControl.ButtonBackColor;
                CbApplyToolList.ForeColor = CheckBox.DefaultForeColor;//MyControl.ButtonForeColor;
                LblDevice.Visible = false;
                ToolListElementsSetVisible(true);
            }
            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.useToolList, useToolList ? 1 : 0));
        }
        private void ToolListElementsSetEnable(bool en)
        {
            if (log) Logger.Trace("SetEnable {0}", en);
            CbGroupSelection.Enabled = !en;
            foreach (Control control in FlpToolList.Controls)
            {
                ((UCToolListElement)control).SetEnable(en);
            }
        }
        private void ToolListElementsSetVisible(bool en)
        {
            foreach (Control control in FlpToolList.Controls)
            {
                ((UCToolListElement)control).Visible = en;
            }
        }
        private void CbGroupSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbGroupSelection.SelectedIndex == 0)
            { MyControl.GroupBy = Graphic.GroupOption.ByColor; groupBy = 0; }
            else if (CbGroupSelection.SelectedIndex == 1)
            { MyControl.GroupBy = Graphic.GroupOption.ByWidth; groupBy = 1; }
            else if (CbGroupSelection.SelectedIndex == 2)
            { MyControl.GroupBy = Graphic.GroupOption.ByLayer; groupBy = 2; }
            ReloadNeded();
        }

        private void BtnExtra_Click(object sender, EventArgs e)
        {
            int tab = _tabDevice;
            if (!HeadlineCoordinate.Visible)
            {
                tab = 3;
                stateShowToolTable = Properties.Settings.Default.gui2DToolTableShow;
                Properties.Settings.Default.gui2DToolTableShow = true;
            }
            else
                Properties.Settings.Default.gui2DToolTableShow = stateShowToolTable;
            Properties.Settings.Default.Save();
            //         OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.guiUpdate));

            HeadlineLaser.Visible = (tab == 0);
            HeadlinePlotter.Visible = (tab == 1);
            HeadlineRouter.Visible = (tab == 2);
            HeadlineCoordinate.Visible = (tab == 3);
            foreach (Control control in FlpToolList.Controls)
            {
                ((UCToolListElement)control).SwitchView(tab, _tabPlotterMode);
            }
        }

        private void NudOffset_ValueChanged(object sender, EventArgs e)
        {
            invalidateIsNeeded = 2;
        }

        public void RestoreColors()
        {
            FlpToolList.BackColor = MyControl.PanelBackColor;
            CbApplyToolTable_CheckedChanged(null, null);
        }

        private void BtnPresetFromColorPalette_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = Datapath.ColorPalette,
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Logger.Trace("BtnPreset_Click Create tool list from color palette: {0}", ofd.FileName);
                Colors.LoadColorPalette(ofd.FileName);

                /* Copy UCToolListElement-ToolProperty to ToolList */
                ToolList.Reset();
                ToolProperty tmp;
                bool replace = true;    /* UpdateToolTip visible tool diameter */
                DeviceSelection dev = MyControl.SelectedDevice;
                short tnr = 1;
                foreach (PaletteEntry cp in Colors.MyPalette)
                {
                    tmp = new ToolProperty();
                    tmp = MyControl.GetToolsProperties().Copy();
                    tmp.GroupColor = cp.Col;
                    tmp.ToolName = cp.Name;
                    tmp.ToolNr = tnr++;
                    if (replace)
                    {
                        if (dev == DeviceSelection.Laser)
                            tmp.GroupWidth = tmp.Laser.Diameter;
                        else if (dev == DeviceSelection.Plotter)
                            tmp.GroupWidth = tmp.Plotter.Diameter;
                        else if (dev == DeviceSelection.Router)
                            tmp.GroupWidth = tmp.Router.Diameter;
                    }
                    if (log) Logger.Trace("UpdateToolList name:{0}  width:{1}  d0:{2}  d1:{3}  d2:{4}", tmp.ToolName, tmp.GroupWidth, tmp.Laser.Diameter, tmp.Plotter.Diameter, tmp.Router.Diameter);
                    ToolList.Add(tmp);
                }
                ToolList.WriteXML(); // save as default
                FillToolListElements();
                ReloadNeded(false);
            }
            ofd.Dispose();
        }

        private void PanelNoXML_DpiChangedAfterParent(object sender, EventArgs e)
        {
            DpiScaling = (float)DeviceDpi / 96;
            FlpToolListSetWidth(FlpToolList.VerticalScroll.Visible ? (toolElementSize.Width + 24) : (toolElementSize.Width + 4));
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

        private void BtnSetupImageImport_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>
            {
                new ControlDefaults(LblSetup1.Text, "importVectorizeAutomatic"),			// Automatic (color) / BW
                new ControlDefaults(LblSetup2.Text, "importVectorizeAlgorithmPoTrace"),		// Po Trace / Geometric Trace
                new ControlDefaults(LblSetup3.Text, "importVectorizeDetectTransparency"),	// BW - find transparency / brightness
                new ControlDefaults(LblSetup4.Text, "importVectorizeThreshold", new decimal[] { 0m, 255m, 1m, 0m }),	// BW brightness threshold
                new ControlDefaults(LblSetup5.Text, "importVectorizePoTraceCurveoptimizing"),
                new ControlDefaults(LblSetup6.Text, "importVectorizeOptimize1")
            };
            MyControl.ShowSimpleSetup(LblSetupHeadline.Text, LblSetupInfo.Text, Cursor.Position, cd);
            MyControl.SettingWasChanged(true);
            //importVectorizeOptimize1
        }

        private void BtnSetup_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>
            {
                new ControlDefaults(LblSetupToolHideUnused.Text, "importGCToolListHideUnused"),
            };
            MyControl.ShowSimpleSetup(LblSetupToolHeadline.Text, "", Cursor.Position, cd);
        }
    }
}
