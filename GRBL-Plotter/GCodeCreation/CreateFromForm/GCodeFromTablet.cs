/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2025 Sven Hasemann contact: svenhb@web.de

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
 * 2025-08-01 new form

// inspiration
 https://learn.microsoft.com/en-us/uwp/api/windows.ui.input.inking.inkpresenter.updatedefaultdrawingattributes?view=winrt-26100
 https://www.codeproject.com/Articles/19102/Adventures-into-Ink-API-using-WPF
 https://wiki.evilmadscientist.com/RoboPaint_RT
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Xml;
using Button = System.Windows.Forms.Button;
using Color = System.Drawing.Color;

namespace GrblPlotter
{
    public partial class GCodeFromTablet : Form
    {
        private bool lastIsDrawing = false;
        private bool isDrawingByStylus = false;
        private bool isDrawingByMouse = false;
        private double lastX = 0, lastY = 0, lastZ = 0, lastDrawX = 0, lastDrawY = 0, lastDrawZ = 0;
        private bool timerTick = false;
        private long figureCount = 1;
        private Color figureColor = Color.Black;
        private int toolNr = 1;
        private int toolNrLast = 1;
        private double figurePenSize = 1;
        private bool figureStarted = false;
        private double canvasToPlotSize = 1;

        private bool recordDrawing = true;
        private string fileName;
        internal StringBuilder gcodeString = new StringBuilder();
        private long gcodeLines = 0;
        private double gcodeDistancePD = 0;
        private string loadingPath = Datapath.Examples;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private bool enableLog = false;

        #region form
        public GCodeFromTablet()
        {
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Logger.Info("++++++ GCodeFromTablet START ++++++");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
            this.Icon = Properties.Resources.Icon;
        }

        private void GCodeFromTablet_Load(object sender, EventArgs e)
        {
            string defaultToolList = Datapath.Tools + "\\" + ToolTable.DefaultFileName;
            LoadToolListSetBtn(defaultToolList);                  // list tools from _current.csv	
            ResizeCanvas();
            figurePenSize = (double)NudSizePen.Value;
            Tablet.PenSize = Tablet.Width * (double)(NudSizePen.Value / NudSizeX.Value);
            Logger.Trace("GCodeFromTablet_Load Plotter X:{0}  Y:{1}  NudPen:{2}   Canvas X:{3}  Y:{4}  TabletPen:{5}", NudSizeX.Value, NudSizeY.Value, NudSizePen.Value, Tablet.Width, Tablet.Height, Tablet.PenSize);
            canvasToPlotSize = (double)(Tablet.Width / NudSizeX.Value);
            UpdateToolTableList();      // show tool-files and last loaded tools
            SetCanvasColor();
			
            this.Size = Properties.Settings.Default.tabletFormSize;
            this.Location = Properties.Settings.Default.tabletFormLocation;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }

            LoadPaperSizePreset();
            Tablet.Clear();
            SetupPanel.Location = new Point(0, 26);
            SetupPanel.Size = new Size(780, 580);
        }

