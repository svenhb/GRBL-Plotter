/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2022 Sven Hasemann contact: svenhb@web.de

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

/*  Thanks to martin2250  https://github.com/martin2250/OpenCNCPilot for his HeightMap class
 * 2019-02-05 switch to global variables grbl.posWork
 * 2019-04-06 limit digits to 3, bugfix x3d export '.'-','
 * 2019-08-15 add logger
 * 2020-03-18 bug fix: abort btnLoad_Click - causes main GUI to load an empty map
 * 2021-04-30 after cancel, fill up missing coordinates line 561
 * 2021-07-14 code clean up / code quality
 * 2021-07-23 add notifier (by pushbullet or email)
 * 2021-12-15 InterpolateZ check index range
 * 2021-12-21 no save, if Map is null
 * 2021-12-22 check if is connected to grbl before sending code - 637
 * 2022-01-12 Except: Nullable object must have a value - Method:InterpolateZ
 * 2022-03-13 add extrudeX and Y option
 * 2022-04-25 line 1044 BtnPosFromCodeDimension_Click change to ((GuiVariables.variable["GMIX"] < Double.MaxValue) 
 * 2022-04-28 line 833 for (int iy = 0; iy < Map.SizeY; iy++)
 * 2022-11-07 line 1367 Nullable object must have a value -> check has value
 * 2022-11-15 line 1332 check if ((x < map.SizeX) && (y < map.SizeY))
 * 2022-11-29 SavePictureAsBMP: size=Map-size; importSTL; GetCoordinates round to 4 decimals
 * 2022-12-05 seperate class into new file "ControlHeightMapClass"
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace GrblPlotter
{
    public partial class ControlHeightMapForm : Form
    {
        private XyzPoint posProbe;
        internal StringBuilder scanCode;
        internal HeightMap Map;
        private List<Point> MapIndex;
        private Bitmap heightMapBMP = new Bitmap(10,10);
        private Bitmap heightMapCutH;
        private Bitmap heightMapCutV;
        private Bitmap heightLegendBMP;
        private bool isMapOk = false;
        internal bool mapIsLoaded = false;
        private int estimatedTime = 0;
        private bool notifierEnable = false;
        private bool notifierUpdateMarker = false;
        private bool notifierUpdateMarkerFinish = false;

        private string pathMap;
        private string pathExport;
		private string lastFileName = "";
        private bool refreshPictureBox = true;
        private bool diyControlConnected;
        internal bool scanStarted = false;
        internal bool extrudeX = false;
        internal bool extrudeY = false;
		
        private int BMPsizeX = 160;
        private int BMPsizeY = 160;
		
		private string pathLastSaved="";

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /******************************************************************
         * get height information from MainFormInterface case grblState.probe: line 285
         ******************************************************************/
        internal XyzPoint SetPosProbe
        {
            set
            {
                posProbe = value;
                double worldZ = posProbe.Z - (Grbl.posMachine.Z - Grbl.posWork.Z);

                if (scanStarted && (cntReceived < cntSent))
                {
                    if (extrudeX)           // scan Y axis, extrude in X
                    {
                        for (int extX = 0; extX < Map.SizeX; extX++)
                        {
                            Map.AddPoint(extX, MapIndex[cntReceived].Y, worldZ);
                        }
                        using (Graphics graph = Graphics.FromImage(heightMapBMP))
                        {
                            int x = BMPsizeX;
                            int y = BMPsizeY - (MapIndex[cntReceived].Y * BMPsizeY / (Map.SizeY - 1));
                            int dy = BMPsizeY / (Map.SizeY - 1);
                            Rectangle ImageSize = new Rectangle(0, y - dy / 2, x, dy);
                            SolidBrush myColor = new SolidBrush(GetColor(Map.MinHeight, Map.MaxHeight, worldZ, false));
                            graph.FillRectangle(myColor, ImageSize);
                            myColor.Dispose();
                        }
                    }
                    else if (extrudeY)      // scan X axis, extrude in Y
                    {
                        for (int extY = 0; extY < Map.SizeY; extY++)
                        {
                            Map.AddPoint(MapIndex[cntReceived].X, extY, worldZ);
                        }
                        using (Graphics graph = Graphics.FromImage(heightMapBMP))
                        {
                            int x = MapIndex[cntReceived].X * BMPsizeX / (Map.SizeX - 1);
                            int dx = BMPsizeX / (Map.SizeX - 1);
                            int y = BMPsizeY;
                            Rectangle ImageSize = new Rectangle(x - dx / 2, 0, dx, y);
                            SolidBrush myColor = new SolidBrush(GetColor(Map.MinHeight, Map.MaxHeight, worldZ, false));
                            graph.FillRectangle(myColor, ImageSize);
                            myColor.Dispose();
                        }
                    }
                    else
                    {
                        Map.AddPoint(MapIndex[cntReceived].X, MapIndex[cntReceived].Y, worldZ);
                        using (Graphics graph = Graphics.FromImage(heightMapBMP))
                        {
                            int x = MapIndex[cntReceived].X * BMPsizeX / (Map.SizeX - 1);
                            int dx = BMPsizeX / (Map.SizeX - 1);
                            int y = BMPsizeY - (MapIndex[cntReceived].Y * BMPsizeY / (Map.SizeY - 1));
                            int dy = BMPsizeY / (Map.SizeY - 1);
                            Rectangle ImageSize = new Rectangle(x - dx / 2, y - dy / 2, dx, dy);
                            SolidBrush myColor = new SolidBrush(GetColor(Map.MinHeight, Map.MaxHeight, worldZ, false));
                            graph.FillRectangle(myColor, ImageSize);
                            myColor.Dispose();
                        }
                    }
                    pictureBox1.Image = new Bitmap(heightMapBMP);
                    if (refreshPictureBox) pictureBox1.Refresh();
                    SetTextThreadSave(lblMin, string.Format("{0:0.000}", Map.MinHeight));
                    SetTextThreadSave(lblMid, string.Format("{0:0.000}", (Map.MinHeight + Map.MaxHeight) / 2));
                    SetTextThreadSave(lblMax, string.Format("{0:0.000}", Map.MaxHeight));

                    cntReceived++;
                    elapsed = DateTime.UtcNow - timeInit;
                    TimeSpan diff = elapsed.Subtract(elapsedOld);
                    string textNotifier, textPercent;

                    TextBoxAddThreadSave(textBox1, string.Format("x: {0:0.000} y: {1:0.00} z: {2:0.000}\r\n", Grbl.posWork.X, Grbl.posWork.Y, worldZ));

                    // during scan
                    if (refreshPictureBox && (diff.Milliseconds > 500))
                    {
                        // adapt estimated time
                        int elapsedSeconds = (int)elapsed.TotalSeconds;
                        if ((elapsedSeconds % 30) == 10)
                        {
                            if (elapsedSeconds > 10)
                            {
                                double newtime = elapsedSeconds * cntSent / cntReceived;
                                estimatedTime = (int)((3 * estimatedTime + newtime) / 4);
                            }
                        }

                        SetProgressValueThreadSave(progressBar1, cntReceived);
                        textPercent = string.Format("{0,4:0.0} %", (100 * (double)cntReceived / cntSent));
                        textNotifier = string.Format("{0} / {1}  {2}  estimated:{3}", cntReceived, progressBar1.Maximum, elapsed.ToString(@"hh\:mm\:ss"), GetTime(estimatedTime));
                        SetTextThreadSave(lblProgress, string.Format("{0} {1}", textPercent, textNotifier));
                        elapsedOld = elapsed;

                        if (notifierEnable && Properties.Settings.Default.notifierMessageProgressEnable)
                        {
                            if ((elapsedSeconds % (int)(60 * Properties.Settings.Default.notifierMessageProgressInterval)) == 5) // offset 5 sec. to get message at start
                            {
                                if (notifierUpdateMarker)
                                {
                                    notifierUpdateMarker = false;
                                    if (Properties.Settings.Default.notifierMessageProgressTitle)
                                        Notifier.SendMessage(textNotifier, textPercent);
                                    else
                                        Notifier.SendMessage(textNotifier);
                                }
                            }
                            else
                                notifierUpdateMarker = true;
                        }
                    }

                    // scan finished
                    if (cntReceived >= progressBar1.Maximum)
                    {
                        EnableControls(true);
                        scanStarted = false;
                        btnStartHeightScan.Text = "Generate Height Map";
                        lblProgress.Text = "Finish t=" + elapsed.ToString(@"hh\:mm\:ss");
                        lblProgress.BackColor = Color.Transparent;

                        ShowHightMapBMP();
                        isMapOk = true;
                        EnableControls(true);
                        progressBar1.Value = cntReceived;
                        SetProgressValueThreadSave(progressBar1, cntReceived);

                        textNotifier = string.Format("{0}% {1} / {2}  {3}  estimated:{4}", (100 * cntReceived / progressBar1.Maximum), cntReceived, progressBar1.Maximum, elapsed.ToString(@"hh\:mm\:ss"), GetTime(estimatedTime));
                        SetTextThreadSave(lblProgress, textNotifier);
                        TextBoxAddThreadSave(textBox1, string.Format("x: {0:0.000} y: {1:0.00} z: {2:0.000}\r\n", Grbl.posWork.X, Grbl.posWork.Y, worldZ));
                        ControlPowerSaving.EnableStandby();

                        if (notifierEnable && !notifierUpdateMarkerFinish)    // just notify once
                        {
                            notifierUpdateMarkerFinish = true;
                            string msg = string.Format("{0}\r\nDuration  : {1} (hh:mm:ss)\r\nTime stamp: {2}", "Heigth scan finished", elapsed.ToString(@"hh\:mm\:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            if (Properties.Settings.Default.notifierMessageProgressTitle)
                                Notifier.SendMessage(msg, "100 %");
                            else
                                Notifier.SendMessage(msg);
                        }
                    }
                }
            }
            get { return posProbe; }
        }

        internal bool DiyControlConnected
        {
            set
            {
                diyControlConnected = value;
                if (diyControlConnected)
                {
                    LblConnected.Text = "DIY Control connected";
                    LblConnected.BackColor = Color.Lime;
                }
                else
                {
                    LblConnected.Text = "DIY Control not connected";
                    LblConnected.BackColor = Color.Fuchsia;
                }
            }
            get { return diyControlConnected; }
        }

        public void SetBtnApply(bool active)
        {
            if (active)
            { btnApply.Text = "Apply Height Map"; }
            else
            { btnApply.Text = "Remove Height Map"; }
        }

        private void BtnOffset_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Map.SetZOffset(-Map.MaxHeight);
                ShowHightMapBMP();
            }
        }
        private void BtnOffsetZ_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Map.SetZOffset((double)nUDOffsetZ.Value);
                ShowHightMapBMP();
            }
        }

        private void BtnZoomZ_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Map.SetZZoom((double)nUDZoomZ.Value);
                ShowHightMapBMP();
            }
        }

        private void BtnInvertZ_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Map.SetZInvert();
                ShowHightMapBMP();
            }
        }

        private void BtnCutOffZ_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Map.SetZCutOff((double)nUDCutOffZ.Value);
                ShowHightMapBMP();
            }
        }


        private void CreateHightMapBMP()
        {
            bool interpolate = CbInterpolate.Checked;
            //    lblProgress.Text = "Finish t=" + elapsed.ToString(@"hh\:mm\:ss");
            //lblProgress.BackColor = Color.Transparent;
            progressBar1.Value = 0;
            if (Map != null)
            {
                double? z;
                int picWidth = pictureBox1.Width;
                int picHeight = pictureBox1.Height;
            //    Logger.Trace("CreateHightMapBMP  x:{0}  y:{1}", picWidth,picHeight);

                double stdX, stdY;
                int mapX, mapY;
                heightMapBMP = new Bitmap(picWidth, picHeight);
                for (int iy = 0; iy < picHeight; iy++)
                {
                    stdY = 1 - (double)iy / (picHeight+1);
                    mapY = (int)(Map.SizeY * stdY);
                    for (int ix = 0; ix < picWidth; ix++)
                    {
                        stdX= (double)ix / (picWidth+1);
                        mapX= (int)(Map.SizeX * stdX);
                        if(interpolate)
                            z = Map.GetPoint(((Map.SizeX-1) * stdX), ((Map.SizeY-1) * stdY));
                        else 
                            z = Map.GetPoint(mapX, mapY);
                        if (z!=null)
                            heightMapBMP.SetPixel(ix, iy, GetColor(Map.MinHeight, Map.MaxHeight, (double)z, isgray));
                    }
                }
            }
        }

        private void ShowHightMapBMP(bool updateLabel = true)
        {
            if (updateLabel)
            {
                lblProgress.Text = "Finish t=" + elapsed.ToString(@"hh\:mm\:ss");
                lblProgress.BackColor = Color.Transparent;
            }
            CreateHightMapBMP();

            pictureBox1.Image = new Bitmap(heightMapBMP);
            pictureBox1.Refresh();
            if (Map != null)
            { 
                lblMin.Text = string.Format("{0:0.000}", Map.MinHeight);
                lblMid.Text = string.Format("{0:0.000}", (Map.MinHeight + Map.MaxHeight) / 2);
                lblMax.Text = string.Format("{0:0.000}", Map.MaxHeight); 
            }
            pictureBox2.Image = new Bitmap(heightLegendBMP);
            pictureBox2.Refresh();
        }
		
		private void ShowHightMapCutH(int sizeZ, double pos)
		{
            if (Map != null)
            {
                bool interpolate = CbInterpolate.Checked;
                sizeZ = pictureBoxH.Height;
                int mx,picWidth = pictureBoxH.Width;

                heightMapCutH = new Bitmap(picWidth, sizeZ);
                int iy, iz;
                double? z;
                double stdX, stdY;
                double rangeZ = (Map.MaxHeight - Map.MinHeight);
                for (int ix = 0; ix < picWidth; ix++)
                {
                    iy = (int)(pos * Map.SizeY);
                    if (iy < 0) iy = 0;
                    if (iy >= Map.SizeY) iy = Map.SizeY - 1;

                    mx = ix * Map.SizeX/ picWidth;
                    if (mx < 0) mx = 0;
                    if (mx>= Map.SizeX) mx= Map.SizeX - 1;

                    stdX = (double)ix / picWidth;
                    if (interpolate)
                        z = Map.GetPoint(((Map.SizeX - 1) * stdX), ((Map.SizeY - 1) * pos));
                    else
                        z = Map.GetPoint(mx, iy);

                    if (z != null)
                    {
                        iz = (int)Math.Abs(((rangeZ - ((double)z - Map.MinHeight)) * sizeZ) / rangeZ);
                        if (iz < 0) iz = 0;
                        if (iz >= sizeZ) iz = sizeZ - 1;
                        heightMapCutH.SetPixel(ix, iz, Color.Black);
                    }
                }
                pictureBoxH.Image = new Bitmap(heightMapCutH);
                pictureBoxH.Refresh();
            }
		}
		private void ShowHightMapCutV(int sizeZ, double pos)
		{
            if (Map == null) return;
            bool interpolate = CbInterpolate.Checked;
            sizeZ = pictureBoxV.Width;
            int mx, picHeight = pictureBoxV.Height;

            heightMapCutV = new Bitmap(sizeZ, picHeight);
            int ix,iz;
            double? z;
            double stdX, stdY;
            double rangeZ = (Map.MaxHeight - Map.MinHeight);
			for (int iy = 0; iy < picHeight; iy++)
			{
                ix = (int)(pos * Map.SizeX);
                if (ix < 0) ix = 0;
                if (ix >= Map.SizeX) ix = Map.SizeX - 1;

                mx = iy * Map.SizeY / picHeight;
                if (mx < 0) mx = 0;
                if (mx >= Map.SizeY) mx = Map.SizeY - 1;

                stdY = (double)iy / picHeight;
                if (interpolate)
                    z = Map.GetPoint(((Map.SizeX - 1) * pos), ((Map.SizeY - 1) * stdY));
                else
                    z = Map.GetPoint(ix, mx);

                if (z != null)
                {
                    iz = (int)Math.Abs((((double)z - Map.MinHeight) * sizeZ) / rangeZ);
                    if (iz < 0) iz = 0;
                    if (iz >= sizeZ) iz = sizeZ - 1;
                    heightMapCutV.SetPixel(iz, iy, Color.Black);
                }
            }
            pictureBoxV.Image = new Bitmap(heightMapCutV);
            pictureBoxV.Refresh();
            pictureBoxV.Visible = true;
        }

        private bool isgray = false;
        private void CbGray_CheckedChanged(object sender, EventArgs e)
        {
            int legendHeight = heightLegendBMP.Height;
            isgray = cBGray.Checked;
            for (int i = 0; i < legendHeight; i++)
            {
                heightLegendBMP.SetPixel(0, (legendHeight - 1) - i, GetColor(0, legendHeight, i, isgray));
                heightLegendBMP.SetPixel(1, (legendHeight - 1) - i, GetColor(0, legendHeight, i, isgray));
            }
            pictureBox2.Image = new Bitmap(heightLegendBMP);
            pictureBox2.Refresh();
            if (Map != null)
                ShowHightMapBMP();
        }
        private static Color GetColor(double min, double max, double value, bool gray)
        {
            int R = 0, G = 0, B = 0;
            if (gray)
            {
                int valC = (int)(255 * (value - min) / (max - min));
                if (valC < 0) valC = 0;
                if (valC > 255) valC = 255;
                R = G = B = valC;
            }
            else
            {
                int segments = 3;
                int valC = (int)(255 * segments * (value - min) / (max - min));
                if (valC < 0) valC = 0;
                if (valC > 255 * segments) valC = 255 * segments;

                if ((valC >= 0) && (valC < 256 * 1))
                { R = 0; G = valC; B = 255 - valC; }

                else if ((valC >= 256) && (valC < 256 * 2))
                { R = valC - (256 * 1); G = 255; B = 0; }

                else if ((valC >= 256 * 2) && (valC < 256 * 3))
                { R = 255; G = (256 * 3 - 1) - valC; B = 0; }
            }
            return Color.FromArgb(R, G, B);
        }

        private void EnableControls(bool enable)
        {
            nUDX1.Enabled = enable; nUDX2.Enabled = enable;
            nUDY1.Enabled = enable; nUDY2.Enabled = enable;
            nUDDeltaX.Enabled = enable; nUDDeltaY.Enabled = enable;
            nUDGridX.Enabled = enable; nUDGridY.Enabled = enable;
            btnPosLL.Enabled = enable; btnPosUR.Enabled = enable;
            nUDProbeDown.Enabled = enable;
            nUDProbeUp.Enabled = enable;
            nUDProbeSpeed.Enabled = enable;
            btnOffset.Enabled = enable;
            cBGray.Enabled = enable;
            btnApply.Enabled = enable && isMapOk;
            menuStrip1.Enabled = enable;
            gB_Manipulation.Enabled = enable;
            CbExtrudeEnable.Enabled = enable;
            RbScanX.Enabled = enable;
            RbScanY.Enabled = enable;
            GbProbing.Enabled = enable;
        }

        public StringBuilder GetCode
        { get { return scanCode; } }

        public ControlHeightMapForm()
        {
            Logger.Trace("++++++ ControlHeightMapForm START ++++++");
            this.Icon = Properties.Resources.Icon;
            InitializeComponent();
        }

        int cntReceived = 0, cntSent = 0;

        #region controls
		
        private void BtnPosLL_Click(object sender, EventArgs e)
        {
            nUDX1.Value = (decimal)Math.Round(Grbl.posWork.X, 2);
            nUDY1.Value = (decimal)Math.Round(Grbl.posWork.Y, 2);
            nUDX2.Value = nUDDeltaX.Value + nUDX1.Value;
            nUDY2.Value = nUDDeltaY.Value + nUDY1.Value;
        }

        private void BtnPosUR_Click(object sender, EventArgs e)
        {
            nUDX2.Value = (decimal)Math.Round(Grbl.posWork.X, 2);
            nUDY2.Value = (decimal)Math.Round(Grbl.posWork.Y, 2);
            decimal tmpX = nUDX2.Value - nUDX1.Value;
            decimal tmpY = nUDY2.Value - nUDY1.Value;
            nUDDeltaX.Value = (tmpX > nUDDeltaX.Minimum) ? tmpX : nUDDeltaX.Minimum;
            nUDDeltaY.Value = (tmpY > nUDDeltaY.Minimum) ? tmpY : nUDDeltaY.Minimum;
        }
        private void NudDeltaX_ValueChanged(object sender, EventArgs e)
        {
            nUDX2.Value = nUDDeltaX.Value + nUDX1.Value;
            nUDY2.Value = nUDDeltaY.Value + nUDY1.Value;
        }
        private void NudX1_ValueChanged(object sender, EventArgs e)
        {
            nUDX2.Value = nUDDeltaX.Value + nUDX1.Value;
            nUDY2.Value = nUDDeltaY.Value + nUDY1.Value;
        }
        private void NudX2_ValueChanged(object sender, EventArgs e)
        {
            decimal tmpX = nUDX2.Value - nUDX1.Value;
            decimal tmpY = nUDY2.Value - nUDY1.Value;
            nUDDeltaX.Value = (tmpX > nUDDeltaX.Minimum) ? tmpX : nUDDeltaX.Minimum;
            nUDDeltaY.Value = (tmpY > nUDDeltaY.Minimum) ? tmpY : nUDDeltaY.Minimum;
        }


        private void ControlHeightMapForm_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.locationImageForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }

            //_event = new eventArgsTemplates();
            decimal tmp = Properties.Settings.Default.heightMapX2 - Properties.Settings.Default.heightMapX1;
            if (tmp <= 0) tmp = 1;
            nUDDeltaX.Value = tmp;

            tmp = Properties.Settings.Default.heightMapY2 - Properties.Settings.Default.heightMapY1;
            if (tmp <= 0) tmp = 1;
            nUDDeltaY.Value = tmp;

            ControlHeightMapForm_SizeChanged(sender, e);
            int stepX = (int)Math.Round((nUDX2.Value - nUDX1.Value) / nUDGridX.Value);
            int stepY = (int)Math.Round((nUDY2.Value - nUDY1.Value) / nUDGridY.Value);
            lblXDim.Text = string.Format("X Min:{0} Max:{1} Step:{2}", nUDX1.Value, nUDX2.Value, stepX);
            lblYDim.Text = string.Format("Y Min:{0} Max:{1} Step:{2}", nUDY1.Value, nUDY2.Value, stepY);
            pathMap = Datapath.Examples;
            RbProbingZ.Checked = Properties.Settings.Default.heightMapProbeUseZ; ;
            RbProbingDiy.Checked = !Properties.Settings.Default.heightMapProbeUseZ;
            RbScanX.Checked = Properties.Settings.Default.heightMapExtrudeScanX;
            RbScanY.Checked = !Properties.Settings.Default.heightMapExtrudeScanX;
            RbProbingZ_CheckedChanged(sender, e);
        }

        private TimeSpan elapsed, elapsedOld;   //elapsed time from file burnin
        private DateTime timeInit;              //time start to burning file

        // applyHeightMap;						// in MainFormGetCodeTransform
        private void BtnApply_Click(object sender, EventArgs e)
        { }
        #endregion

        private void ControlHeightMapForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("++++++ ControlHeightMapForm Stop ++++++");
            Properties.Settings.Default.heightMapX2 = nUDX2.Value;
            Properties.Settings.Default.heightMapY2 = nUDY2.Value;
            Properties.Settings.Default.locationImageForm = Location;
            Properties.Settings.Default.heightMapProbeUseZ = RbProbingZ.Checked;
            Properties.Settings.Default.heightMapExtrudeScanX = RbScanX.Checked;
            Properties.Settings.Default.Save();
        }

        /******************************************************************
         * Send position to GUI
         ******************************************************************/
        private void PictureBox1_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (!scanStarted))
            {
                double relposX = Map.Delta.X * (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).X) / pictureBox1.Width);
                double relposY = Map.Delta.Y - Map.Delta.Y * (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).Y) / pictureBox1.Height);
                double posX = Map.Min.X + relposX;
                double posY = Map.Min.Y + relposY;
				if (Grbl.isConnected)
                {	DialogResult result;
					result = MessageBox.Show("Move to this position? " + posX.ToString() + " ; " + posY.ToString(), "Attention", MessageBoxButtons.YesNo);
					if (result == System.Windows.Forms.DialogResult.Yes)
					{
						if ((decimal)Grbl.posWork.Z < nUDProbeUp.Value)
							OnRaiseXyzEvent(new XyzEventArgs(null, null, (double)nUDProbeUp.Value, "G90"));
						OnRaiseXyzEvent(new XyzEventArgs(posX, posY, "G90"));   // move relative and fast
					}
				}
            }
        }
        public event EventHandler<XyzEventArgs> RaiseXyzEvent;
        // OnRaisePositionClickEvent;				// in MainForm
        protected virtual void OnRaiseXyzEvent(XyzEventArgs e)
        {
            RaiseXyzEvent?.Invoke(this, e);
        }

        private readonly ToolTip tt = new ToolTip();
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Map != null)
            {
                double relX = (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).X) / pictureBox1.Width);
                double relY = 1 - (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).Y) / pictureBox1.Height);
                double relposX = Map.Delta.X * relX;
                double relposY = Map.Delta.Y * relY;
                double posX = Map.Min.X + relposX;
                double posY = Map.Min.Y + relposY;

                double iX = (relX * Map.SizeX);
                if (iX < 0) iX = 0;
                if (iX >= Map.SizeX) iX = Map.SizeX - 1;

                double iY = (relY * Map.SizeY);
                if (iY < 0) iY = 0;
                if (iY >= Map.SizeY) iY = Map.SizeY - 1;

                double? posZ;
                if (CbInterpolate.Checked)
                {
                    posZ = Map.GetPoint(iX, iY);
                    tt.SetToolTip(this.pictureBox1, string.Format("Index X:{0:0.00} Y:{1:0.00}  \r\nPos.   X:{2:0.00} Y:{3:0.00}  \r\nZ:{4:0.00}", iX, iY, posX, posY, posZ));
                }
                else
                {
                    posZ = Map.GetPoint((int)iX, (int)iY);
                    tt.SetToolTip(this.pictureBox1, string.Format("Index X:{0}    Y:{1}  \r\nPos.   X:{2:0.00} Y:{3:0.00}  \r\nZ:{4:0.00}", (int)iX, (int)iY, posX, posY, posZ));
                }
				ShowHightMapCutH(pictureBoxH.Height, relY);
				ShowHightMapCutV(pictureBoxV.Width, relX);				
            }
        }
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
        //    CreateHightMap(240);
            pictureBoxV.Visible = false;
        }


        /************************************************************************
        Generate GCode for probing
        getGCodeScanHeightMap;      // in MainFormGetCodeTransform
        ************************************************************************/
        private void BtnStartHeightScan_Click(object sender, EventArgs e)
        {
            EnableControls(scanStarted);
            if (!scanStarted)
            {
                if (!Grbl.isConnected)
                {
                    MessageBox.Show(Localization.GetString("grblNotConnected"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Logger.Info("Generate Height scan Code");
                refreshPictureBox = true;
                isMapOk = false;
                timeInit = DateTime.UtcNow;
                elapsed = TimeSpan.Zero;
                elapsedOld = elapsed;
                btnStartHeightScan.Text = "STOP scanning Height Map";
                decimal x1, x2, y1, y2;
                x1 = Math.Round(Math.Min(nUDX1.Value, nUDX2.Value), 2);
                x2 = Math.Round(Math.Max(nUDX1.Value, nUDX2.Value), 2);
                y1 = Math.Round(Math.Min(nUDY1.Value, nUDY2.Value), 2);
                y2 = Math.Round(Math.Max(nUDY1.Value, nUDY2.Value), 2);
                if (x1 == x2) x2 = x1 + 10;
                if (y1 == y2) y2 = y1 + 10;
                nUDX1.Value = x1; nUDX2.Value = x2; nUDY1.Value = y1; nUDY2.Value = y2;
                //decimal stepX = (x2 - x1) / (nUDGridX.Value - 1);
                //decimal stepY = (y2 - y1) / (nUDGridY.Value - 1);
                cntSent = 0; cntReceived = 0;
                Gcode.ReduceGCode = true;   // reduce number format to #.# in gcode.frmtNum()

                Logger.Trace("BtnStartHeightScan x1:{0} Y1:{1}  x2:{2} y2:{3}", x1, y1, x2, y2);
                Map = new HeightMap((double)nUDGridX.Value, new Vector2((double)x1, (double)y1), new Vector2((double)x2, (double)y2));
                MapIndex = new List<Point>();

                lblXDim.Text = string.Format("X Min:{0} Max:{1} Step:{2}", Map.Min.X, Map.Max.X, Map.SizeX);
                lblYDim.Text = string.Format("Y Min:{0} Max:{1} Step:{2}", Map.Min.Y, Map.Max.Y, Map.SizeY);

				lastFileName = string.Format("New Map Min-X:{0} Y:{1} Max-X:{2} Y:{3}  Grid-X:{4} Y:{5}", Map.Min.X, Map.Min.Y, Map.Max.X, Map.Max.Y, Map.SizeX, Map.SizeY);	
                CalculateEstimatedTime();

                notifierEnable = ((double)Properties.Settings.Default.notifierMessageProgressInterval < (estimatedTime / 60));
                notifierUpdateMarker = true;
                notifierUpdateMarkerFinish = false;

                textBox1.Clear();
                textBox1.Text += string.Format("(Probing Size X:{0} Y:{1} )", Map.SizeX, Map.SizeY);
                int pixX, pixY;
                BMPsizeX = pictureBox1.Width;// 240;
                BMPsizeY = Map.SizeY * BMPsizeX / Map.SizeX;
                heightMapBMP = new Bitmap(BMPsizeX, BMPsizeY);
                using (Graphics graph = Graphics.FromImage(heightMapBMP))
                {
                    Rectangle ImageSize = new Rectangle(0, 0, BMPsizeX, BMPsizeY);
                    graph.FillRectangle(Brushes.White, ImageSize);
                }
                Vector2 tmp;

                scanCode = new StringBuilder();
                scanCode.AppendFormat("G90F{0}\r\n", Gcode.FrmtNum((float)nUDProbeSpeed.Value));

                bool useZ = RbProbingZ.Checked; //(nUDProbeDown.Value != 0);
                extrudeX = CbExtrudeEnable.Checked && !RbScanX.Checked;    // scan Y axis, extrude in X
                extrudeY = CbExtrudeEnable.Checked && RbScanX.Checked;     // scan X axis, extrude in Y
                bool noExtrude = !CbExtrudeEnable.Checked;

                if (noExtrude)
                {
                    for (int iy = 0; iy < Map.SizeY; iy++)
                    {
                        tmp = Map.GetCoordinates(0, iy);
                        if (useZ) scanCode.AppendFormat("G0Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeUp.Value));
                        if (CbUseG1.Checked)
                        { scanCode.AppendFormat("G1Y{0}F{1}\r\n", Gcode.FrmtNum((float)tmp.Y), NudXYFeedrate.Value); }
                        else
                        { scanCode.AppendFormat("G0Y{0}\r\n", Gcode.FrmtNum((float)tmp.Y)); }

                        pixY = iy * BMPsizeY / Map.SizeY;
                        for (int ix = 0; ix < Map.SizeX; ix++)
                        {
                            pixX = ix * BMPsizeX / Map.SizeX;
                            heightMapBMP.SetPixel(pixX, pixY, Color.FromArgb(255, 00, 00));
                            tmp = Map.GetCoordinates(ix, iy);
                            MapIndex.Add(new Point(ix, iy));

                            if (useZ) scanCode.AppendFormat("G0Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeUp.Value));  // save Z pos

                            if (CbUseG1.Checked)
                            { scanCode.AppendFormat("G1X{0}F{1}\r\n", Gcode.FrmtNum((float)tmp.X), NudXYFeedrate.Value); }
                            else
                            { scanCode.AppendFormat("G0X{0}\r\n", Gcode.FrmtNum((float)tmp.X)); }

                            if (!useZ)
                            { scanCode.AppendFormat("($PROBE)\r\n"); }
                            else
                            { scanCode.AppendFormat("G38.3Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeDown.Value)); }

                            cntSent++;
                        }
                        if (iy < Map.SizeY - 1)
                        {
                            iy++;
                            tmp = Map.GetCoordinates(0, iy);       //?

                            if (useZ) scanCode.AppendFormat("G0Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeUp.Value));
                            if (CbUseG1.Checked)
                            { scanCode.AppendFormat("G1Y{0}F{1}\r\n", Gcode.FrmtNum((float)tmp.Y), NudXYFeedrate.Value); }
                            else
                            { scanCode.AppendFormat("G0Y{0}\r\n", Gcode.FrmtNum((float)tmp.Y)); }

                            for (int ix = Map.SizeX - 1; ix >= 0; ix--)
                            {
                                pixX = ix * BMPsizeX / Map.SizeX;
                                heightMapBMP.SetPixel(pixX, pixY, Color.FromArgb(100, 100, 100));
                                tmp = Map.GetCoordinates(ix, iy);
                                MapIndex.Add(new Point(ix, iy));

                                if (useZ) scanCode.AppendFormat("G0Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeUp.Value));

                                if (CbUseG1.Checked)
                                { scanCode.AppendFormat("G1X{0}F{1}\r\n", Gcode.FrmtNum((float)tmp.X), NudXYFeedrate.Value); }
                                else
                                { scanCode.AppendFormat("G0X{0}\r\n", Gcode.FrmtNum((float)tmp.X)); }

                                if (!useZ)
                                { scanCode.AppendFormat("($PROBE)\r\n"); }
                                else
                                { scanCode.AppendFormat("G38.3Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeDown.Value)); }
                                cntSent++;
                            }

                        }
                    }
                }
                else if (extrudeX)  // scan Y axis, extrude in X
                {
                    textBox1.Text += "ExtrudeX scan Y axis, extrude in X";
                    int ix = 0; //ix < Map.SizeX; ix++)
                    tmp = Map.GetCoordinates(ix, 0);
                    if (useZ) scanCode.AppendFormat("G0Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeUp.Value));
                    if (CbUseG1.Checked)
                    { scanCode.AppendFormat("G1X{0}F{1}\r\n", Gcode.FrmtNum((float)tmp.X), NudXYFeedrate.Value); }
                    else
                    { scanCode.AppendFormat("G0X{0}\r\n", Gcode.FrmtNum((float)tmp.X)); }

                    for (int iy = 0; iy < Map.SizeY; iy++)	// fixed 2022-04-28
                    {
                        pixX = ix * BMPsizeX / Map.SizeX;
                        pixY = iy * BMPsizeY / Map.SizeY;
                        heightMapBMP.SetPixel(pixX, pixY, Color.FromArgb(255, 00, 00));
                        tmp = Map.GetCoordinates(ix, iy);
                        MapIndex.Add(new Point(ix, iy));

                        if (useZ) scanCode.AppendFormat("G0Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeUp.Value));  // save Z pos

                        if (CbUseG1.Checked)
                        { scanCode.AppendFormat("G1Y{0}F{1}\r\n", Gcode.FrmtNum((float)tmp.Y), NudXYFeedrate.Value); }
                        else
                        { scanCode.AppendFormat("G0Y{0}\r\n", Gcode.FrmtNum((float)tmp.Y)); }

                        if (!useZ)
                        { scanCode.AppendFormat("($PROBE)\r\n"); }
                        else
                        { scanCode.AppendFormat("G38.3Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeDown.Value)); }

                        cntSent++;
                    }

                }
                else if (extrudeY)  // scan X axis, extrude in Y
                {
                    textBox1.Text += "ExtrudeY scan X axis, extrude in Y";
                    int iy = 0; //iy < Map.SizeY; iy++)
                    tmp = Map.GetCoordinates(0, iy);
                    if (useZ) scanCode.AppendFormat("G0Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeUp.Value));
                    if (CbUseG1.Checked)
                    { scanCode.AppendFormat("G1Y{0}F{1}\r\n", Gcode.FrmtNum((float)tmp.Y), NudXYFeedrate.Value); }
                    else
                    { scanCode.AppendFormat("G0Y{0}\r\n", Gcode.FrmtNum((float)tmp.Y)); }

                    pixY = iy * BMPsizeY / Map.SizeY;
                    for (int ix = 0; ix < Map.SizeX; ix++)
                    {
                        pixX = ix * BMPsizeX / Map.SizeX;
                        heightMapBMP.SetPixel(pixX, pixY, Color.FromArgb(255, 00, 00));
                        tmp = Map.GetCoordinates(ix, iy);
                        MapIndex.Add(new Point(ix, iy));

                        if (useZ) scanCode.AppendFormat("G0Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeUp.Value));  // save Z pos

                        if (CbUseG1.Checked)
                        { scanCode.AppendFormat("G1X{0}F{1}\r\n", Gcode.FrmtNum((float)tmp.X), NudXYFeedrate.Value); }
                        else
                        { scanCode.AppendFormat("G0X{0}\r\n", Gcode.FrmtNum((float)tmp.X)); }

                        if (!useZ)
                        { scanCode.AppendFormat("($PROBE)\r\n"); }
                        else
                        { scanCode.AppendFormat("G38.3Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeDown.Value)); }

                        cntSent++;
                    }
                }

                if (useZ) scanCode.AppendFormat("G0 Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeUp.Value));
                tmp = Map.GetCoordinates(0, 0);
                scanCode.AppendFormat("G0 X{0} Y{1}\r\n", Gcode.FrmtNum((float)tmp.X), Gcode.FrmtNum((float)tmp.Y));
                scanCode.AppendLine("M30");     // finish

                textBox1.Text += "\r\nCode sent\r\n" + scanCode.ToString() + "\r\n##########################\r\n";
                progressBar1.Maximum = cntSent;
                lblProgress.Text = string.Format("{0}%  estimated time:{1}", (100 * cntReceived / progressBar1.Maximum), GetTime(estimatedTime));
                lblProgress.BackColor = Color.Transparent;

                pictureBox1.Image = new Bitmap(heightMapBMP);
                pictureBox1.Refresh();
                ControlPowerSaving.SuppressStandby();
                scanStarted = true;
            }
            else
            {   // scan was started, now interruption
                btnStartHeightScan.Text = "Generate Height Map";
                refreshPictureBox = false;
                notifierEnable = false;
                ControlPowerSaving.EnableStandby();
                if ((Map != null) && (cntReceived < cntSent))   // fill missing coordiantes
                {

                    double worldZ = 0 - (Grbl.posMachine.Z - Grbl.posWork.Z);
                    while (cntReceived < cntSent)
                    {
                        if (extrudeX)           // scan Y axis, extrude in X
                        {
                            for (int extX = 0; extX < Map.SizeX; extX++)
                            {
                                Map.AddPoint(extX, MapIndex[cntReceived].Y, worldZ);
                            }
                        }
                        else if (extrudeY)      // scan X axis, extrude in Y
                        {
                            for (int extY = 0; extY < Map.SizeY; extY++)
                            {
                                Map.AddPoint(MapIndex[cntReceived].X, extY, worldZ);
                            }
                        }
                        else
                            Map.AddPoint(MapIndex[cntReceived].X, MapIndex[cntReceived].Y, worldZ);

                        cntReceived++;
                        //                  SetPosProbe = new XyzPoint(0, 0, 0);
                    }
                    refreshPictureBox = true;
                }
                scanStarted = false;
            }
        }
        private string GetTime(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return time.ToString(@"hh\:mm\:ss");
        }

        private void CalculateEstimatedTime()
        {
            double speed = (Grbl.GetSetting(110) > 500) ? Grbl.GetSetting(110) : 500;
            speed = CbUseG1.Checked ? (double)NudXYFeedrate.Value : speed;
            speed /= 60;    // given in mm/min but we need mm/sec

            double accel = (Grbl.GetSetting(120) > 100) ? Grbl.GetSetting(120) : 100;

            double smax = (double)nUDGridX.Value;
            double s = (speed * speed) / (2 * accel);       // distance until given speed is reached
            double t = Math.Sqrt(2 * s / accel);
            if (s <= 2 * smax)              // enough space to get full speed?
            {
                Logger.Info("calculateEstimatedTime1 speed:{0}  acc:{1}  smax:{2}  s:{3}  ta:{4}", speed, accel, smax, s, t);
                smax -= 2 * s;
                t += smax / speed;
            }
            else                            // full speed never reached
            {
                t = 2 * Math.Sqrt(smax / accel);
                Logger.Info("calculateEstimatedTime2 speed:{0}  acc:{1}  smax:{2}  s:{3}  t:{4}", speed, accel, smax, s, t);
            }

            estimatedTime = Map.SizeX * Map.SizeY * Grbl.pollInterval / 1000;	// pollinterval in ms
            estimatedTime += (int)(Map.SizeX * Map.SizeY * t);

            Logger.Info("calculateEstimatedTime  {0}  {1}  ", estimatedTime, GetTime(estimatedTime));
        }


        private void RbProbingZ_CheckedChanged(object sender, EventArgs e)
        {
            GbProbingZ.Enabled = RbProbingZ.Checked;
            GbProbingDiy.Enabled = RbProbingDiy.Checked;
            Properties.Settings.Default.heightMapProbeUseZ = RbProbingZ.Checked;
            Properties.Settings.Default.heightMapExtrudeScanX = RbScanX.Checked;
            Properties.Settings.Default.Save();
        }

        private void BtnMoveLL_Click(object sender, EventArgs e)
        {

        }

        private void BtnMoveUR_Click(object sender, EventArgs e)
        {

        }

        private void BtnPosFromCodeDimension_Click(object sender, EventArgs e)
        {
            if ((GuiVariables.variable["GMIX"] < Double.MaxValue) && (GuiVariables.variable["GMAX"] > Double.MinValue))		//(GuiVariables.variable["GMIX"] != GuiVariables.variable["GMAX"])
            {
                decimal min = (decimal)Math.Round((CbRoundUp.Checked ? Math.Floor(GuiVariables.variable["GMIX"]) : GuiVariables.variable["GMIX"]),2);
                decimal max = (decimal)Math.Round((CbRoundUp.Checked ? Math.Ceiling(GuiVariables.variable["GMAX"]) : GuiVariables.variable["GMAX"]),2);
                if (min >= nUDX1.Minimum)
                    nUDX1.Value = min;
                if (max <= nUDX2.Maximum)
                    nUDX2.Value = max;
            }
            else { MessageBox.Show("No graphic loaded?"); }

            if ((GuiVariables.variable["GMIY"] < Double.MaxValue) && (GuiVariables.variable["GMAY"] > Double.MinValue))		//(GuiVariables.variable["GMIY"] != GuiVariables.variable["GMAY"])
            {
                decimal min = (decimal)Math.Round((CbRoundUp.Checked ? Math.Floor(GuiVariables.variable["GMIY"]) : GuiVariables.variable["GMIY"]),2);
                decimal max = (decimal)Math.Round((CbRoundUp.Checked ? Math.Ceiling(GuiVariables.variable["GMAY"]) : GuiVariables.variable["GMAY"]),2);
                if (min >= nUDY1.Minimum)
                    nUDY1.Value = min;
                if (max <= nUDY2.Maximum)
                    nUDY2.Value = max;
            }
        }

        public void StopScan()
        {
            scanStarted = false;
            btnStartHeightScan.Text = "Generate Height Map";
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            EnableControls(true);
        }

        private void SetTextThreadSave(Label label, string txt)
        {
            if (label.InvokeRequired)
            { label.BeginInvoke((MethodInvoker)delegate () { label.Text = txt; }); }
            else
            { label.Text = txt; }
        }

        private void ControlHeightMapForm_SizeChanged(object sender, EventArgs e)
        {
			if ((this.WindowState == FormWindowState.Minimized) || (Width == 0) || (Height == 0))
				return;
			
            groupBox3.Width = Width - (620 - 270);
            groupBox3.Height=Height - (375 - 306);
            int left = groupBox3.Width - (270 - 206);
            int top = groupBox3.Height - (306 - 234);


            int origWidth = pictureBox1.Width = pictureBoxH.Width = left - 6;
            int origHeight = pictureBox1.Height = pictureBoxV.Height = top - 34;

            if (Map != null)
            {
                double ratio = (double)Map.SizeX / Map.SizeY;
                int newWidth = (int)(origHeight * ratio);
                int newHeight = (int)(origWidth / ratio);
            //    Logger.Trace("ControlHeightMapForm_SizeChanged  ratio:{0:0.000} newx:{1}  y:{2}",ratio,newWidth,newHeight);
                if (ratio >= 1)
                {
                    if (newHeight < origHeight)
                    { pictureBox1.Height = pictureBoxV.Height = newHeight; top = newHeight + 34; }
                    else
                    { pictureBox1.Width = pictureBoxH.Width = newWidth; left = newWidth + 6; }
                }
                else
                {
                    if (newWidth < origWidth)
                    { pictureBox1.Width = pictureBoxH.Width = newWidth; left = newWidth + 6; }
                    else
                    { pictureBox1.Height = pictureBoxV.Height = newHeight; top = newHeight + 34; }
                }
            }

            pictureBoxV.Left = left + 15;
            pictureBoxH.Top = top + 2;

            pictureBox2.Left = left + 2;
            pictureBox2.Height = pictureBox1.Height;
            lblMax.Left = pictureBox2.Left + 12;
            lblMid.Left = pictureBox2.Left + 12;
            lblMin.Left = pictureBox2.Left + 12;
            lblMid.Top = lblMax.Top + pictureBox1.Height / 2 -7;
            lblMin.Top = lblMax.Top + pictureBox1.Height -14;

            lblXDim.Top = groupBox3.Height - 28;
            lblYDim.Top = groupBox3.Height - 16;

            BMPsizeX = pictureBox1.Width;// 240;
            BMPsizeY = pictureBox1.Height;
            if (Map!=null)
                BMPsizeY = Map.SizeY * BMPsizeX / Map.SizeX;

            int legendHeight = pictureBox2.Height;
            heightLegendBMP = new Bitmap(2, legendHeight);
            for (int i = 0; i < legendHeight; i++)
            {
                heightLegendBMP.SetPixel(0, (legendHeight - 1) - i, GetColor(0, legendHeight, i, isgray));
                heightLegendBMP.SetPixel(1, (legendHeight - 1) - i, GetColor(0, legendHeight, i, isgray));
            }

            ShowHightMapBMP(false);    // and legend
        }

        private void CbInterpolate_CheckedChanged(object sender, EventArgs e)
        {
            ShowHightMapBMP();
        }


        private void TextBoxAddThreadSave(TextBox box, string txt)
        {
            if (box.InvokeRequired)
            { box.BeginInvoke((MethodInvoker)delegate () { box.AppendText(txt); }); }
            else
            { box.AppendText(txt); box.ScrollToCaret(); }
        }

        private void SetProgressValueThreadSave(ProgressBar bar, int val)
        {
            if (bar.InvokeRequired)
            { bar.BeginInvoke((MethodInvoker)delegate () { bar.Value = val; }); }
            else
            { bar.Value = val; }
        }

        /************************************************************************
        Generate GCode from probing data
        getGCodeFromHeightMap;      			// in MainFormGetCodeTransform
        ************************************************************************/
        private void BtnGCode_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Vector2 tmp;
                //          double z;
                float gcodeXYFeed = (float)Properties.Settings.Default.importGCXYFeed;
                float gcodeZFeed = (float)Properties.Settings.Default.importGCZFeed;
                float gcodeZUp = (float)Properties.Settings.Default.importGCZUp;

				float distanceX = (float)(Map.Max.X - Map.Min.X);
				float distanceY = (float)(Map.Max.Y - Map.Min.Y);
				float gcodeDistance=0; 

                scanCode = new StringBuilder();
                StringBuilder tmpCode = new StringBuilder();
                tmp = Map.GetCoordinates(0, 0);

                double saveZ = Math.Max(gcodeZUp, gcodeZUp + Map.MaxHeight);
                tmpCode.AppendFormat("G90 G0 F{0} Z{1}\r\n", Gcode.FrmtNum(gcodeZFeed), Gcode.FrmtNum(saveZ));
                tmpCode.AppendFormat("G0 X{0} Y{1}\r\n", Gcode.FrmtNum((float)tmp.X), Gcode.FrmtNum((float)tmp.Y));
                tmpCode.AppendFormat("G1 F{0}\r\n", Gcode.FrmtNum(gcodeXYFeed));
				int gcodeLines = 6;

				if (!CbNewStepWidth.Checked)
                {	for (int iy = 0; iy < Map.SizeY; iy++)
					{
						for (int ix = 0; ix < Map.SizeX; ix++)
						{
							MoveXYZ(tmpCode, ix, iy); gcodeLines++;
						}
						gcodeDistance += distanceX;
						if (iy < Map.SizeY - 1)
						{
							iy++;
							for (int ix = Map.SizeX - 1; ix >= 0; ix--)
							{
								MoveXYZ(tmpCode, ix, iy); gcodeLines++;
							}
							gcodeDistance += distanceX;
						}
					}
				}
				else
				{	
					if (CbMoveXY.Checked)
					{
						double stepX,stepY;
						stepX = stepY = (double)NudNewStepWidth.Value;

						for (double ry = Map.Min.Y; ry <= Map.Max.Y; ry+=stepY)
						{
							for (double rx = Map.Min.X; rx <= Map.Max.X; rx+=stepX)
							{
								MoveXYZ(tmpCode, rx, ry); gcodeLines++;
							}
                            MoveXYZ(tmpCode, Map.Max.X, ry); gcodeLines++;
                            gcodeDistance += distanceX + (float)stepY; 
							if (ry < Map.Max.Y)
							{
								ry+=stepY;
								for (double rx = Map.Max.X; rx >= Map.Min.X; rx-=stepX)
								{
									MoveXYZ(tmpCode, rx, ry); gcodeLines++;
								}
                                MoveXYZ(tmpCode, Map.Min.X, ry); gcodeLines++;
                                gcodeDistance += distanceX + (float)stepY; 
							}
						}
					}
					else
					{
						double stepX,stepY;
						stepX = stepY = (double)NudNewStepWidth.Value;

						for (double rx = Map.Min.X; rx <= Map.Max.X; rx+=stepX)
						{
							for (double ry = Map.Min.Y; ry <= Map.Max.Y; ry+=stepY)
							{
								MoveXYZ(tmpCode, rx, ry); gcodeLines++;
							}
                            MoveXYZ(tmpCode, rx, Map.Max.Y); gcodeLines++;
                            gcodeDistance += distanceY + (float)stepX; 
							if (rx < Map.Max.X)
							{
								rx+=stepX;
								for (double ry = Map.Max.Y; ry >= Map.Min.Y; ry-=stepY)
								{
									MoveXYZ(tmpCode, rx, ry); gcodeLines++;
								}
                                MoveXYZ(tmpCode, rx, Map.Min.Y); gcodeLines++;
                                gcodeDistance += distanceY + (float)stepX; 
							}
						}						
					}
				}
				
				Gcode.SetHeaderInfo(lastFileName, gcodeDistance, gcodeXYFeed, gcodeLines, 1);

                tmpCode.AppendFormat("G0 Z{0}\r\n", Gcode.FrmtNum(saveZ));
                tmp = Map.GetCoordinates(0, 0);
                tmpCode.AppendFormat("G0 X{0} Y{1}\r\n", Gcode.FrmtNum((float)tmp.X), Gcode.FrmtNum((float)tmp.Y));

                scanCode.AppendFormat("{0}", Gcode.GetHeader("Height Map", ""));
                scanCode.Append(tmpCode);
                scanCode.AppendFormat("{0}", Gcode.GetFooter());
            }
        }
        private void MoveXYZ(StringBuilder tmpCode, int ix, int iy)				// expect index
        {
            Vector2 tmp = Map.GetCoordinates(ix, iy);
            double z = 0;
            double? zn = Map.GetPoint(ix, iy);

            if (zn == null) Logger.Error("MoveXYZ z=null ix:{0}  iy:{1}", ix, iy);
            if (zn != null) z = (double)zn;

            tmpCode.AppendFormat("X{0} Y{1} Z{2}\r\n", Gcode.FrmtNum((float)tmp.X), Gcode.FrmtNum((float)tmp.Y), Gcode.FrmtNum((float)z));
        }
        private void MoveXYZ(StringBuilder tmpCode, double rx, double ry)		// expect real coordinate
        {
            double? zn = Map.InterpolateZ(rx, ry);
            double z = (double)zn;
            if (zn == null) Logger.Error("MoveXYZ z=null rx:{0}  ry:{1}", rx, ry);

            tmpCode.AppendFormat("X{0} Y{1} Z{2}\r\n", Gcode.FrmtNum((float)rx), Gcode.FrmtNum((float)ry), Gcode.FrmtNum((float)z));
        }

        /****************************************************************
		***** LOAD data
		*****************************************************************/

        private void ControlHeightMapForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void ControlHeightMapForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string s = (string)e.Data.GetData(DataFormats.Text);
            if (files != null)
            { LoadExtern(files[0]); }
            this.WindowState = FormWindowState.Normal;
        }
  
		public void LoadExtern(string file)		// via drag&drop on main GUI
        {
            if (!File.Exists(file)) return;
            String ext = Path.GetExtension(file).ToLower();
			
            try
            {
				if (ext == ".map")
				{	LoadMap(file, false);}	// false = don't remember path
				if (ext == ".stl")
				{	LoadSTL(file, false);}
                ControlHeightMapForm_SizeChanged(null, null);
            }
            catch (Exception err)
            {
                Logger.Error(err, "LoadExtern {0} ", file);
                MessageBox.Show("Error loading image:\r\n" + err.Message, "Error");
            }

            this.BringToFront();
        }

        // loadHeightMap;	// in MainFormGetCodeTransform
        private void LoadHeightMapToolStripMenuItem_Click(object sender, EventArgs e)
        { BtnLoad_Click(sender, e); }
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog
            {
                InitialDirectory = pathMap,//    Application.StartupPath + Datapath.Examples;
                Filter = "HeightMap|*.map"
            };
            mapIsLoaded = false;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
				LoadMap(sfd.FileName);
                ControlHeightMapForm_SizeChanged(sender, e);
            }
            sfd.Dispose();
        }
		private void LoadMap(string file, bool rememberPath = true)
		{
			Cursor.Current = Cursors.WaitCursor;
            cntReceived = 0; cntSent = 0;
			if (File.Exists(file))
			{
				Map = HeightMap.Load(file);
				lblXDim.Text = string.Format("X Min:{0:0.00} Max:{1:0.00} Step:{2:0.00}  Size:{3}", Map.Min.X, Map.Max.X, Map.GridX, Map.SizeX);
				lblYDim.Text = string.Format("Y Min:{0:0.00} Max:{1:0.00} Step:{2:0.00}  Size:{3}", Map.Min.Y, Map.Max.Y, Map.GridY, Map.SizeY);
				NudNewStepWidth.Value = (decimal)Math.Round(Map.GridX,2);
                nUDCutOffZ.Value = (decimal)Math.Round(Map.MinHeight, 2);
                isMapOk = true;
				EnableControls(true);
				mapIsLoaded = true;
				if (rememberPath) pathMap = Path.GetDirectoryName(file);
				Logger.Info("▀▀▀▀▀▀▀▀▀▀ Load Map size X:{0} Y:{1}  file:{2}", Map.SizeX, Map.SizeY, file);
				lastFileName = Path.GetFileName(file);
			}
			Cursor.Current = Cursors.Default;			
		}
		
        private void BtnLoadSTL_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog
            {
                InitialDirectory = pathExport,//    Application.StartupPath + Datapath.Examples;
                Filter = "STL|*.stl"
            };
            mapIsLoaded = false;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
				LoadSTL(sfd.FileName);
                ControlHeightMapForm_SizeChanged(sender, e);
            }
            sfd.Dispose();
        }
		private void LoadSTL(string file, bool rememberPath = true)
		{
			Cursor.Current = Cursors.WaitCursor;
			cntReceived = 0; cntSent = 0;
			if (File.Exists(file))
			{
				Logger.Info("▀▀▀▀▀▀▀▀▀▀ Load STL file:{0}", file);
				HeightMap tmpMap = HeightMap.ImportSTL(file);
				if (tmpMap != null)
				{
					Map = tmpMap;
					lblXDim.Text = string.Format("X Min:{0:0.00} Max:{1:0.00} Step:{2:0.00}  Size:{3}", Map.Min.X, Map.Max.X, Map.GridX, Map.SizeX);
					lblYDim.Text = string.Format("Y Min:{0:0.00} Max:{1:0.00} Step:{2:0.00}  Size:{3}", Map.Min.Y, Map.Max.Y, Map.GridY, Map.SizeY);
					NudNewStepWidth.Value = (decimal)Math.Round(Map.GridX,2);
                    if ((Map.MinHeight2nd > (float)decimal.MinValue) && (Map.MinHeight2nd < (float)decimal.MaxValue))
                        nUDCutOffZ.Value = (decimal)Math.Round(Map.MinHeight2nd,2);
                    else
                        nUDCutOffZ.Value = 0;

                    isMapOk = true;
					EnableControls(true);
					mapIsLoaded = true;
					lastFileName = Path.GetFileName(file);
                    tabControl1.SelectedTab = tabControl1.TabPages["tabPage3"];
                }
				else
				{ MessageBox.Show("Loading STL file failed", "Error"); }
			
				if (Map.LastError != "")
				{	lblProgress.Text = Map.LastError;
                    lblProgress.BackColor = Color.Yellow;
                }
			}
			if (rememberPath) pathExport = Path.GetDirectoryName(file);
			Cursor.Current = Cursors.Default;
		}


		/****************************************************************
		***** SAVE data
		*****************************************************************/
        private void SavePictureAsBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Map != null)
            {
                Bitmap tmpBMP = new Bitmap(Map.SizeX, Map.SizeY);

                double x, y, z;
                for (int iy = 0; iy < Map.SizeY; iy++)
                {
                    for (int ix = 0; ix < Map.SizeX; ix++)
                    {
                        z = (double)Map.GetPoint(ix, (Map.SizeY-1) - iy);
                        tmpBMP.SetPixel(ix, iy, GetColor(Map.MinHeight, Map.MaxHeight, z, isgray));
                    }
                }

                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Bitmap|*.bmp",
					InitialDirectory = pathLastSaved
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Logger.Info("Save Bitmap {0}", sfd.FileName);
                    tmpBMP.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
					pathLastSaved = Path.GetDirectoryName(sfd.FileName);					
                }
                tmpBMP.Dispose();
                sfd.Dispose();
            }
            else
                MessageBox.Show("No Height Map to save");
        }
		
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (Map == null)
            {
                MessageBox.Show("There is no Map-data to save", "Error");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = pathMap,
				Filter = "HeightMap|*.map"
            };
            Cursor.Current = Cursors.WaitCursor;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Logger.Info("Save Height map {0}", sfd.FileName);
                Map.Save(sfd.FileName);
                pathMap = Path.GetDirectoryName(sfd.FileName);
                pathExport = pathMap;
				lastFileName = Path.GetFileName(sfd.FileName);
            }
            Cursor.Current = Cursors.Default;
            sfd.Dispose();
        }

        private void BtnSaveSTL_Click(object sender, EventArgs e)
        {
            if (Map == null)
            {
                MessageBox.Show("There is no Map-data to save", "Error");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = pathExport,
				Filter = "StereoLithography|*.stl"
            };
            Cursor.Current = Cursors.WaitCursor;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Logger.Info("Save STL {0}", sfd.FileName);
                Map.SaveSTL(sfd.FileName);
                pathExport = Path.GetDirectoryName(sfd.FileName);
            }
            Cursor.Current = Cursors.Default;
            sfd.Dispose();
        }

        private void BtnSaveOBJ_Click(object sender, EventArgs e)
        {
            if (Map == null)
            {
                MessageBox.Show("There is no Map-data to save", "Error");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = pathExport,
				Filter = "3D Object Format|*.obj"
            };
            Cursor.Current = Cursors.WaitCursor;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Logger.Info("Save OBJ {0}", sfd.FileName);
                Map.SaveOBJ(sfd.FileName);
                pathExport = Path.GetDirectoryName(sfd.FileName);
            }
            Cursor.Current = Cursors.Default;
            sfd.Dispose();
        }

        private void BtnSaveX3D_Click(object sender, EventArgs e)
        {
            if (Map == null)
            {
                MessageBox.Show("There is no Map-data to save", "Error");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = pathExport,
				Filter = "X3D|*.x3d"
            };
            Cursor.Current = Cursors.WaitCursor;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Logger.Info("Save X3D {0}", sfd.FileName);
                Map.SaveX3D(sfd.FileName);
                pathExport = Path.GetDirectoryName(sfd.FileName);
            }
            Cursor.Current = Cursors.Default;
            sfd.Dispose();
        }

    }
}
