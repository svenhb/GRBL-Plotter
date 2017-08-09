/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2017 Sven Hasemann contact: svenhb@web.de

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

*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace GRBL_Plotter
{
    public partial class ControlHeightMapForm : Form
    {
        private xyzPoint actualPosWorld;
        private xyzPoint actualPosMachine;
        private xyzPoint actualPosProbe;
        private StringBuilder scanCode;
        public HeightMap Map;
        private List<Point> MapIndex;
        private Bitmap heightMapBMP;
        private Bitmap heightLegendBMP;
        private bool isMapOk = false;

        public xyzPoint setPosWorld
        { set { actualPosWorld = value; } }
        public xyzPoint setPosMachine
        { set { actualPosMachine = value; } }
        public xyzPoint setPosProbe
        {   set {
                actualPosProbe = value;
                double worldZ = actualPosProbe.Z - (actualPosMachine.Z - actualPosWorld.Z);
                Map.AddPoint(MapIndex[cntReceived].X, MapIndex[cntReceived].Y, worldZ);
                using (Graphics graph = Graphics.FromImage(heightMapBMP))
                {
                    int x = MapIndex[cntReceived].X * BMPsizeX / (Map.SizeX-1);
                    int dx= BMPsizeX / (Map.SizeX - 1);
                    int y = BMPsizeY - (MapIndex[cntReceived].Y * BMPsizeY / (Map.SizeY-1));
                    int dy = BMPsizeY / (Map.SizeY - 1);
                    Rectangle ImageSize = new Rectangle(x-dx/2, y-dy/2, dx, dy);
                    SolidBrush myColor = new SolidBrush(getColor(Map.MinHeight, Map.MaxHeight, worldZ, false));
                    graph.FillRectangle(myColor, ImageSize);
                }
                pictureBox1.Image = new Bitmap(heightMapBMP);
                pictureBox1.Refresh();
                lblMin.Text = string.Format("{0:0.000}", Map.MinHeight);
                lblMid.Text = string.Format("{0:0.000}", (Map.MinHeight + Map.MaxHeight) / 2);
                lblMax.Text = string.Format("{0:0.000}", Map.MaxHeight);

                cntReceived++;
                progressBar1.Value = cntReceived;
                elapsed = DateTime.UtcNow - timeInit;
                lblProgress.Text = string.Format("{0}% {1} of {2}  t={3}", (100 * cntReceived / progressBar1.Maximum), cntReceived, progressBar1.Maximum, elapsed.ToString(@"hh\:mm\:ss"));
                textBox1.Text += string.Format("x: {0:0.000} y: {1:0.00} z: {2:0.000}\r\n", actualPosWorld.X, actualPosWorld.Y, worldZ);
                if (cntReceived == progressBar1.Maximum)
                {
                    enableControls(true);
                    scanStarted = false;
                    btnStartHeightScan.Text = "Generate Height Map";
                    showHightMapBMP(heightMapBMP, BMPsizeX, isgray);
                    isMapOk = true;
                    enableControls(true);
                }
            }
        }

        public void setBtnApply(bool active)
        {   if (active)
                { btnApply.Text = "Apply Height Map"; }
            else
                { btnApply.Text = "Remove Height Map"; }
        }

        private void btnOffset_Click(object sender, EventArgs e)
        {
            if ((Map != null) && (cntReceived == cntSent))
            {
                Map.setZOffset(-Map.MaxHeight);
                showHightMapBMP(heightMapBMP, BMPsizeX, isgray);
            }
        }

        private void savePictureAsBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Map != null)
            {
                int sizeX = 640;
                int sizeY = Map.SizeY * sizeX / Map.SizeX;
                Bitmap tmpBMP = new Bitmap(sizeX, sizeY);
                createHightMapBMP(tmpBMP, sizeX, isgray);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Bitmap|*.bmp";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    tmpBMP.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                tmpBMP.Dispose();
            }
            else
                MessageBox.Show("No Height Map to save");
        }

        private void createHightMapBMP(Bitmap bmp, int sizeX, bool gray)
        { lblProgress.Text = "Finish t=" + elapsed.ToString(@"hh\:mm\:ss");
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
                    bmp.SetPixel(ix, iy, getColor(Map.MinHeight, Map.MaxHeight, z, gray));
                }
            }
        }

        private void showHightMapBMP(Bitmap bmp, int sizeX, bool gray)
        {
            createHightMapBMP(bmp, sizeX, gray);
            pictureBox1.Image = new Bitmap(bmp);
            pictureBox1.Refresh();
            lblMin.Text = string.Format("{0:0.000}", Map.MinHeight);
            lblMid.Text = string.Format("{0:0.000}", (Map.MinHeight+ Map.MaxHeight)/2);
            lblMax.Text = string.Format("{0:0.000}", Map.MaxHeight);
            pictureBox2.Image = new Bitmap(heightLegendBMP);
            pictureBox2.Refresh();
        }

        private bool isgray = false;
        private void cBGray_CheckedChanged(object sender, EventArgs e)
        {
            isgray = cBGray.Checked;
            for (int i = 0; i < 120; i++)
            {
                heightLegendBMP.SetPixel(0, 119 - i, getColor(0, 120, i, isgray));
                heightLegendBMP.SetPixel(1, 119 - i, getColor(0, 120, i, isgray));
            }
            pictureBox2.Image = new Bitmap(heightLegendBMP);
            pictureBox2.Refresh();
            if (Map != null)
                showHightMapBMP(heightMapBMP, BMPsizeX, isgray);
        }
        private Color getColor(double min, double max, double value, bool gray)
        {
            int R = 0, G = 0, B = 0;
            if (gray)
            {
                int valC = (int)(255 * (value - min) / (max - min));
                if (valC < 0) valC = 0;
                if (valC > 255 ) valC = 255 ;
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

        private void enableControls(bool enable)
        {   nUDX1.Enabled = enable; nUDX2.Enabled = enable;
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
        }

        public StringBuilder getCode
        { get { return scanCode; } }

        public ControlHeightMapForm()
        {
            InitializeComponent();
        }

        int cntReceived=0, cntSent=0;

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "HeightMap|*.map";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Map.Save(sfd.FileName);
            }
        }

        private void loadHeightMapToolStripMenuItem_Click(object sender, EventArgs e)
        {   btnLoad_Click(sender, e); }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "HeightMap|*.map";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Map = HeightMap.Load(sfd.FileName);
                lblXDim.Text = string.Format("X Min:{0} Max:{1} Step:{2}", Map.Min.X, Map.Max.X, Map.SizeX);
                lblYDim.Text = string.Format("Y Min:{0} Max:{1} Step:{2}", Map.Min.Y, Map.Max.Y, Map.SizeY);
                BMPsizeX = 240;
                BMPsizeY = Map.SizeY * BMPsizeX / Map.SizeX;
                heightMapBMP = new Bitmap(BMPsizeX, BMPsizeY);
                showHightMapBMP(heightMapBMP, BMPsizeX, isgray);
                isMapOk = true;
                enableControls(true);
            }
        }

        private void btnPosLL_Click(object sender, EventArgs e)
        {
            nUDX1.Value = (decimal)actualPosWorld.X;
            nUDY1.Value = (decimal)actualPosWorld.Y;
            nUDX2.Value = nUDDeltaX.Value + nUDX1.Value;
            nUDY2.Value = nUDDeltaY.Value + nUDY1.Value;
        }

        private void btnPosUR_Click(object sender, EventArgs e)
        {
            nUDX2.Value = (decimal)actualPosWorld.X;
            nUDY2.Value = (decimal)actualPosWorld.Y;
            nUDDeltaX.Value = nUDX2.Value - nUDX1.Value;
            nUDDeltaY.Value = nUDY2.Value - nUDY1.Value;
        }
        private void nUDDeltaX_ValueChanged(object sender, EventArgs e)
        {
            nUDX2.Value = nUDDeltaX.Value + nUDX1.Value;
            nUDY2.Value = nUDDeltaY.Value + nUDY1.Value;
        }
        private void nUDX1_ValueChanged(object sender, EventArgs e)
        {
            nUDX2.Value = nUDDeltaX.Value + nUDX1.Value;
            nUDY2.Value = nUDDeltaY.Value + nUDY1.Value;
        }
        private void nUDX2_ValueChanged(object sender, EventArgs e)
        {
            nUDDeltaX.Value = nUDX2.Value - nUDX1.Value;
            nUDDeltaY.Value = nUDY2.Value - nUDY1.Value;
        }

        private int BMPsizeX = 240;
        private int BMPsizeY = 160;
        public bool scanStarted = false;

        private void ControlHeightMapForm_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.locationImageForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(0, 0); }

            nUDDeltaX.Value = Properties.Settings.Default.heightMapX2 - Properties.Settings.Default.heightMapX1;
            nUDDeltaY.Value = Properties.Settings.Default.heightMapY2 - Properties.Settings.Default.heightMapY1;
            heightLegendBMP = new Bitmap(2, 120);
            for (int i = 0; i < 120; i++)
            {   heightLegendBMP.SetPixel(0, 119 - i, getColor(0, 120, i, isgray));
                heightLegendBMP.SetPixel(1, 119 - i, getColor(0, 120, i, isgray));
            }
            pictureBox2.Image = new Bitmap(heightLegendBMP);
            pictureBox2.Refresh();
            int stepX = (int)Math.Round((nUDX2.Value - nUDX1.Value) / nUDGridX.Value);
            int stepY = (int)Math.Round((nUDY2.Value - nUDY1.Value) / nUDGridY.Value);
            lblXDim.Text = string.Format("X Min:{0} Max:{1} Step:{2}", nUDX1.Value, nUDX2.Value, stepX);
            lblYDim.Text = string.Format("Y Min:{0} Max:{1} Step:{2}", nUDY1.Value, nUDY2.Value, stepY);
        }

        private TimeSpan elapsed;               //elapsed time from file burnin
        private DateTime timeInit;              //time start to burning file
        private void btnApply_Click(object sender, EventArgs e)
        {        }

        private void ControlHeightMapForm_FormClosing(object sender, FormClosingEventArgs e)
        {   Properties.Settings.Default.heightMapX2 = nUDX2.Value;
            Properties.Settings.Default.heightMapY2 = nUDY2.Value;
            Properties.Settings.Default.locationImageForm = Location;
        }

        private void btnStartHeightScan_Click(object sender, EventArgs e)
        {
            enableControls(scanStarted);
            if (!scanStarted)
            {
                isMapOk = false;
                timeInit = DateTime.UtcNow;
                elapsed = TimeSpan.Zero;
                btnStartHeightScan.Text = "STOP scanning Height Map";
                decimal x1, x2, y1, y2;
                x1 = Math.Min(nUDX1.Value, nUDX2.Value);
                x2 = Math.Max(nUDX1.Value, nUDX2.Value);
                y1 = Math.Min(nUDY1.Value, nUDY2.Value);
                y2 = Math.Max(nUDY1.Value, nUDY2.Value);
                if (x1 == x2) x2 = x1 + 10;
                if (y1 == y2) y2 = y1 + 10;
                nUDX1.Value = x1; nUDX2.Value = x2; nUDY1.Value = y1; nUDY2.Value = y2;
                decimal stepX = (x2 - x1) / (nUDGridX.Value - 1);
                decimal stepY = (y2 - y1) / (nUDGridY.Value - 1);
                cntSent = 0; cntReceived = 0;
                gcode.reduceGCode = true;   // reduce number format to #.# in gcode.frmtNum()

                Map = new HeightMap((double)nUDGridX.Value, new Vector2((double)x1, (double)y1), new Vector2((double)x2, (double)y2));
                MapIndex = new List<Point>();

                lblXDim.Text = string.Format("X Min:{0} Max:{1} Step:{2}", Map.Min.X, Map.Max.X, Map.SizeX);
                lblYDim.Text = string.Format("Y Min:{0} Max:{1} Step:{2}", Map.Min.Y, Map.Max.Y, Map.SizeY);

                textBox1.Clear();
                textBox1.Text += Map.SizeX.ToString() + "  " + Map.SizeY.ToString();
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
                scanCode.AppendFormat("G90F{0}\r\n", gcode.frmtNum((float)nUDProbeSpeed.Value));
                for (int iy = 0; iy < Map.SizeY; iy++)
                {
                    tmp = Map.GetCoordinates(0, iy);
                    scanCode.AppendFormat("G0Y{0}\r\n", gcode.frmtNum((float)tmp.Y));
                    pixY = iy * BMPsizeY / Map.SizeY;
                    for (int ix = 0; ix < Map.SizeX; ix++)
                    {
                        pixX = ix * BMPsizeX / Map.SizeX;
                        heightMapBMP.SetPixel(pixX, pixY, Color.FromArgb(255, 00, 00));
                        tmp = Map.GetCoordinates(ix, iy);
                        MapIndex.Add(new Point(ix, iy));
                        scanCode.AppendFormat("G0Z{0}\r\n", gcode.frmtNum((float)nUDProbeUp.Value));
                        scanCode.AppendFormat("X{0}\r\n", gcode.frmtNum((float)tmp.X));
                        scanCode.AppendFormat("G38.3Z{0}\r\n", gcode.frmtNum((float)nUDProbeDown.Value));
                        cntSent++;
                    }
                    if (iy < Map.SizeY - 1)
                    {
                        iy++;
                        tmp = Map.GetCoordinates(0, iy);
                        scanCode.AppendFormat("G0Y{0}\r\n", gcode.frmtNum((float)tmp.Y));
                        for (int ix = Map.SizeX - 1; ix >= 0; ix--)
                        {
                            pixX = ix * BMPsizeX / Map.SizeX;
                            heightMapBMP.SetPixel(pixX, pixY, Color.FromArgb(100, 100, 100));
                            tmp = Map.GetCoordinates(ix, iy);
                            MapIndex.Add(new Point(ix, iy));
                            scanCode.AppendFormat("G0Z{0}\r\n", gcode.frmtNum((float)nUDProbeUp.Value));
                            scanCode.AppendFormat("X{0}\r\n", gcode.frmtNum((float)tmp.X));
                            scanCode.AppendFormat("G38.3Z{0}\r\n", gcode.frmtNum((float)nUDProbeDown.Value));
                            cntSent++;
                        }
                    }
                }
                scanCode.AppendFormat("G0 Z{0}\r\n", gcode.frmtNum((float)nUDProbeUp.Value));
                tmp = Map.GetCoordinates(0, 0);
                scanCode.AppendFormat("G0 X{0} Y{1}\r\n", gcode.frmtNum((float)tmp.X), gcode.frmtNum((float)tmp.Y));

                textBox1.Text += "Code sent\r\n";// scanCode.ToString();
                progressBar1.Maximum = cntSent;
                lblProgress.Text = string.Format("{0}%", (100 * cntReceived / progressBar1.Maximum));
                pictureBox1.Image = new Bitmap(heightMapBMP);
                pictureBox1.Refresh();
            }
            else
                btnStartHeightScan.Text = "Generate Height Map";
            scanStarted = !scanStarted;
        }
    }

    public class HeightMap
    {
        public double?[,] Points { get; private set; }
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

//        public int Progress { get { return TotalPoints - NotProbed.Count; } }
        public int TotalPoints { get { return SizeX * SizeY; } }

        public Queue<Tuple<int, int>> NotProbed { get; private set; } = new Queue<Tuple<int, int>>();

        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }

        public Vector2 Delta { get { return Max - Min; } }

        public double MinHeight { get; set; } = double.MaxValue;
        public double MaxHeight { get; set; } = double.MinValue;

