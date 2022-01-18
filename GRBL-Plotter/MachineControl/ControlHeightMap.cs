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
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
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
        private Bitmap heightMapBMP;
        private Bitmap heightLegendBMP;
        private bool isMapOk = false;
        internal bool mapIsLoaded = false;
        private int estimatedTime = 0;
        private bool notifierEnable = false;
        private bool notifierUpdateMarker = false;
        private bool notifierUpdateMarkerFinish = false;

        private string pathMap;
        private string pathExport;
        private bool refreshPictureBox = true;
        private bool diyControlConnected;
        internal bool scanStarted = false;

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
                //   lblInfo.Text = string.Format("Last XYZ: X:{0} Y:{1} Z:{2} ", posProbe.Z, posProbe.Y, worldZ);
                if (scanStarted && (cntReceived < cntSent))
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
                    pictureBox1.Image = new Bitmap(heightMapBMP);
                    if (refreshPictureBox) pictureBox1.Refresh();
                    lblMin.Text = string.Format("{0:0.000}", Map.MinHeight);
                    lblMid.Text = string.Format("{0:0.000}", (Map.MinHeight + Map.MaxHeight) / 2);
                    lblMax.Text = string.Format("{0:0.000}", Map.MaxHeight);

                    cntReceived++;
                    elapsed = DateTime.UtcNow - timeInit;
                    TimeSpan diff = elapsed.Subtract(elapsedOld);
                    string textNotifier, textPercent;

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
                                //           Logger.Info("Adjust estimated time: old:{0} new:{1}  sec:{2}  cnt:{3}", estimatedTime, newtime, elapsedSeconds, cntReceived);
                                estimatedTime = (int)((3 * estimatedTime + newtime) / 4);
                            }
                        }

                        progressBar1.Value = cntReceived;
                        textPercent = string.Format("{0,4:0.0} %", (100 * (double)cntReceived / cntSent));
                        textNotifier = string.Format("{0} / {1}  {2}  estimated:{3}", cntReceived, progressBar1.Maximum, elapsed.ToString(@"hh\:mm\:ss"), GetTime(estimatedTime));
                        lblProgress.Text = string.Format("{0} {1}", textPercent, textNotifier);
                        textBox1.Text += string.Format("x: {0:0.000} y: {1:0.00} z: {2:0.000}\r\n", Grbl.posWork.X, Grbl.posWork.Y, worldZ);
                        elapsedOld = elapsed;

                        if (notifierEnable && Properties.Settings.Default.notifierMessageProgressEnable)
                        {
                            if ((elapsedSeconds % (int)(60 * Properties.Settings.Default.notifierMessageProgressInterval)) == 5) // offset 5 sec. to get message at start
                            {
                                //            Logger.Info("Notify {0}  {1}", elapsedSeconds, (int)(60 * Properties.Settings.Default.notifierMessageProgressInterval));
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
                        ShowHightMapBMP(heightMapBMP, BMPsizeX, isgray);
                        isMapOk = true;
                        EnableControls(true);
                        progressBar1.Value = cntReceived;
                        textNotifier = string.Format("{0}% {1} / {2}  {3}  estimated:{4}", (100 * cntReceived / progressBar1.Maximum), cntReceived, progressBar1.Maximum, elapsed.ToString(@"hh\:mm\:ss"), GetTime(estimatedTime));
                        lblProgress.Text = textNotifier;
                        textBox1.AppendText(string.Format("x: {0:0.000} y: {1:0.00} z: {2:0.000}\r\n", Grbl.posWork.X, Grbl.posWork.Y, worldZ));
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
                ShowHightMapBMP(heightMapBMP, BMPsizeX, isgray);
            }
        }
        private void BtnOffsetZ_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Map.SetZOffset((double)nUDOffsetZ.Value);
                ShowHightMapBMP(heightMapBMP, BMPsizeX, isgray);
            }
        }

        private void BtnZoomZ_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Map.SetZZoom((double)nUDZoomZ.Value);
                ShowHightMapBMP(heightMapBMP, BMPsizeX, isgray);
            }
        }

        private void BtnInvertZ_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Map.SetZInvert();
                ShowHightMapBMP(heightMapBMP, BMPsizeX, isgray);
            }
        }

        private void BtnCutOffZ_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Map.SetZCutOff((double)nUDCutOffZ.Value);
                ShowHightMapBMP(heightMapBMP, BMPsizeX, isgray);
            }
        }

        private void SavePictureAsBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Map != null)
            {
                int sizeX = 640;
                int sizeY = Map.SizeY * sizeX / Map.SizeX;
                Bitmap tmpBMP = new Bitmap(sizeX, sizeY);
                CreateHightMapBMP(tmpBMP, sizeX, isgray);
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Bitmap|*.bmp"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Logger.Info("Save Bitmap {0}", sfd.FileName);
                    tmpBMP.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                tmpBMP.Dispose();
                sfd.Dispose();
            }
            else
                MessageBox.Show("No Height Map to save");
        }

        private void CreateHightMapBMP(Bitmap bmp, int sizeX, bool gray)
        {
            lblProgress.Text = "Finish t=" + elapsed.ToString(@"hh\:mm\:ss");
            progressBar1.Value = 0;
            int sizeY = Map.SizeY * sizeX / Map.SizeX;
            double x, y, z;
            for (int iy = 0; iy < sizeY; iy++)
            {
                y = Map.Min.Y + (sizeY - iy) * Map.Delta.Y / sizeY;
                for (int ix = 0; ix < sizeX; ix++)
                {
                    x = Map.Min.X + ix * Map.Delta.X / sizeX;
                    z = Map.InterpolateZ(x, y);
                    bmp.SetPixel(ix, iy, GetColor(Map.MinHeight, Map.MaxHeight, z, gray));
                }
            }
        }

        private void ShowHightMapBMP(Bitmap bmp, int sizeX, bool gray)
        {
            CreateHightMapBMP(bmp, sizeX, gray);
            pictureBox1.Image = new Bitmap(bmp);
            pictureBox1.Refresh();
            lblMin.Text = string.Format("{0:0.000}", Map.MinHeight);
            lblMid.Text = string.Format("{0:0.000}", (Map.MinHeight + Map.MaxHeight) / 2);
            lblMax.Text = string.Format("{0:0.000}", Map.MaxHeight);
            pictureBox2.Image = new Bitmap(heightLegendBMP);
            pictureBox2.Refresh();
            nUDCutOffZ.Value = (decimal)Map.MinHeight;
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
                ShowHightMapBMP(heightMapBMP, BMPsizeX, isgray);
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
            btnSave.Enabled = enable;
            btnLoad.Enabled = enable;
            btnOffset.Enabled = enable;
            cBGray.Enabled = enable;
            btnApply.Enabled = enable && isMapOk;
            menuStrip1.Enabled = enable;
            gB_Manipulation.Enabled = enable;
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
        private void BtnSave_Click(object sender, EventArgs e)
        {
			if (Map == null)
			{	MessageBox.Show("There is no Map-data to save","Error");
				return;
			}
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = pathMap//    Application.StartupPath + Datapath.Examples;
            };
            Cursor.Current = Cursors.WaitCursor;
            sfd.Filter = "HeightMap|*.map";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Logger.Info("Save Height map {0}", sfd.FileName);
                Map.Save(sfd.FileName);
                pathMap = Path.GetDirectoryName(sfd.FileName);
                pathExport = pathMap;
            }
            Cursor.Current = Cursors.Default;
            sfd.Dispose();
        }

        private void BtnSaveSTL_Click(object sender, EventArgs e)
        {
			if (Map == null)
			{	MessageBox.Show("There is no Map-data to save","Error");
				return;
			}
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = pathExport//    Application.StartupPath + Datapath.Examples;
            };
            Cursor.Current = Cursors.WaitCursor;
            sfd.Filter = "StereoLithography|*.stl";
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
			{	MessageBox.Show("There is no Map-data to save","Error");
				return;
			}
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = pathExport//    Application.StartupPath + Datapath.Examples;
            };
            Cursor.Current = Cursors.WaitCursor;
            sfd.Filter = "3D Object Format|*.obj";
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
			{	MessageBox.Show("There is no Map-data to save","Error");
				return;
			}
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = pathExport//    Application.StartupPath + Datapath.Examples;
            };
            Cursor.Current = Cursors.WaitCursor;
            sfd.Filter = "X3D|*.x3d";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Logger.Info("Save X3D {0}", sfd.FileName);
                Map.SaveX3D(sfd.FileName);
                pathExport = Path.GetDirectoryName(sfd.FileName);
            }
            Cursor.Current = Cursors.Default;
            sfd.Dispose();
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
                cntReceived = 0; cntSent = 0;
                if (File.Exists(sfd.FileName))
                {
                    Map = HeightMap.Load(sfd.FileName);
                    //    Map.Load(sfd.FileName);
                    lblXDim.Text = string.Format("X Min:{0} Max:{1} Step:{2}", Map.Min.X, Map.Max.X, Map.SizeX);
                    lblYDim.Text = string.Format("Y Min:{0} Max:{1} Step:{2}", Map.Min.Y, Map.Max.Y, Map.SizeY);
                    BMPsizeX = 240;
                    BMPsizeY = Map.SizeY * BMPsizeX / Map.SizeX;
                    heightMapBMP = new Bitmap(BMPsizeX, BMPsizeY);
                    ShowHightMapBMP(heightMapBMP, BMPsizeX, isgray);
                    isMapOk = true;
                    EnableControls(true);
                    mapIsLoaded = true;
                    pathMap = Path.GetDirectoryName(sfd.FileName);
                    pathExport = pathMap;
                }
            }
            sfd.Dispose();
        }

        private void BtnPosLL_Click(object sender, EventArgs e)
        {
            nUDX1.Value = (decimal)Grbl.posWork.X;
            nUDY1.Value = (decimal)Grbl.posWork.Y;
            nUDX2.Value = nUDDeltaX.Value + nUDX1.Value;
            nUDY2.Value = nUDDeltaY.Value + nUDY1.Value;
        }

        private void BtnPosUR_Click(object sender, EventArgs e)
        {
            nUDX2.Value = (decimal)Grbl.posWork.X;
            nUDY2.Value = (decimal)Grbl.posWork.Y;
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

        private int BMPsizeX = 160;
        private int BMPsizeY = 160;

        private void ControlHeightMapForm_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.locationImageForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }

            //_event = new eventArgsTemplates();

            nUDDeltaX.Value = Properties.Settings.Default.heightMapX2 - Properties.Settings.Default.heightMapX1;
            nUDDeltaY.Value = Properties.Settings.Default.heightMapY2 - Properties.Settings.Default.heightMapY1;
            int legendHeight = 160;
            heightLegendBMP = new Bitmap(2, legendHeight);
            for (int i = 0; i < legendHeight; i++)
            {
                heightLegendBMP.SetPixel(0, (legendHeight - 1) - i, GetColor(0, legendHeight, i, isgray));
                heightLegendBMP.SetPixel(1, (legendHeight - 1) - i, GetColor(0, legendHeight, i, isgray));
            }
            pictureBox2.Image = new Bitmap(heightLegendBMP);
            pictureBox2.Refresh();
            int stepX = (int)Math.Round((nUDX2.Value - nUDX1.Value) / nUDGridX.Value);
            int stepY = (int)Math.Round((nUDY2.Value - nUDY1.Value) / nUDGridY.Value);
            lblXDim.Text = string.Format("X Min:{0} Max:{1} Step:{2}", nUDX1.Value, nUDX2.Value, stepX);
            lblYDim.Text = string.Format("Y Min:{0} Max:{1} Step:{2}", nUDY1.Value, nUDY2.Value, stepY);
            pathMap = Datapath.Examples;
            RbProbingDiy.Checked = !Properties.Settings.Default.heightMapProbeUseZ;
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
                DialogResult result;
                result = MessageBox.Show("Move to this position? " + posX.ToString() + " ; " + posY.ToString(), "Attention", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    if ((decimal)Grbl.posWork.Z < nUDProbeUp.Value)
                        OnRaiseXyzEvent(new XyzEventArgs(null, null, (double)nUDProbeUp.Value, "G90"));
                    OnRaiseXyzEvent(new XyzEventArgs(posX, posY, "G90"));   // move relative and fast
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
                double iX = (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).X) / pictureBox1.Width);
                double iY = (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).Y) / pictureBox1.Height);
                double relposX = Map.Delta.X * iX;
                double relposY = Map.Delta.Y - Map.Delta.Y * iY;
                double posX = Map.Min.X + relposX;
                double posY = Map.Min.Y + relposY;
                double? posZ = Map.GetPoint((int)(iX * Map.SizeX), (int)(iY * Map.SizeY));
                //            ToolTip tt = new ToolTip();
                //            tt.SetToolTip(this.pictureBox1, string.Format("X:{0:0.00} Y:{1:0.00}  Z:{2:0.00}", posX, posY, posZ));
                tt.SetToolTip(this.pictureBox1, string.Format("X:{0:0.00} Y:{1:0.00}  Z:{2:0.00}", posX, posY, posZ));
            }
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
				{	MessageBox.Show(Localization.GetString("grblNotConnected"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                x1 = Math.Min(nUDX1.Value, nUDX2.Value);
                x2 = Math.Max(nUDX1.Value, nUDX2.Value);
                y1 = Math.Min(nUDY1.Value, nUDY2.Value);
                y2 = Math.Max(nUDY1.Value, nUDY2.Value);
                if (x1 == x2) x2 = x1 + 10;
                if (y1 == y2) y2 = y1 + 10;
                nUDX1.Value = x1; nUDX2.Value = x2; nUDY1.Value = y1; nUDY2.Value = y2;
                //decimal stepX = (x2 - x1) / (nUDGridX.Value - 1);
                //decimal stepY = (y2 - y1) / (nUDGridY.Value - 1);
                cntSent = 0; cntReceived = 0;
                Gcode.ReduceGCode = true;   // reduce number format to #.# in gcode.frmtNum()

                Map = new HeightMap((double)nUDGridX.Value, new Vector2((double)x1, (double)y1), new Vector2((double)x2, (double)y2));
                MapIndex = new List<Point>();

                lblXDim.Text = string.Format("X Min:{0} Max:{1} Step:{2}", Map.Min.X, Map.Max.X, Map.SizeX);
                lblYDim.Text = string.Format("Y Min:{0} Max:{1} Step:{2}", Map.Min.Y, Map.Max.Y, Map.SizeY);

                CalculateEstimatedTime();

                notifierEnable = ((double)Properties.Settings.Default.notifierMessageProgressInterval < (estimatedTime / 60));
                notifierUpdateMarker = true;
                notifierUpdateMarkerFinish = false;

                textBox1.Clear();
                textBox1.Text += string.Format("(Probing Size X:{0} Y:{1} )", Map.SizeX, Map.SizeY);
                int pixX, pixY;
                BMPsizeX = 240;
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
                for (int iy = 0; iy < Map.SizeY; iy++)
                {
                    tmp = Map.GetCoordinates(0, iy);
                    if (useZ) scanCode.AppendFormat("G0Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeUp.Value));
                    //                    scanCode.AppendFormat("G0Y{0}\r\n", Gcode.FrmtNum((float)tmp.Y));
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
                        //                    scanCode.AppendFormat("G0Y{0}\r\n", Gcode.FrmtNum((float)tmp.Y));
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
                if (useZ) scanCode.AppendFormat("G0 Z{0}\r\n", Gcode.FrmtNum((float)nUDProbeUp.Value));
                tmp = Map.GetCoordinates(0, 0);
                scanCode.AppendFormat("G0 X{0} Y{1}\r\n", Gcode.FrmtNum((float)tmp.X), Gcode.FrmtNum((float)tmp.Y));
                scanCode.AppendLine("M30");     // finish

                textBox1.Text += "\r\nCode sent\r\n" + scanCode.ToString() + "\r\n##########################\r\n";
                progressBar1.Maximum = cntSent;
                lblProgress.Text = string.Format("{0}%  estimated time:{1}", (100 * cntReceived / progressBar1.Maximum), GetTime(estimatedTime));
                pictureBox1.Image = new Bitmap(heightMapBMP);
                pictureBox1.Refresh();
                ControlPowerSaving.SuppressStandby();
                scanStarted = true;
            }
            else
            {
                btnStartHeightScan.Text = "Generate Height Map";
                refreshPictureBox = false;
                notifierEnable = false;
                ControlPowerSaving.EnableStandby();
                if ((Map != null) && (cntReceived < cntSent))   // fill missing coordiantes
                {

                    double worldZ = 0 - (Grbl.posMachine.Z - Grbl.posWork.Z);
                    while (cntReceived < cntSent)
                    {
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

        /************************************************************************
        Generate GCode for probing
        getGCodeFromHeightMap;      			// in MainFormGetCodeTransform
        ************************************************************************/
        private void BtnGCode_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Vector2 tmp;
                //          double z;
                float gcodeZFeed = (float)Properties.Settings.Default.importGCZFeed;
                float gcodeZUp = (float)Properties.Settings.Default.importGCZUp;

                scanCode = new StringBuilder();
                StringBuilder tmpCode = new StringBuilder();
                tmp = Map.GetCoordinates(0, 0);

                tmpCode.AppendFormat("G90 G0 F{0} Z{1}\r\n", Gcode.FrmtNum(gcodeZFeed), Gcode.FrmtNum(gcodeZUp));
                tmpCode.AppendFormat("X{0} Y{1}\r\n", Gcode.FrmtNum((float)tmp.X), Gcode.FrmtNum((float)tmp.Y));
                tmpCode.AppendFormat("G1\r\n");

                for (int iy = 0; iy < Map.SizeY; iy++)
                {
                    for (int ix = 0; ix < Map.SizeX; ix++)
                    {
                        MoveXYZ(tmpCode, ix, iy);
                    }
                    if (iy < Map.SizeY - 1)
                    {
                        iy++;
                        for (int ix = Map.SizeX - 1; ix >= 0; ix--)
                        {
                            MoveXYZ(tmpCode, ix, iy);
                        }
                    }
                }
                tmpCode.AppendFormat("G0 Z{0}\r\n", Gcode.FrmtNum(gcodeZUp));
                tmp = Map.GetCoordinates(0, 0);
                tmpCode.AppendFormat("G0 X{0} Y{1}\r\n", Gcode.FrmtNum((float)tmp.X), Gcode.FrmtNum((float)tmp.Y));

                scanCode.AppendFormat("{0}", Gcode.GetHeader("Height Map", ""));
                scanCode.Append(tmpCode);
                scanCode.AppendFormat("{0}", Gcode.GetFooter());
            }
        }

        private void RbProbingZ_CheckedChanged(object sender, EventArgs e)
        {
            GbProbingZ.Enabled = RbProbingZ.Checked;
            GbProbingDiy.Enabled = RbProbingDiy.Checked;
        }

        private void BtnMoveLL_Click(object sender, EventArgs e)
        {

        }

        private void BtnMoveUR_Click(object sender, EventArgs e)
        {

        }

        private void BtnPosFromCodeDimension_Click(object sender, EventArgs e)
        {
            if (GuiVariables.variable["GMIX"] != GuiVariables.variable["GMAX"])
            {   nUDX1.Value = (decimal)(CbRoundUp.Checked ? Math.Floor(GuiVariables.variable["GMIX"]) : GuiVariables.variable["GMIX"]);
                nUDX2.Value = (decimal)(CbRoundUp.Checked ? Math.Ceiling(GuiVariables.variable["GMAX"]) : GuiVariables.variable["GMAX"]);
            }
            if (GuiVariables.variable["GMIY"] != GuiVariables.variable["GMAY"])
            {
                nUDY1.Value = (decimal)(CbRoundUp.Checked ? Math.Floor(GuiVariables.variable["GMIY"]) : GuiVariables.variable["GMIY"]);
                nUDY2.Value = (decimal)(CbRoundUp.Checked ? Math.Ceiling(GuiVariables.variable["GMAY"]) : GuiVariables.variable["GMAY"]);
            }
        }

        private void MoveXYZ(StringBuilder tmpCode, int ix, int iy)
        {
            Vector2 tmp = Map.GetCoordinates(ix, iy);
            double z = (double)Map.GetPoint(ix, iy);
            tmpCode.AppendFormat("X{0} Y{1} Z{2}\r\n", Gcode.FrmtNum((float)tmp.X), Gcode.FrmtNum((float)tmp.Y), Gcode.FrmtNum((float)z));
        }

        public void StopScan()
        {
            scanStarted = false;
            btnStartHeightScan.Text = "Generate Height Map";
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            EnableControls(true);
        }

    }

    public class HeightMap
    {
        public double?[,] Points { get; private set; }
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        //     internal int TotalPoints { get { return SizeX * SizeY; } }

        internal Queue<Tuple<int, int>> NotProbed { get; private set; } = new Queue<Tuple<int, int>>();

        internal Vector2 Min { get; private set; }
        internal Vector2 Max { get; private set; }

        internal Vector2 Delta { get { return Max - Min; } }

        public double MinHeight { get; set; } = double.MaxValue;
        public double MaxHeight { get; set; } = double.MinValue;

        //        public event Action MapUpdated;

        public double GridX { get { return (Max.X - Min.X) / (SizeX - 1); } }
        public double GridY { get { return (Max.Y - Min.Y) / (SizeY - 1); } }


        internal HeightMap(double gridSize, Vector2 min, Vector2 max)
        {
            MinHeight = double.MaxValue;
            MaxHeight = double.MinValue;

            if (min.X == max.X) { max.X = min.X + 1; }
            if (min.Y == max.Y) { max.Y = min.Y + 1; }
            //                throw new Exception("Height map can't be infinitely narrow");

            int pointsX = (int)Math.Ceiling((max.X - min.X) / gridSize) + 1;
            int pointsY = (int)Math.Ceiling((max.Y - min.Y) / gridSize) + 1;

            if (pointsX == 0) { pointsX = 1; }
            if (pointsY == 0) { pointsY = 1; }
            //        throw new Exception("Height map must have at least 4 points");

            Points = new double?[pointsX, pointsY];

            if (max.X < min.X)
            {
                double a = min.X;
                min.X = max.X;
                max.X = a;
            }

            if (max.Y < min.Y)
            {
                double a = min.Y;
                min.Y = max.Y;
                max.Y = a;
            }

            Min = min;
            Max = max;

            SizeX = pointsX;
            SizeY = pointsY;

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                    NotProbed.Enqueue(new Tuple<int, int>(x, y));

                if (++x >= SizeX)
                    break;

                for (int y = SizeY - 1; y >= 0; y--)
                    NotProbed.Enqueue(new Tuple<int, int>(x, y));
            }
        }

        public double InterpolateZ(double x, double y)	// find Z for given XY coordinate
        {
            if (x > Max.X || x < Min.X || y > Max.Y || y < Min.Y)	// out of area?
                return MaxHeight;

            x -= Min.X;		// convert coordinate to index
            y -= Min.Y;		// remove offset

            x /= GridX;		// convert coordinate to index
            y /= GridY;		// devide by resolution

            int iLX = (int)Math.Floor(x);   //lower integer part
            int iLY = (int)Math.Floor(y);

			if (iLX < 0) iLX = 0;		// check range
			if (iLY < 0) iLY = 0;
			
            int iHX = (int)Math.Ceiling(x); //upper integer part
            int iHY = (int)Math.Ceiling(y);

			if (iHX >= SizeX) iHX = SizeX - 1;	// check range
			if (iHY >= SizeY) iHY = SizeY - 1;

            //     try
            //     {
            double fX = x - iLX;             //fractional part
            double fY = y - iLY;

			double hxhy = Points[iHX, iHY].GetValueOrDefault(MaxHeight);
			double lxhy = Points[iLX, iHY].GetValueOrDefault(MaxHeight);
			double hxly = Points[iHX, iLY].GetValueOrDefault(MaxHeight);
			double lxly = Points[iLX, iLY].GetValueOrDefault(MaxHeight);
            double linUpper = hxhy * fX + lxhy * (1 - fX); 
            double linLower = hxly * fX + lxly * (1 - fX);
			
    //        double linUpper = Points[iHX, iHY].Value * fX + Points[iLX, iHY].Value * (1 - fX);       //linear immediates
    //        double linLower = Points[iHX, iLY].Value * fX + Points[iLX, iLY].Value * (1 - fX);

            return linUpper * fY + linLower * (1 - fY);     //bilinear result
                                                            //   } catch { return MaxHeight; }
        }

        internal Vector2 GetCoordinates(int x, int y, bool applyOffset = true)
        {
            if (applyOffset)
                return new Vector2(x * (Delta.X / (SizeX - 1)) + Min.X, y * (Delta.Y / (SizeY - 1)) + Min.Y);
            else
                return new Vector2(x * (Delta.X / (SizeX - 1)), y * (Delta.Y / (SizeY - 1)));
        }

        private HeightMap()
        { }

        public void AddPoint(int x, int y, double height)
        {
            Points[x, y] = height;

            if (height > MaxHeight)
                MaxHeight = height;
            if (height < MinHeight)
                MinHeight = height;
        }
        public double? GetPoint(int x, int y)
        {
            if ((x >= 0) && (x < SizeX) && (y >= 0) && (y < SizeY))
                return Points[x, y];
            return null;
        }
        public void SetZOffset(double offset)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = Points[ix, iy] + offset;
                }
            }
            MaxHeight += offset;
            MinHeight += offset;
        }
        public void SetZZoom(double zoom)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = Points[ix, iy] * zoom;
                }
            }
            MaxHeight *= zoom;
            MinHeight *= zoom;
        }
        public void SetZInvert()
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = -Points[ix, iy];
                }
            }
            double tmp = MaxHeight;
            MaxHeight = -MinHeight;
            MinHeight = -tmp;
        }
        public void SetZCutOff(double limit)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    if (Points[ix, iy] < limit)
                        Points[ix, iy] = limit;
                }
            }
            MinHeight = limit;
        }

        public XmlReaderSettings settings = new XmlReaderSettings()
        { DtdProcessing = DtdProcessing.Prohibit };
        public static HeightMap Load(string path)
        {
            HeightMap map = new HeightMap();
            XmlReader content = XmlReader.Create(path);//, settings);
            map.MaxHeight = double.MinValue;
            map.MinHeight = double.MaxValue;

            while (content.Read())
            {
                if (!content.IsStartElement())
                    continue;

                switch (content.Name)
                {
                    case "heightmap":
                        map.Min = new Vector2(double.Parse(content["MinX"].Replace(',', '.'), NumberFormatInfo.InvariantInfo), double.Parse(content["MinY"].Replace(',', '.'), NumberFormatInfo.InvariantInfo));
                        map.Max = new Vector2(double.Parse(content["MaxX"].Replace(',', '.'), NumberFormatInfo.InvariantInfo), double.Parse(content["MaxY"].Replace(',', '.'), NumberFormatInfo.InvariantInfo));
                        map.SizeX = int.Parse(content["SizeX"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        map.SizeY = int.Parse(content["SizeY"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        map.Points = new double?[map.SizeX, map.SizeY];
                        break;
                    case "point":
                        int x = int.Parse(content["X"].Replace(',', '.')), y = int.Parse(content["Y"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        double height = double.Parse(content.ReadInnerXml().Replace(',', '.'), NumberFormatInfo.InvariantInfo);

                        map.Points[x, y] = height;

                        if (height > map.MaxHeight)
                            map.MaxHeight = height;
                        if (height < map.MinHeight)
                            map.MinHeight = height;

                        break;
                }
            }
            for (int x = 0; x < map.SizeX; x++)
            {
                for (int y = 0; y < map.SizeY; y++)
                    if (!map.Points[x, y].HasValue)
                        map.NotProbed.Enqueue(new Tuple<int, int>(x, y));

                if (++x >= map.SizeX)
                    break;

                for (int y = map.SizeY - 1; y >= 0; y--)
                    if (!map.Points[x, y].HasValue)
                        map.NotProbed.Enqueue(new Tuple<int, int>(x, y));
            }
            //       r.Dispose();
            return map;
        }

        private static readonly string formatNumber = "0.000";
        private static string FrmtNum(double number)     // convert float to string using format pattern
        { return number.ToString(formatNumber); }

        // vertex coordinates must be positive-definite (nonnegative and nonzero) numbers. 
        // The StL file does not contain any scale information; the coordinates are in arbitrary units.
        public void SaveSTL(string path)
        {
            StringBuilder data = new StringBuilder();
            data.AppendLine("solid ASCII_STL_GRBL_Plotter");
            double z0, z1, z2, z3;
            Vector2 p0, p1, p2, p3;
            for (int y = 0; y < (SizeY - 1); y++)
            {
                for (int x = 0; x < (SizeX - 1); x++)
                {
                    if (!Points[x, y].HasValue)
                        continue;
                    p0 = GetCoordinates(x, y, false);       // vertex coordinates must be positive-definite (nonnegative and nonzero) numbers. 
                    p1 = GetCoordinates(x, y + 1, false);
                    p2 = GetCoordinates(x + 1, y, false);
                    p3 = GetCoordinates(x + 1, y + 1, false);
                    z0 = -1 * Points[x, y].Value;    // vertex coordinates must be positive-definite (nonnegative and nonzero) numbers. 
                    z1 = -1 * Points[x, y + 1].Value;
                    z2 = -1 * Points[x + 1, y].Value;
                    z3 = -1 * Points[x + 1, y + 1].Value;

                    data.AppendLine(" facet normal 0 0 0");
                    data.AppendLine("  outer loop");
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p0.X, p0.Y, z0);
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p1.X, p1.Y, z1);
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p2.X, p2.Y, z2);
                    data.AppendLine("  endloop");
                    data.AppendLine(" endfacet");

                    data.AppendLine(" facet normal 0 0 0");
                    data.AppendLine("  outer loop");
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p1.X, p1.Y, z1);
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p3.X, p3.Y, z3);
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p2.X, p2.Y, z2);
                    data.AppendLine("  endloop");
                    data.AppendLine(" endfacet");
                }
            }
            data.AppendLine("endsolid ASCII_STL_GRBL_Plotter");
            File.WriteAllText(path, data.ToString().Replace(',', '.'));
            data.Clear();
        }

        // https://www.fileformat.info/format/wavefrontobj/egff.htm
        // https://people.sc.fsu.edu/~jburkardt/data/obj/obj.html
        // https://en.wikipedia.org/wiki/Wavefront_.obj_file
        public void SaveOBJ(string path)
        {
            StringBuilder data = new StringBuilder();
            StringBuilder data_point = new StringBuilder();
            StringBuilder data_face = new StringBuilder();
            Dictionary<int, StringBuilder> data_face_color = new Dictionary<int, StringBuilder>();

            OBJdata_color = new StringBuilder();
            OBJused_color = new Dictionary<Color, int>();   // to check if color is already indexed
            OBJcolor_used = 0;

            string mtl_path = Path.ChangeExtension(path, ".mtl");
            data.AppendLine("#\r\n# GRBL_Plotter Height map");
            data.AppendFormat("# SizeX:{0}  SizeY:{1}\r\n", SizeX, SizeY);
            data.AppendFormat("#\r\n\r\nmtllib {0}\r\n\r\n", Path.GetFileName(mtl_path));
            //          data.AppendLine("o HeightMap");
            //          data.AppendLine("g map1");
            double z0, z1;
            Vector2 p0;
            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    if (!Points[x, y].HasValue)
                        continue;
                    p0 = GetCoordinates(x, y, false);
                    z0 = Points[x, y].Value;
                    data_point.AppendFormat("v {0} {1} {2:0.0000}\r\n", p0.X, p0.Y, z0);

                    if ((y < (SizeY - 1)) && (x < (SizeX - 1)))
                    {
                        int color_index = CheckOBJColorUse(z0);
                        if (data_face_color.ContainsKey(color_index))
                        {
                            data_face_color[color_index].AppendFormat("f {0} {1} {2}\r\n", SizeX * y + x + 1, SizeX * y + x + 2, SizeX * (y + 1) + x + 1);		// 1. triangle
                        }
                        else
                        {
                            data_face_color.Add(color_index, new StringBuilder());
                            data_face_color[color_index].AppendFormat("f {0} {1} {2}\r\n", SizeX * y + x + 1, SizeX * y + x + 2, SizeX * (y + 1) + x + 1);		// 1. triangle
                        }

                        z1 = Points[x + 1, y].Value;
                        color_index = CheckOBJColorUse(z1);
                        if (data_face_color.ContainsKey(color_index))
                        {
                            data_face_color[color_index].AppendFormat("f {0} {1} {2}\r\n", SizeX * (y + 1) + x + 1, SizeX * (y + 1) + x + 2, SizeX * y + x + 2);       // 2. triangle																		
                        }    // 2. triangle																		
                        else
                        {
                            data_face_color.Add(color_index, new StringBuilder());
                            data_face_color[color_index].AppendFormat("f {0} {1} {2}\r\n", SizeX * (y + 1) + x + 1, SizeX * (y + 1) + x + 2, SizeX * y + x + 2);       // 2. triangle																		
                        }
                    }
                }
            }
            data_point.AppendLine("o HeightMap");
            data_point.AppendLine("g map1");

            data.Append(data_point.ToString());
            //data.Append(data_face.ToString());
            for (int i = 0; i < OBJcolor_used; i++)
            {
                if (data_face_color.ContainsKey(i))
                {
                    data_face.AppendFormat("usemtl c_{0}\r\n", i);
                    data_face.Append(data_face_color[i].ToString());
                }
            }
            data.Append(data_face.ToString());

            File.WriteAllText(path, data.ToString().Replace(',', '.'));	// save obj file
            File.WriteAllText(mtl_path, OBJdata_color.ToString().Replace(',', '.'));    // save mtl file		
            OBJdata_color.Clear();
            OBJused_color.Clear();
            data.Clear();
            data_point.Clear();
            data_face.Clear();
        }

        private static Dictionary<Color, int> OBJused_color = new Dictionary<Color, int>(); // to check if color is already indexed
        private static int OBJcolor_used = 0;
        private static StringBuilder OBJdata_color = new StringBuilder();

        private int CheckOBJColorUse(double zVal)
        {
            Color tmpColor = GetColor(MinHeight, MaxHeight, zVal, false);
            if (OBJused_color.ContainsKey(tmpColor))
            { return OBJused_color[tmpColor]; }
            else
            {
                OBJused_color.Add(tmpColor, OBJcolor_used);
                //			data_face.AppendFormat("usemtl c_{0}",OBJcolor_used);}
                // create mtl file
                OBJdata_color.AppendFormat("newmtl c_{0}\r\n", OBJcolor_used);
                OBJdata_color.AppendFormat("Kd {0} {1} {2}\r\n", (double)tmpColor.R / 255, (double)tmpColor.G / 255, (double)tmpColor.B / 255);
                OBJdata_color.AppendFormat("Ks {0} {1} {2}\r\n", (double)tmpColor.R / 255, (double)tmpColor.G / 255, (double)tmpColor.B / 255);
                OBJdata_color.AppendFormat("Ka {0} {1} {2}\r\n", (double)tmpColor.R / 255, (double)tmpColor.G / 255, (double)tmpColor.B / 255);
                //         OBJdata_color.AppendFormat("Ks 0.8 0.8 0.8\r\n");
                //		OBJdata_color.AppendFormat("Ka 0.2 0.2 0.2\r\n");
                OBJdata_color.AppendFormat("illum 2\r\n");
                OBJdata_color.AppendFormat("Ns 40\r\n\r\n");

                OBJcolor_used++;
                return OBJcolor_used - 1;
            }
        }

        public void SaveX3D(string path)
        {
            StringBuilder object_code = new StringBuilder();
            StringBuilder color_code = new StringBuilder();
            bool first_val = true;
            if (true)//elevation)
            {
                object_code.AppendLine(" <Transform DEF='elevationgrid' containerField='children' translation='0 0 0'>");
                object_code.AppendLine("  <Shape DEF='GRBL-Plotter Height Map' containerField='children'>");
                object_code.AppendFormat("    <ElevationGrid creaseAngle='3.14159' solid='false' xDimension='{0}' xSpacing='1' zDimension='{1}' zSpacing='1' height='", SizeX, SizeY);
                for (int y = (SizeY - 1); y >= 0; y--) //(int y = 0; y < SizeY; y++)
                {
                    for (int x = 0; x < SizeX; x++)
                    {
                        if (first_val) { first_val = false; }
                        else { object_code.Append(","); color_code.Append(","); }
                        if (x == 0) { object_code.Append("\r\n      "); color_code.Append("\r\n         "); }
                        object_code.Append(FrmtNum(Points[x, y].Value).Replace(',', '.'));
                        color_code.Append(GetColorString(Points[x, y].Value).Replace(',', '.'));
                    }
                }
                object_code.Append("'>\r\n");
                object_code.AppendFormat("        <Color color='{0}'/>\r\n", color_code);
                object_code.Append("     </ElevationGrid>\r\n");
                object_code.Append("  </Shape>\r\n");
                object_code.Append(" </Transform>\r\n");
            }
            string file_head = "", file_foot = "";
            file_head += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n";
            file_head += "<!DOCTYPE X3D PUBLIC \"ISO//Web3D//DTD X3D 3.0//EN\" \"http://www.web3d.org/specifications/x3d-3.0.dtd\">\r\n";
            file_head += "<X3D profile='Immersive' >\r\n";
            file_head += "<head></head>\r\n";
            file_head += "<Scene>\r\n";

            file_head += "<NavigationInfo containerField='children' avatarSize='.25 1.6 .75' visibilityLimit='0' speed='10' headlight='true' type='\"EXAMINE\" \"ANY\"'/>\r\n";
            file_head += "<Background containerField='children' skyAngle=' .7854 1.91986' skyColor='  0 .2 .70196 0 .50196 1 1 1 1' groundAngle='1.5708' groundColor='  .2 .2 .2 .8 .8 .8'/>\r\n";

            file_head += "<Transform DEF='dad_Group_light' rotation='-.286 -.914 -.286 1.66'>\r\n";
            file_head += "  <Transform DEF='light1_t' containerField='children' translation='" + SizeY / 2 + " 0 " + 3 * SizeX + "' scale='2 2 2'>\r\n";
            file_head += "    <SpotLight DEF='light1' containerField='children' ambientIntensity='0.000' intensity='1.000' radius='100.000' cutOffAngle='1.309' beamWidth='0.785' attenuation='1 0 0' color='1 1 1' on='true'/>\r\n";
            file_head += "  </Transform>\r\n";
            file_head += "</Transform>\r\n";

            // camera static
            var camera_static_distance = SizeX * 2;
            var camera_static_angle = 45 * Math.PI / 180;
            file_head += "<Transform DEF='dad_Group_static_camera' translation='" + SizeX / 2 + " 0 " + SizeY / 2 + "' rotation='-1 0 0 " + camera_static_angle + "'>\r\n";
            file_head += " <Viewpoint DEF='Viewpoint_static_camera' containerField='children' description='Static camera' jump='true' fieldOfView='0.785' position='0 0 " + camera_static_distance + "' orientation='0 0 1 0'/>\r\n";
            file_head += "</Transform>\r\n";

            //            file_head += navi + back + light + camera + plate + text + legend;
            file_foot += "</Scene>\r\n</X3D>\r\n";
            string file_data = file_head.Replace(',', '.') + object_code.ToString() + file_foot;
            File.WriteAllText(path, file_data);     // .Replace(',', '.')
            object_code.Clear();
            color_code.Clear();
        }

        public void Save(string path)
        {
            XmlWriterSettings set = new XmlWriterSettings
            {
                Indent = true
            };
            XmlWriter w = XmlWriter.Create(path, set);
            w.WriteStartDocument();
            w.WriteStartElement("heightmap");
            w.WriteAttributeString("MinX", Min.X.ToString().Replace(',', '.'));
            w.WriteAttributeString("MinY", Min.Y.ToString().Replace(',', '.'));
            w.WriteAttributeString("MaxX", Max.X.ToString().Replace(',', '.'));
            w.WriteAttributeString("MaxY", Max.Y.ToString().Replace(',', '.'));
            w.WriteAttributeString("SizeX", SizeX.ToString().Replace(',', '.'));
            w.WriteAttributeString("SizeY", SizeY.ToString().Replace(',', '.'));

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    if (!Points[x, y].HasValue)
                        continue;

                    w.WriteStartElement("point");
                    w.WriteAttributeString("X", x.ToString().Replace(',', '.'));
                    w.WriteAttributeString("Y", y.ToString().Replace(',', '.'));
                    w.WriteString(FrmtNum(Points[x, y].Value).Replace(',', '.'));
                    w.WriteEndElement();
                }
            }
            w.WriteEndElement();
            w.Close();
        }

        public static Color GetColor(double min, double max, double value, bool gray)
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
        public String GetColorString(double value)
        {
            Color tmp = GetColor(MinHeight, MaxHeight, value, false);
            return string.Format("{0:0.00} {1:0.00} {2:0.00}", (double)tmp.R / 255, (double)tmp.G / 255, (double)tmp.B / 255);
        }

        internal void FillWithTestPattern(string pattern)
        {
            DataTable t = new DataTable();

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    double X = (x * (Max.X - Min.X)) / (SizeX - 1) + Min.X;
                    double Y = (y * (Max.Y - Min.Y)) / (SizeY - 1) + Min.Y;

                    decimal d = (decimal)t.Compute(pattern.Replace("x", X.ToString()).Replace("y", Y.ToString()), "");
                    AddPoint(x, y, (double)d);
                }
            }
            t.Dispose();
        }
    }

    public struct Vector2 : IEquatable<Vector2>
    {

        private double x;

        private double y;

        public Vector2(double x, double y)
        {
            // Pre-initialisation initialisation
            // Implemented because a struct's variables always have to be set in the constructor before moving control
            this.x = 0;
            this.y = 0;

            // Initialisation
            X = x;
            Y = y;
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Length()
        {
            return (float)Math.Sqrt(x*x+y*y);
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return
            (
                new Vector2
                    (
                        v1.X + v2.X,
                        v1.Y + v2.Y
                    )
            );
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return
            (
                new Vector2
                    (
                        v1.X - v2.X,
                        v1.Y - v2.Y
                    )
            );
        }

        public static Vector2 operator *(Vector2 v1, double s2)
        {
            return
            (
                new Vector2
                (
                    v1.X * s2,
                    v1.Y * s2
                )
            );
        }

        public static Vector2 operator *(double s1, Vector2 v2)
        {
            return v2 * s1;
        }

        public static Vector2 operator /(Vector2 v1, double s2)
        {
            return
            (
                new Vector2
                    (
                        v1.X / s2,
                        v1.Y / s2
                    )
            );
        }

        public static Vector2 operator -(Vector2 v1)
        {
            return
            (
                new Vector2
                    (
                        -v1.X,
                        -v1.Y
                    )
            );
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return
            (
                Math.Abs(v1.X - v2.X) <= EqualityTolerence &&
                Math.Abs(v1.Y - v2.Y) <= EqualityTolerence
            );
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return !(v1 == v2);
        }

        public bool Equals(Vector2 other)
        {
            return other == this;
        }

        public override bool Equals(object other)
        {
            // Convert object to Vector3
            // Check object other is a Vector3 object
            if (other is Vector2 otherVector)
            {
                // Check for equality
                return otherVector == this;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return
            (
                (int)((X + Y) % Int32.MaxValue)
            );
        }

        public const double EqualityTolerence = double.Epsilon;
    }

}