        private void GCodeFromTablet_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Info("++++++ GCodeFromTablet STOP ++++++");
            //    Properties.Settings.Default.locationImageForm = Location;
            Properties.Settings.Default.tabletFormSize = this.Size;
            Properties.Settings.Default.tabletFormLocation = this.Location;
            Properties.Settings.Default.Save();
        }

        private void GCodeFromTablet_Resize(object sender, EventArgs e)
        {
            const int dWidth = 800;
            const int dGbColorWidth = 360;
            const int dFlpToolSWidth = 158;
            const int dGbColorWGap = dWidth - dGbColorWidth;
            const int dFlpToolSWGap = dWidth - dFlpToolSWidth;
            //    FlpToolSelection.Height = Height - 296;
            //    GbToolChange.Top = Height - 198;
            //    Tablet.Width = PTablet.Width - 2;
            BtnHelp.Left = Width - 40;
            GbColor.Width = Width - dGbColorWGap;
            FlpToolSelection.Width = Width - dFlpToolSWGap;
            ResizeCanvas();
        }

        private void ResizeCanvas()
        {
            double x = (double)NudSizeX.Value;
            double y = (double)NudSizeY.Value;
            double rPlotter = x / y;
            double rCanvas = TabletPanel.Width / TabletPanel.Height;

            if (rPlotter > rCanvas)
            {
                Tablet.Width = TabletPanel.Width - 2;
                Tablet.Height = (int)(y * TabletPanel.Width / x) - 23;
            }
            else
            {
                Tablet.Height = TabletPanel.Height - 23;
                Tablet.Width = (int)(x * TabletPanel.Height / y) - 2;
            }
            canvasToPlotSize = (double)(Tablet.Width / NudSizeX.Value);
            if (enableLog) Logger.Trace("ResizeCanvas Plotter X:{0}  Y:{1}  Pen:{2}   Canvas X:{3}  Y:{4}  Pen:{5}", x, y, NudSizePen.Value, Tablet.Width, Tablet.Height, Tablet.PenSize);
        }
        #endregion


        #region tablet
        /* create GCode on the fly */
        private void Tablet_TabletEvent(object sender, TabletControl.TabletEventArgs e)
        {
            //    Logger.Trace("Stylus:{0}  X:{1:0.00}  Y:{2:0.00}  Z:{3:0.00}  Draw:{4}  Air:{5}",e.IsStylus,e.PosX,e.PosY,e.PosZ, e.IsDrawing, e.IsAir);
            //    return;
            // get tablet coordinates in px and pressure z in range 0 to 1
            double realX = (double)NudSizeX.Value * e.PosX / Tablet.Width;
            double realY = (double)NudSizeY.Value - (double)NudSizeY.Value * e.PosY / Tablet.Height;
            double realZ = CalculateSorZ_Value(e.PosZ);
            double minMove = 0.1;
            bool posXyChanged = ((Math.Abs(realX - lastDrawX) > minMove) || (Math.Abs(realY - lastDrawY) > minMove));
            //    bool posChanged = ((Math.Abs(realX - lastX) > minMove) || (Math.Abs(realY - lastY) > minMove) || (Math.Abs(realZ - lastZ) > minMove));
            bool posXyzChanged = ((Math.Abs(realX - lastDrawX) > minMove) || (Math.Abs(realY - lastDrawY) > minMove) || (Math.Abs(realZ - lastDrawZ) > minMove));

            // filter events depending if drawing was started with stylus or mouse
            if (isDrawingByStylus && !e.IsStylus)
                return;
            if (isDrawingByMouse && e.IsStylus)
                return;

            bool penLift = e.IsDrawing != !lastIsDrawing;
            bool penDown = e.IsDrawing && !lastIsDrawing;
            bool penUp = !e.IsDrawing && lastIsDrawing;

            //    Logger.Trace("Event isDraw:{0} lastIsDraw:{1} isStyl:{2}  isAir:{3} rX:{4:0.0}  rY:{5:0.0}  rZ:{6:0.0} posXyChanged:{7}", e.IsDrawing, lastIsDrawing, e.IsStylus, e.IsAir, realX, realY, realZ, posXyChanged);
            PositionLabelUpdate(e.IsDrawing, e.IsStylus, realX, realY, e.PosZ, realZ, penLift, penDown, penUp);

            string cmd;// = e.IsDrawing ? "G1" : "G0";
            if (e.IsStylus || CbMovementMouse.Checked)
            {
                if (penDown)
                {
                    isDrawingByStylus = e.IsStylus;
                    isDrawingByMouse = !e.IsStylus;
                    PenDown(realX, realY);  // move pen down
                    lastDrawX = realX; lastDrawY = realY;
                }
                // move plotter if pen is in the air
                if (CbMovementPenUp.Checked && !e.IsDrawing && timerTick && RbMode0.Checked && posXyChanged)
                {	// don't record air moves
                    if (enableLog) Logger.Trace("Jog  X:{0:0.0}  Y:{1:0.0}", realX, realY);
                    // Jog commands causes hang-up of grbl
                    //    SendCommandEvent(new CmdEventArgs((string.Format("stop;$J=G90X{0:0.00}Y{1:0.00}F10000", realX, realY).Replace(',', '.'))));
                    SendCommandEvent(new CmdEventArgs((string.Format("G90G0X{0:0.00}Y{1:0.00}", realX, realY).Replace(',', '.'))));
                    lastDrawX = realX; lastDrawY = realY;
                    timerTick = false;
                }
                else if (e.IsDrawing && figureStarted)
                {
                    cmd = "";// G1F1000";
                    // draw with stylus-pressure
                    if (e.IsDrawing && posXyzChanged)
                    {
                        if (Properties.Settings.Default.importGCPWMEnable)
                        { cmd = string.Format("{0}X{1:0.00}Y{2:0.00}S{3:0} ", cmd, realX, realY, realZ).Replace(',', '.'); }
                        else if (Properties.Settings.Default.importGCZEnable)
                        { cmd = string.Format("{0}X{1:0.00}Y{2:0.00}Z{3:0.00}", cmd, realX, realY, realZ).Replace(',', '.'); }
                        lastDrawX = realX; lastDrawY = realY; lastDrawZ = realZ;
                    }

                    double diffX = realX - lastX;
                    double diffY = realY - lastY;
                    gcodeDistancePD += Math.Sqrt(diffX * diffX + diffY * diffY);

                    if (cmd.Length > 0)
                    {
                        SendCommandEvent(new CmdEventArgs(cmd));
                        //RecordDrawing(cmd);
                    }
                }

                if (penUp)
                {
                    if (PenUp() && CbUpdate.Checked)
                        Update2DView();
                    isDrawingByStylus = isDrawingByMouse = false;
                }
            }
            lastIsDrawing = e.IsDrawing;
            lastX = realX; lastY = realY; lastZ = realZ;
        }

        private void PositionLabelUpdate(bool draw, bool stylus, double x, double y, double press, double z, bool pl, bool pd, bool pu)
        {
            string cmdZ = "";
            if (Properties.Settings.Default.importGCPWMEnable)  // to avoid error 9, do jog command at last
            {
                if (!draw)
                { cmdZ = string.Format("S:{0:0}", Properties.Settings.Default.importGCPWMUp); }
                else
                {
                    /*     if (!stylus)
                         { cmdZ = string.Format("S:{0:0}", Properties.Settings.Default.importGCPWMDown); }
                         else*/
                    { cmdZ = string.Format("S:{0:0}", z); }
                }
            }
            if (Properties.Settings.Default.importGCZEnable)  // to avoid error 9, do jog command at last
            {
                if (!draw)
                { cmdZ = string.Format("Z:{0:0.000}", Properties.Settings.Default.importGCZUp); }
                else
                {
                    if (!stylus)
                    { cmdZ = string.Format("Z:{0:0.000}", Properties.Settings.Default.importGCZDown); }
                    else
                    { cmdZ = string.Format("Z:{0:0.000}", z); }
                }
            }
            TssLblActualPos.Text = string.Format("Pos.: X:{0,6:0.0}  Y:{1,6:0.0}  Pressure:{2,6:0.0}%  {3,5};     Paper size: X:{4:0.0}  Y:{5:0.0}   pl:{6}   pd:{7}   pu:{8}", x, y, press * 100, cmdZ, NudSizeX.Value, NudSizeY.Value, pl, pd, pu);
        }

        private bool PenUp()
        {
            if (enableLog) Logger.Trace("PenUp  figureStarted:{0}", figureStarted);
            if (!figureStarted)
                return false;
            figureStarted = false;

            string cmd = "";
            if (Properties.Settings.Default.importGCPWMEnable)  // to avoid error 9, do jog command at last
            {
                cmd = string.Format("M3S{0} (PU)", Properties.Settings.Default.importGCPWMUp);
                if (Properties.Settings.Default.importGCPWMDlyUp > 0)
                    cmd += string.Format(";G{0}P{1}", Gcode.FrmtCode(4), Gcode.FrmtNum(Properties.Settings.Default.importGCPWMDlyUp));
            }
            if (Properties.Settings.Default.importGCZEnable)
            {
                cmd = string.Format("G0Z{0} (PU)", Gcode.FrmtNum(Properties.Settings.Default.importGCZUp), Properties.Settings.Default.importGCZFeed);
            }
            cmd = cmd.Replace(",", ".");
            SendCommandEvent(new CmdEventArgs(cmd));
            //RecordDrawing(cmd);
            //RecordDrawing(string.Format("({0}>)", XmlMarker.FigureEnd));
            return true;
        }

        private void PenDown(double realX, double realY)
        {
            if (enableLog) Logger.Trace("PenDown figureStarted:{0}", figureStarted);
            if (figureStarted)
                return;
            figureStarted = true;

            /* for 2D view update, set XML tag
            string fColor = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}",
                     figureColor.A, figureColor.R, figureColor.G, figureColor.B);
            if (!CbTransparency.Checked)
                fColor = ColorTranslator.ToHtml(figureColor);

            if (fColor.StartsWith("#"))
                fColor = fColor.Substring(1);
            string cmd = string.Format("({0} Id=\"{1}\" Geometry=\"Draw\" PenColor=\"{2}\" PenWidth=\"{3:0.0}\">)", XmlMarker.FigureStart, figureCount++, fColor, figurePenSize).Replace(',', '.');
            RecordDrawing(cmd);
            */

            if (toolNr != toolNrLast)
                ToolChange(toolNr, ColorTranslator.ToHtml(figureColor));
            toolNrLast = toolNr;

            // move via G90 G0 to 1st position
            string cmd = string.Format("{0} X{1:0.0} Y{2:0.0} F10000", "G90\r\nG00", realX, realY).Replace(',', '.');
            SendCommandEvent(new CmdEventArgs(cmd));
            //RecordDrawing(cmd);

            // set S value, then G1 move
            if (Properties.Settings.Default.importGCPWMEnable)
            {
                cmd = string.Format("M3S{0:0} (PD)", Properties.Settings.Default.importGCPWMZero);
                if (Properties.Settings.Default.importGCPWMDlyDown > 0)
                    cmd += string.Format(";G{0}P{1}", Gcode.FrmtCode(4), Gcode.FrmtNum(Properties.Settings.Default.importGCPWMDlyDown));
                cmd += ";G1F10000";
            }

            // or set Z value via G1
            else if (Properties.Settings.Default.importGCZEnable)
            {
                cmd = string.Format("G1Z{0:0.0}F10000 (PD)", 0);//, Properties.Settings.Default.importGCZFeed);
            }

            cmd = cmd.Replace(",", ".");
            SendCommandEvent(new CmdEventArgs(cmd));
            //RecordDrawing(cmd);
        }

        private double CalculateSorZ_Value(double z)
        {
            if (z > 1) z = 1;
            if (z < 0) z = 0;
            if (Properties.Settings.Default.importGCPWMEnable)
            {
                decimal range = Properties.Settings.Default.importGCPWMDown - Properties.Settings.Default.importGCPWMZero;
                return (double)Properties.Settings.Default.importGCPWMZero + (double)range * z;
            }
            else if (Properties.Settings.Default.importGCZEnable)
            {
                return (double)Properties.Settings.Default.importGCZDown * z;
            }
            return z;
        }

        /* deliver simple GCode from collected data */
        internal StringBuilder GetCode()
        {
            StringBuilder header = new StringBuilder(string.Format("( {0} by GRBL-Plotter {1} )\r\n", "Direct draw", MyApplication.GetVersion()));
            header.AppendFormat("( G-Code lines      : {0} )\r\n", gcodeLines);
            header.AppendFormat("( Path length (PD)  : {0:0.0} mm )\r\n", gcodeDistancePD);

            string[] commands = Properties.Settings.Default.importGCHeader.Split(';');
            foreach (string cmd in commands)
                if (cmd.Length > 1)
                { header.AppendFormat("{0} (Setup - GCode-Header)\r\n", cmd.Trim()); gcodeLines++; }

            if (Properties.Settings.Default.importGCPWMEnable)
                header.AppendFormat("({0} Min=\"{1:0.0}\" Max=\"{2:0.0}\" Width=\"{3:0.0}\" />)\r\n", XmlMarker.HalftoneS, Properties.Settings.Default.importGCPWMZero, Properties.Settings.Default.importGCPWMDown, figurePenSize).Replace(",", ".");
            else if (Properties.Settings.Default.importGCZEnable)
                header.AppendFormat("({0} Min=\"{1:0.0}\" Max=\"{2:0.0}\" Width=\"{3:0.0}\" />)\r\n", XmlMarker.HalftoneZ, 0, Properties.Settings.Default.importGCZDown, figurePenSize).Replace(",", ".");

            header.Append(gcodeString);
            header.Append(Gcode.GetFooter());
            return header;
        }

        private void Update2DView()
        {	// called by refresh btn, triggers main form to pick-up Gcode via GetCode()
            Logger.Trace("Update2DView");
            SendCodeEvent(new CmdEventArgs("update"));
            //   VisuGCode.pathBackground;
        }

        /* Read complete inkCanvas and generate GCode */
        private void GenerateGCode()
        {	/* called by BtnImport_Click, which also triggers MainFormOtherForms.cs GetGCodeFromTablet
			   Read all Strokes from InkCanvas and copy to Graphic-class 
			   GetGCodeFromTablet() copies result from Graphic-class to 2D-View
			   */
            int strokeCount = Tablet.GetStrokeCount();
            Logger.Trace("###### Import strokes {0}", strokeCount);
            if (strokeCount > 0)
            {
                VisuGCode.pathBackground.Reset();
                Graphic.CleanUp();
                Graphic.Init(Graphic.SourceType.Text, "", null, null);
                Graphic.graphicInformation.DxfImportZ = false;
                Graphic.graphicInformation.OptionZFromWidth = false;
                Graphic.graphicInformation.OptionSFromWidth = false;

                Graphic.graphicInformation.PenWidthMin = 0;
                Graphic.graphicInformation.PenWidthMax = (double)NudSizePen.Value;

                if (Properties.Settings.Default.importGCPWMEnable)
                    Graphic.graphicInformation.OptionSFromWidth = true;
                else if (Properties.Settings.Default.importGCZEnable)
                {
                    Graphic.graphicInformation.OptionZFromWidth = true;
                    Graphic.graphicInformation.DxfImportZ = true;
                }
                Graphic.graphicInformation.ApplyHatchFill = false;
                Graphic.graphicInformation.OptionNodesOnly = false;
                Graphic.graphicInformation.OptionCodeSortDistance = Properties.Settings.Default.importGraphicSortDistance; //false;
                Graphic.graphicInformation.FigureEnable = true;
                Graphic.graphicInformation.GroupEnable = true;
                Graphic.graphicInformation.GroupOption = Graphic.GroupOption.ByColor;
                Graphic.maxObjectCountBeforeReducingXML = 0;   // no limit

                Graphic.SetType("Ink");

                /* save settings */
                decimal origWidthMax = Properties.Settings.Default.importDepthFromWidthMax;
                decimal zMin = Properties.Settings.Default.importDepthFromWidthMin;
                decimal zMax = Properties.Settings.Default.importDepthFromWidthMax;
                bool pwmFromWidth = Properties.Settings.Default.importPWMFromWidth;
                Properties.Settings.Default.importDepthFromWidthMin = 0;
                Properties.Settings.Default.importDepthFromWidthMax = Properties.Settings.Default.importGCZDown;
                Properties.Settings.Default.importPWMFromWidth = Graphic.graphicInformation.OptionSFromWidth;
                Properties.Settings.Default.Save();

                Logger.Trace("##### GenerateGCode S:{0}  Z:{1}  Pen-min:{2}  Pen-max:{3}", Graphic.graphicInformation.OptionSFromWidth, Graphic.graphicInformation.OptionZFromWidth, Graphic.graphicInformation.PenWidthMin, Graphic.graphicInformation.PenWidthMax);
                Logger.Trace("##### GenerateGCode min:{0}  max:{1}", Properties.Settings.Default.importDepthFromWidthMin, Properties.Settings.Default.importDepthFromWidthMax);

                for (int i = 0; i < strokeCount; i++)
                {
                    List<Point3D> list = Tablet.GetStroke(i, out Color col, out double widthPx);
                    double widthMM = (double)NudSizePen.Value;
                    string fColor = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", col.A, col.R, col.G, col.B);
                    Graphic.SetPenColor(fColor);
                    Graphic.SetPenWidth(widthMM.ToString().Replace(",", "."));
                    Graphic.SetGeometry("Ink");
                    Graphic.StartPath(Translate(list[0].X, list[0].Y), list[0].Z * widthMM);                  // start next path
                    Logger.Trace("# Stroke Nr.:{0,4}  {1,35}  width-px:{2:0.00}  mm:{3:0.00}", i, col, widthPx, widthMM);
                    foreach (Point3D p in list)
                    {
                        Graphic.AddLine(Translate(p.X, p.Y), p.Z * widthMM);
                    }
                    Graphic.StopPath();
                }

                Graphic.CreateGCode();          // result is saved as stringbuilder in Graphic.GCode;
                                                //	VisuGCode.pathBackground

                /* restore settings */
                Properties.Settings.Default.importDepthFromWidthMax = origWidthMax;
                Properties.Settings.Default.importDepthFromWidthMin = zMin;
                Properties.Settings.Default.importDepthFromWidthMax = zMax;
                Properties.Settings.Default.importPWMFromWidth = pwmFromWidth;
            }
        }

        private System.Windows.Point Translate(double x, double y)
        {
            double realX = (double)NudSizeX.Value * x / Tablet.Width;
            double realY = (double)NudSizeY.Value - (double)NudSizeY.Value * y / Tablet.Height;
            return new System.Windows.Point(realX, realY);
        }

        /* Remove surplus points, smooth positions 
        private void SmoothStrokes()
        {
            double distance = 0.1;
            int smoothFactor = 3;
            Tablet.Smooth(distance, smoothFactor);
        }
        */

        private void Timer_Tick(object sender, EventArgs e)
        {
            timerTick = true;
        }

        #endregion

        #region notify main form 
        /* MainFormOtherForms.cs OnRaiseTabletCmdEvent */
        public event EventHandler<CmdEventArgs> RaiseCmdEvent;

        protected virtual void SendCommandEvent(CmdEventArgs e)
        {
            if (liveUpdateToolStripMenuItem.Checked)//CbEnableMovement.Checked)
                RaiseCmdEvent?.Invoke(this, e);
        }

        protected virtual void SendCodeEvent(CmdEventArgs e)
        {
            RaiseCmdEvent?.Invoke(this, e);
        }
        #endregion

        #region recording
        private void RecordDrawing(string cmd)
        {
            if (recordDrawing)
            {
                gcodeString.AppendLine(cmd);
                gcodeLines++;
            }
        }

        #endregion

        #region tool table	
        private void ToolChange(int toolnr, string cmt)
        {
            bool gcodeToolChange = Properties.Settings.Default.importGCTool;
            bool gcodeToolChangeM0 = Properties.Settings.Default.importGCToolM0;

            if (gcodeToolChange)                // otherweise no command needed
            {
                string toolCmd = string.Format("T{0:D2} M{1} (Tool:{2} {3})", toolnr, (6), toolnr, cmt);
                if (gcodeToolChangeM0)
                { gcodeString.AppendFormat("M0 (Tool:{0}  Color:{1})\r\n", toolnr, cmt); gcodeLines++; }
                else
                { gcodeString.AppendFormat("{0}\r\n", toolCmd); gcodeLines++; }
            }
        }

        /* reload file list and update tool buttons */
        private void UpdateToolTableList()
        {
            FillToolTableFileList(Datapath.Tools);			// list tool table files
            CboxToolFiles.Text = Properties.Settings.Default.toolTableLastLoaded;
            string defaultToolList = Datapath.Tools + "\\" + ToolTable.DefaultFileName;
            LoadToolListSetBtn(defaultToolList);                  // list tools from _current.csv			
        }

        /* list all CSV files from tools-folder in combobox */
        private void FillToolTableFileList(string filepath)
        {
            try
            {
                string[] Files = System.IO.Directory.GetFiles(filepath);
                CboxToolFiles.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i].ToLower().EndsWith("csv"))
                    {
                        string name = Path.GetFileName(Files[i]);
                        if (name != ToolTable.DefaultFileName)
                            CboxToolFiles.Items.Add(name);
                    }
                }
            }
            catch (Exception err) { Logger.Error(err, "FillToolTableFileList "); }
        }

        /* copy selected tool-file to _current.csv */
        private void CboxToolFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CboxToolFiles.Text))
            {
                string newTool = Datapath.Tools + "\\" + CboxToolFiles.Text;
                string defTool = Datapath.Tools + "\\" + ToolTable.DefaultFileName;
                try
                {
                    System.IO.File.Copy(newTool, defTool, true);
                    Properties.Settings.Default.toolTableLastLoaded = CboxToolFiles.Text;
                    Properties.Settings.Default.Save();
                }
                catch (Exception err)
                {
                    Logger.Error(err, "CboxToolFiles_SelectedIndexChanged could not copy file to default {0}", newTool);
                    EventCollector.StoreException("CboxToolFiles_SelectedIndexChanged: " + newTool + " " + err.Message + " - ");
                }

                LoadToolListSetBtn(defTool);
                SendCodeEvent(new CmdEventArgs("tool"));
            }
        }

        /* Display tools from selected tool table */
        private bool LoadToolListSetBtn(string file)
        {
            if (File.Exists(file))
            {
                Logger.Trace("Load Tool Table {0}", file);
                FlpToolSelection.Controls.Clear();
                string[] readText;

                try
                {
                    readText = File.ReadAllLines(file);
                }
                catch (IOException err)
                {
                    // read already opened file???
                    // https://stackoverflow.com/questions/9759697/reading-a-file-used-by-another-process
                    EventCollector.StoreException("LoadToolList IOException: " + file + " " + err.Message + " - ");
                    Logger.Error(err, "LoadToolList IOException:{0}", file);
                    MessageBox.Show("Could not read " + file + "\r\n" + err.Message, "Error");
                    return false;
                }
                catch
                {//(Exception err) { 
                    throw;      // unknown exception...
                }

                string[] col;
                int toolNr;
                string toolColor;
                string toolName;

                int index = 0;
                int toolCnt = readText.Length;
                //    double flpRatio = (double)FlpToolSelection.Width / FlpToolSelection.Height;
                int w = FlpToolSelection.Width;
                int btnSize = 44;
                if (toolCnt * btnSize > 2 * w)
                    btnSize = 33;
                if (toolCnt * btnSize > 3 * w)
                    btnSize = 22;
                foreach (string s in readText)
                {
                    if (s.StartsWith("#") || s.StartsWith("/"))     // jump over comments
                        continue;

                    if (s.Length > 10)
                    {
                        col = s.Split(',');
                        toolNr = Convert.ToInt32(col[0].Trim());
                        toolColor = col[1].Trim();
                        toolName = col[2].Trim();

                        Button btn = new Button
                        {
                            Name = "btn_" + index.ToString(),
                            //    btn.Text = toolName;
                            Tag = toolNr,
                            Font = new Font("Arial", 10f, FontStyle.Bold),
                            BackColor = ColorTranslator.FromHtml('#' + toolColor)
                        };
                        btn.ForeColor = ContrastColor(btn.BackColor);
                        btn.Height = btn.Width = btnSize;
                        btn.Margin = new System.Windows.Forms.Padding(0);
                        btn.Padding = new System.Windows.Forms.Padding(0);
                        btn.Click += BtnToolSelection_Click;
                        toolTip1.SetToolTip(btn, string.Format("{0}\r\nTool:{1} change pos. \r\nX:{2} \r\nY:{3} \r\nZ:{4} \r\nA:{5}", toolName, col[0], col[3], col[4], col[5], col[6]));
                        FlpToolSelection.Controls.Add(btn);
                        //    FlpToolSelection.SetFlowBreak(btn, false);
                    }
                }
                return true;
            }
            return false;
        }

        /* tool button was clicked */
        private void BtnToolSelection_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Color toolColor = btn.BackColor;
            PSelectedColor.BackColor = toolColor;
            toolTip1.SetToolTip(PSelectedColor, btn.Text);
            toolNr = (int)btn.Tag;

            Tablet.ForeColor = figureColor = Color.FromArgb(128, toolColor.R, toolColor.G, toolColor.B);    // add transparency
            if (!CbTransparency.Checked)
                Tablet.ForeColor = figureColor = toolColor;    // add transparency

            Logger.Trace("btnToolSelection_Click toolNr:{0}  Color:{1}", toolNr, toolColor);

            if (RbMode1.Checked)
                Tablet.SetSelectionColor(figureColor);
        }

        /* set inkcanvas pen color */
        private void SetCanvasColor()
        {
            Color toolColor = PSelectedColor.BackColor;
            Tablet.ForeColor = figureColor = Color.FromArgb(128, toolColor.R, toolColor.G, toolColor.B);    // add transparency
            if (!CbTransparency.Checked)
                Tablet.ForeColor = figureColor = toolColor;    // add transparency
        }

        private static Color ContrastColor(Color color)
        {
            int d;
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }


        /* nok */
        private void Tablet_KeyDown(object sender, KeyEventArgs e)
        {

        }
        #endregion

        #region controls
        private void RbMode_CheckedChanged(object sender, EventArgs e)
        {
            string nam = ((System.Windows.Forms.RadioButton)sender).Name;
            if (nam.EndsWith("0")) Tablet.SetEditMode(0);
            else if (nam.EndsWith("1")) Tablet.SetEditMode(1);
            else if (nam.EndsWith("2")) Tablet.SetEditMode(2);
            else if (nam.EndsWith("3")) Tablet.SetEditMode(3);
        }
        private void CbFitToCurve_CheckedChanged(object sender, EventArgs e)
        {
            Tablet.FitToCurve = CbFitToCurve.Checked;
        }

        private void NudSizeX_ValueChanged(object sender, EventArgs e)
        {
            ResizeCanvas();
        }

        private void NudSizePen_ValueChanged(object sender, EventArgs e)
        {
            figurePenSize = (double)NudSizePen.Value;
            Tablet.PenSize = Tablet.Width * (double)(NudSizePen.Value / NudSizeX.Value);
            Logger.Trace("NudSizePen_ValueChanged Plotter X:{0}  Y:{1}  NudPen:{2}   Canvas X:{3}  Y:{4}  TabletPen:{5}", NudSizeX.Value, NudSizeY.Value, NudSizePen.Value, Tablet.Width, Tablet.Height, Tablet.PenSize);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            Update2DView();
        }

        private void CbTransparency_CheckedChanged(object sender, EventArgs e)
        {
            SetCanvasColor();
        }
        private void BtnOpenSetup_Click(object sender, EventArgs e)
        {
            SendCodeEvent(new CmdEventArgs("setup"));
        }

        private void CboxPapersize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = CboxPapersize.SelectedIndex;
            if ((i >= 0) && (i < papersizePreset.Count))
            {
                NudSizeX.Value = (decimal)papersizePreset[i].X;
                NudSizeY.Value = (decimal)papersizePreset[i].Y;
            }

            Tablet.CorrectRatio((double)NudSizeX.Value * canvasToPlotSize, (double)NudSizeY.Value * canvasToPlotSize);
        }

        private void BtnSwitchXy_Click(object sender, EventArgs e)
        {
            decimal tmp = NudSizeX.Value;
            NudSizeX.Value = NudSizeY.Value;
            NudSizeY.Value = tmp;
            Tablet.CorrectRatio((double)NudSizeX.Value * canvasToPlotSize, (double)NudSizeY.Value * canvasToPlotSize);
        }

        private void BtnReduce_Click(object sender, EventArgs e)
        {
            Tablet.PointDistance = (double)NudPointDistance.Value;
            Tablet.ReducePointsInAllStrokes((double)NudPointDistance.Value);
            TssLblCanvasData.Text = Tablet.GetStrokesData();
        }

        private void BtnFitCurve_Click(object sender, EventArgs e)
        {
            Tablet.FitCurve(0.1);
            TssLblCanvasData.Text = Tablet.GetStrokesData();
        }

        private void NudPointDistance_ValueChanged(object sender, EventArgs e)
        {
            Tablet.PointDistance = (double)NudPointDistance.Value;
        }

        private void MenuSetup_Click(object sender, EventArgs e)
        {
            SetupPanel.Visible = !SetupPanel.Visible;
            SetupPanel.Width = Width - 10;
            SetupPanel.Height = Height - 30;
            ToolPanel.Visible = TabletPanel.Visible = !SetupPanel.Visible;
        }
        private void MenuHead_Click(object sender, EventArgs e)
        {
            SetupPanel.Visible = false;
            ToolPanel.Visible = TabletPanel.Visible = true;
        }
        private void BtnCloseSetup_Click(object sender, EventArgs e)
        {
            SetupPanel.Visible = false;
            ToolPanel.Visible = TabletPanel.Visible = true;
        }

        private void MenuNewClear_Click(object sender, EventArgs e)
        {
            ClearCanvas();
            //   Tablet.Clear();
            //   canvasToPlotSize = (double)(Tablet.Width / NudSizeX.Value);
        }
        private void ClearCanvas()
        {
            Tablet.Clear();
            canvasToPlotSize = (double)(Tablet.Width / NudSizeX.Value);
            string defaultToolList = Datapath.Tools + "\\" + ToolTable.DefaultFileName;
            LoadToolListSetBtn(defaultToolList);                  // list tools from _current.csv						
            figureCount = 1;
            Tablet.Clear();
            gcodeString.Clear();
            gcodeLines = 0;
            gcodeDistancePD = 0;
        }

        private void MenuOpen_Click(object sender, EventArgs e)
        {
            loadingPath = Tablet.StrokesLoad(loadingPath);
            TssLblCanvasData.Text = Tablet.GetStrokesData();
        }

        private void MenuSave_Click(object sender, EventArgs e)
        {
            int cnt = Tablet.GetStrokeCount();
            Logger.Trace("###### Save strokes {0}", cnt);
            Tablet.StrokesSave(Datapath.Examples);
        }

        private void GCodeFromTablet_KeyDown(object sender, KeyEventArgs e)
        {
            Logger.Trace("GCodeFromTablet_KeyDown");
            Logger.Trace(string.Format("CTRL+V  {0}  {1}", e.KeyCode, e.Modifiers));
            if (e.KeyCode == Keys.V && e.Control)//.Modifiers == Keys.Control)
            {
                Logger.Trace(string.Format("CTRL+V  {0}  {1}", e.KeyCode, e.Modifiers));
                MessageBox.Show("CTRL+V");
                string txt = Clipboard.GetText().Replace(',', '.');
                ImportData(txt);
                e.SuppressKeyPress = true;
            }
        }
        private void ImportData(string txt)
        { // https://www.mathe-fa.de/de#result
            Logger.Trace("ImportData");
            if (txt != null)
            {
                var lines = Regex.Split(txt, "\r\n|\r|\n");
                double yDefault = Tablet.Height / 2;
                if (lines.Length > 0)
                {
                    List<System.Windows.Media.Media3D.Point3D> xyz = new List<System.Windows.Media.Media3D.Point3D>();
                    int count = 1;
                    foreach (string line in lines)
                    {
                        var coord = Regex.Split(line.Trim(), "\t|;");
                        double x = 0, y = 0, z = 0;
                        if (coord.Length > 2)
                        {
                            if (!double.TryParse(coord[0], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out x))
                            { Logger.Trace("ImportData nok X '{0}'", coord[0]); }

                            if (!double.TryParse(coord[1], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out y))
                            { Logger.Trace("ImportData nok Y '{0}'", coord[1]); }

                            if (!double.TryParse(coord[2], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out z))
                            { Logger.Trace("ImportData nok Z '{0}'", coord[2]); }
                            if (count++ < 10) Logger.Trace("ImportData add '{0}' -> {1}   '{2}' -> {3}   '{4}' -> {5}", coord[0], x, coord[1], y, coord[2], z);
                        }
                        else if (coord.Length > 1)
                        {
                            if (!double.TryParse(coord[0], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out x))
                            { Logger.Trace("ImportData nok X '{0}'", coord[0]); }

                            y = yDefault;

                            if (!double.TryParse(coord[1], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out z))
                            { Logger.Trace("ImportData nok Z '{0}'", coord[1]); }
                            if (count++ < 10) Logger.Trace("ImportData add '{0}' -> {1}   '{2}' -> {3}", coord[0], x, coord[1], z);
                        }
                        xyz.Add(new System.Windows.Media.Media3D.Point3D(x, y, z));
                    }

                    if (xyz.Count > 0)
                    {
                        Tablet.AddStroke(xyz);
                    }
                }
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            string txt = Clipboard.GetText().Replace(',', '.');
            ImportData(txt);
        }

        private void MenuImportXyzData(object sender, EventArgs e)
        {
            string txt = Clipboard.GetText().Replace(',', '.');
            ImportData(txt);
        }

        private void MenuImportWholeDrawing_Click(object sender, EventArgs e)
        {
            GenerateGCode();
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            string url = Datapath.HelpURL + "?";
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
        #endregion


        //List
        private readonly List<System.Windows.Point> papersizePreset = new List<System.Windows.Point>();
        private readonly XmlReaderSettings settings = new XmlReaderSettings()
        { DtdProcessing = DtdProcessing.Prohibit };
        private void LoadPaperSizePreset()
        {
            Logger.Trace("LoadPaperSizePreset");
            string fileName = Datapath.Papersize;
            if (!File.Exists(fileName))
            {
                Logger.Error("File 'papersize.xml' not found in {0}", fileName);
                return;
            }
            papersizePreset.Clear();
            CboxPapersize.Items.Clear();
            string tmp;
            try
            {
                XmlReader content = XmlReader.Create(fileName, settings);   // "papersize.xml");
                while (content.Read())
                {
                    if (!content.IsStartElement())
                        continue;

                    switch (content.Name)
                    {
                        case "papersize":
                            break;
                        case "preset":
                            if ((content["name"].Length > 0) && (content["min"] != null) && (content["max"] != null) && (content["unit"] != null))
                            {
                                tmp = content["name"] + "    " + content["min"] + " x " + content["max"] + "  " + content["unit"];
                                double.TryParse(content["min"], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double x);
                                double.TryParse(content["max"], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double y);
                                if (content["unit"] == "in") { x *= 2.54; y *= 2.54; }
                                if (content["unit"] == "px") { x *= 0.264583; y *= 0.264583; }
                                CboxPapersize.Items.Add(tmp);
                                papersizePreset.Add(new System.Windows.Point(x, y));
                            }
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                Logger.Error(err, "LoadPaperSizePreset {0} ", fileName);
                MessageBox.Show("Could not load / read papersize.xml.\r\n" + err.Message, "Error");
            }
            //	content.Dispose();
        }


    }
}