//        public event Action MapUpdated;

        public double GridX { get { return (Max.X - Min.X) / (SizeX - 1); } }
        public double GridY { get { return (Max.Y - Min.Y) / (SizeY - 1); } }


        public HeightMap(double gridSize, Vector2 min, Vector2 max)
        {
            MinHeight = double.MaxValue;
            MaxHeight = double.MinValue;

            if (min.X == max.X || min.Y == max.Y)
                throw new Exception("Height map can't be infinitely narrow");

            int pointsX = (int)Math.Ceiling((max.X - min.X) / gridSize) + 1;
            int pointsY = (int)Math.Ceiling((max.Y - min.Y) / gridSize) + 1;

            if (pointsX == 0 || pointsY == 0)
                throw new Exception("Height map must have at least 4 points");

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

        public double InterpolateZ(double x, double y)
        {
            if (x > Max.X || x < Min.X || y > Max.Y || y < Min.Y)
                return MaxHeight;

            x -= Min.X;
            y -= Min.Y;

            x /= GridX;
            y /= GridY;

            int iLX = (int)Math.Floor(x);   //lower integer part
            int iLY = (int)Math.Floor(y);

            int iHX = (int)Math.Ceiling(x); //upper integer part
            int iHY = (int)Math.Ceiling(y);

            double fX = x - iLX;             //fractional part
            double fY = y - iLY;

            double linUpper = Points[iHX, iHY].Value * fX + Points[iLX, iHY].Value * (1 - fX);       //linear immediates
            double linLower = Points[iHX, iLY].Value * fX + Points[iLX, iLY].Value * (1 - fX);

            return linUpper * fY + linLower * (1 - fY);     //bilinear result
        }

        public Vector2 GetCoordinates(int x, int y)
        {
            return new Vector2(x * (Delta.X / (SizeX - 1)) + Min.X, y * (Delta.Y / (SizeY - 1)) + Min.Y);
        }

        private HeightMap()
        {

        }

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
            return Points[x, y];
        }
        public void setZOffset(double offset)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = Points[ix, iy] + offset;
                }
            }
            MaxHeight = MaxHeight + offset;
            MinHeight = MinHeight + offset;
        }

        public static HeightMap Load(string path)
        {
            HeightMap map = new HeightMap();

            XmlReader r = XmlReader.Create(path);
            map.MaxHeight = double.MinValue;
            map.MinHeight = double.MaxValue;

            while (r.Read())
            {
                if (!r.IsStartElement())
                    continue;

                switch (r.Name)
                {
                    case "heightmap":
                        map.Min = new Vector2(double.Parse(r["MinX"]), double.Parse(r["MinY"]));
                        map.Max = new Vector2(double.Parse(r["MaxX"]), double.Parse(r["MaxY"]));
                        map.SizeX = int.Parse(r["SizeX"]);
                        map.SizeY = int.Parse(r["SizeY"]);
                        map.Points = new double?[map.SizeX, map.SizeY];
                        break;
                    case "point":
                        int x = int.Parse(r["X"]), y = int.Parse(r["Y"]);
                        double height = double.Parse(r.ReadInnerXml());

                        map.Points[x, y] = height;

                        if (height > map.MaxHeight)
                            map.MaxHeight = height;
                        if (height < map.MinHeight)
                            map.MinHeight = height;

                        break;
                }
            }

     //       r.Dispose();

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

            return map;
        }

        public void Save(string path)
        {
            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            XmlWriter w = XmlWriter.Create(path, set);
            w.WriteStartDocument();
            w.WriteStartElement("heightmap");
            w.WriteAttributeString("MinX", Min.X.ToString());
            w.WriteAttributeString("MinY", Min.Y.ToString());
            w.WriteAttributeString("MaxX", Max.X.ToString());
            w.WriteAttributeString("MaxY", Max.Y.ToString());
            w.WriteAttributeString("SizeX", SizeX.ToString());
            w.WriteAttributeString("SizeY", SizeY.ToString());

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    if (!Points[x, y].HasValue)
                        continue;

                    w.WriteStartElement("point");
                    w.WriteAttributeString("X", x.ToString());
                    w.WriteAttributeString("Y", y.ToString());
                    w.WriteString(Points[x, y].Value.ToString());
                    w.WriteEndElement();
                }
            }
            w.WriteEndElement();
            w.Close();
        }

        public void FillWithTestPattern(string pattern)
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
            // Check object other is a Vector3 object
            if (other is Vector2)
            {
                // Convert object to Vector3
                Vector2 otherVector = (Vector2)other;

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
