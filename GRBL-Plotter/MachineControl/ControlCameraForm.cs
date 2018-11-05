/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2018 Sven Hasemann contact: svenhb@web.de

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
    Thanks to http://code-bude.net/2011/06/02/webcam-benutzen-in-csharp/
*/
/*  2018-04-02  add shape recognition and code clean-up
 *  2017-01-01  check form-location and fix strange location
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Drawing.Imaging;

// AForge Library http://www.aforgenet.com/framework/
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge;
using AForge.Math.Geometry;
using System.Collections.Generic;

namespace GRBL_Plotter
{
    public partial class ControlCameraForm : Form
    {
        private byte cameraIndex = 0;
        private int cameraResolution = 640;
        private double cameraRotation = 180;
        private double cameraTeachRadiusTop = 30;
        private double cameraTeachRadiusBot = 10;
        private double cameraScalingTop = 20;
        private double cameraScalingBot = 20;
        private double cameraPosTop = 0;
        private double cameraPosBot = -50;
        xyPoint realPosition;

        private float cameraZoom = 1;
        private bool teachingTop = false;
        private bool teachingBot = false;
        private bool showOverlay = true;
        //private bool teachTP1 = false;

        private Color colText = Color.Lime;
        private Brush brushText = Brushes.Lime;
        private Color colCross = Color.Yellow;

        private xyzPoint actualPosWorld;
        private xyzPoint actualPosMachine;
        private xyPoint actualPosMarker;
        private xyPoint teachPoint1;
        private xyPoint teachPoint2;
        private xyPoint teachPoint3;
        private int coordG = 54;

        public xyzPoint setPosWorld
        { set { actualPosWorld = value; } }
        public xyzPoint setPosMachine
        { set { actualPosMachine = value; } }
        public void setPosMarker(xyPoint tmp)// double x, double y)
        { actualPosMarker= tmp; }
        public int setCoordG
        {   set { coordG = value;
                btnCamCoordTool.BackColor = SystemColors.Control;
                btnCamCoordCam.BackColor = SystemColors.Control;
                if (coordG == 54) { btnCamCoordTool.BackColor = Color.Lime; }
                else if (coordG == 59) { btnCamCoordCam.BackColor = Color.Lime; }
            }
        }


        public ControlCameraForm()
        {
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.language);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        VideoCaptureDevice videoSource;
        FilterInfoCollection videosources;
        // load settings, get list of video-sources, set video-source
        private void camera_form_Load(object sender, EventArgs e)
        {
            cameraIndex = Properties.Settings.Default.cameraindex;
            cameraRotation = Properties.Settings.Default.camerarotation;
            if (cameraRotation > 360)
                cameraRotation = 0;
            toolStripTextBox1.Text = String.Format("{0}", cameraRotation);
            cameraTeachRadiusTop = Properties.Settings.Default.cameraTeachRadiusTop;
            cameraTeachRadiusBot = Properties.Settings.Default.cameraTeachRadiusBot;
            toolStripTextBox2.Text = String.Format("{0}", cameraTeachRadiusTop);
            toolStripTextBox3.Text = String.Format("{0}", cameraTeachRadiusBot);
            cameraScalingTop = Properties.Settings.Default.camerascalingTop;
            cameraScalingBot = Properties.Settings.Default.camerascalingBot;
            cameraPosTop = Properties.Settings.Default.cameraPosTop;
            cameraPosBot = Properties.Settings.Default.cameraPosBot;

            colText = Properties.Settings.Default.cameraColorText;
            setButtonColors(textToolStripMenuItem, colText);
            brushText = new SolidBrush(colText);

            colCross = Properties.Settings.Default.cameraColorCross;
            setButtonColors(crossHairsToolStripMenuItem, colCross);
            pen1 = new Pen(colCross, 1f);

            penUp.Color = Properties.Settings.Default.colorPenUp;
            penDown.Color = Properties.Settings.Default.colorPenDown;
            penRuler.Color = Properties.Settings.Default.colorRuler;
            penTool.Color = Properties.Settings.Default.colorTool;
            penMarker.Color = Properties.Settings.Default.colorMarker;
            penRuler.Width = (float)Properties.Settings.Default.widthRuler;
            penUp.Width = (float)Properties.Settings.Default.widthPenUp;
            penDown.Width = (float)Properties.Settings.Default.widthPenDown;
            penTool.Width = (float)Properties.Settings.Default.widthTool;
            penMarker.Width = (float)Properties.Settings.Default.widthMarker;

            videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videosources != null)
            {
                ToolStripMenuItem[] items = new ToolStripMenuItem[videosources.Count]; // You would obviously calculate this value at runtime

                for (int i = 0; i < videosources.Count; i++)
                {
                    items[i] = new ToolStripMenuItem();
                    items[i].Name = "camselect" + i.ToString();
                    items[i].Tag = i;
                    items[i].Text = videosources[i].Name;
                    items[i].Checked = false;
                    items[i].Click += new EventHandler(camSourceSubmenuItem_Click);
                }
                if (videosources.Count > 0)
                {
                    if (cameraIndex >= videosources.Count)
                        cameraIndex = 0;
                    videoSource = new VideoCaptureDevice(videosources[cameraIndex].MonikerString);
                    camSourceToolStripMenuItem.DropDownItems.AddRange(items);
                    selectCameraSource(cameraIndex, cameraResolution);
                }
                else
                    MessageBox.Show("No camera source found. Close and open this window after connecting a camera.");
            }
            xmid = pictureBoxVideo.Size.Width / 2;
            ymid = pictureBoxVideo.Size.Height / 2;
            picCenter.X = xmid;
            picCenter.Y = ymid;
            ratio = (double)pictureBoxVideo.Size.Height / pictureBoxVideo.Size.Width;

            Location = Properties.Settings.Default.locationCamForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new System.Drawing.Point(0, 0); }

            fillComboBox(1, Properties.Settings.Default.camShapeSet1);
            fillComboBox(2, Properties.Settings.Default.camShapeSet2);
            fillComboBox(3, Properties.Settings.Default.camShapeSet3);
            fillComboBox(4, Properties.Settings.Default.camShapeSet4);
            //comboBox1.Text
        }
        // save settings
        private void camera_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.locationCamForm = Location;
            Properties.Settings.Default.cameraindex = cameraIndex;
            Properties.Settings.Default.camerarotation = cameraRotation;
            Properties.Settings.Default.camerascalingTop = cameraScalingTop;
            Properties.Settings.Default.camerascalingBot = cameraScalingBot;
            Properties.Settings.Default.cameraTeachRadiusTop = cameraTeachRadiusTop;
            Properties.Settings.Default.cameraTeachRadiusBot = cameraTeachRadiusBot;
            Properties.Settings.Default.cameraPosTop = cameraPosTop;
            Properties.Settings.Default.cameraPosBot = cameraPosBot;
  //          Properties.Settings.Default.cameraToolOffsetX = cameraToolOffsetX;
  //          Properties.Settings.Default.cameraToolOffsetY = cameraToolOffsetY;
            Properties.Settings.Default.Save();
        }
        // try to set video-source, set event-handler
        private void selectCameraSource(int index, int resolution)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource = null;
            }
            videoSource = new VideoCaptureDevice(videosources[index].MonikerString);
            ((ToolStripMenuItem)camSourceToolStripMenuItem.DropDownItems[index]).Checked = true;

            try
            {
                int i;
                if (videoSource.VideoCapabilities.Length > 0)
                {
                    for (i = 0; i < videoSource.VideoCapabilities.Length; i++)
                    {
                        if (videoSource.VideoCapabilities[i].FrameSize.Width == resolution)
                            break;
                    }
                    videoSource.VideoResolution = videoSource.VideoCapabilities[i];
                }
            }
            catch { }
            videoSource.NewFrame += new AForge.Video.NewFrameEventHandler(videoSource_NewFrame);
            videoSource.Start();
        }

        int frameCounter = 0;
        // event-handler of video - rotate image and display
        void videoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            frameCounter++;
            if (frameCounter > 10)
            {
                if (cBShapeDetection.Checked)
                    ProcessImage((Bitmap)RotateImage((Bitmap)eventArgs.Frame.Clone(), (float)cameraRotation, cameraZoom));
                else
                    pictureBoxVideo.BackgroundImage = RotateImage((Bitmap)eventArgs.Frame.Clone(), (float)cameraRotation, cameraZoom);
                frameCounter = 0;
            }
        }
        // rotate image from: http://code-bude.net/2011/07/12/bilder-rotieren-mit-csharp-bitmap-rotateflip-vs-gdi-graphics/
        public static System.Drawing.Image RotateImage(System.Drawing.Image img, float rotationAngle, float zoom)
        {
            using (Graphics gfx = Graphics.FromImage(img))
            {
                gfx.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);
                gfx.RotateTransform(rotationAngle);
                gfx.ScaleTransform(zoom, zoom);
                gfx.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);
                gfx.DrawImage(img, new System.Drawing.Point(0, 0));
            }
            return img;
        }

        private void camera_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource = null;
            }
        }

        // onPaint event
        private Pen penTeach = new Pen(Color.LightPink, 0.5F);
        private Pen penUp = new Pen(Color.Green, 0.1F);
        private Pen penDown = new Pen(Color.Red, 0.4F);
        private Pen penRuler = new Pen(Color.Blue, 0.1F);
        private Pen penTool = new Pen(Color.Black, 0.5F);
        private Pen penMarker = new Pen(Color.DeepPink, 1F);
        private int xmid;// = pictureBoxVideo.Size.Width / 2;
        private int ymid;// = pictureBoxVideo.Size.Height / 2;
        private int radius;// = (int)(cameraTeachRadius * cameraZoom * xmid / cameraScaling);
        private double ratio;// = (double)pictureBoxVideo.Size.Height / pictureBoxVideo.Size.Width;
        private float angle = 0;
        private Pen pen1 = new Pen(Color.Yellow, 1f);
        private void pictureBoxVideo_Paint(object sender, PaintEventArgs e)
        {
            double diff = cameraPosTop - cameraPosBot;
            if (diff == 0) diff = 1;
            double m = (cameraScalingTop - cameraScalingBot) / diff;
            double n = cameraScalingTop - m * cameraPosTop;
            float actualScaling = (float)Math.Abs(actualPosMachine.Z * m + n);
            if (actualScaling < 1) actualScaling = 5;

            int gap = 5;
            int chlen = 200;
            e.Graphics.DrawLine(pen1, new System.Drawing.Point(xmid - chlen, ymid), new System.Drawing.Point(xmid - gap, ymid));
            e.Graphics.DrawLine(pen1, new System.Drawing.Point(xmid + gap, ymid), new System.Drawing.Point(xmid + chlen, ymid));
            e.Graphics.DrawLine(pen1, new System.Drawing.Point(xmid, ymid - chlen), new System.Drawing.Point(xmid, ymid - gap));
            e.Graphics.DrawLine(pen1, new System.Drawing.Point(xmid, ymid + gap), new System.Drawing.Point(xmid, ymid + chlen));

            realPosition.X = 2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).X) / pictureBoxVideo.Size.Width - 0.5) * actualScaling / cameraZoom;
            realPosition.Y = -2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).Y) / pictureBoxVideo.Size.Height - 0.5) * actualScaling * ratio / cameraZoom;
            int stringposY = -40;
            if (realPosition.Y > 0) stringposY = 10;
            System.Drawing.Point stringpos = new System.Drawing.Point(pictureBoxVideo.PointToClient(MousePosition).X - 60, pictureBoxVideo.PointToClient(MousePosition).Y + stringposY);
            if (teachingTop)
            {
                radius = (int)(cameraTeachRadiusTop * cameraZoom * xmid / actualScaling);
                e.Graphics.DrawEllipse(pen1, new Rectangle(xmid - radius, ymid - radius, 2 * radius, 2 * radius));
                string txt = "Next click will teach scaling\r\n of camera view to " + cameraTeachRadiusTop.ToString();
                showLabel(txt, stringpos, e.Graphics);
            }
            else if (teachingBot)
            {
                radius = (int)(cameraTeachRadiusBot * cameraZoom * xmid / actualScaling);
                e.Graphics.DrawEllipse(pen1, new Rectangle(xmid - radius, ymid - radius, 2 * radius, 2 * radius));
                string txt = "Next click will teach scaling\r\n of camera view to " + cameraTeachRadiusBot.ToString();
                showLabel(txt, stringpos, e.Graphics);
            }
            else if (measureAngle)
            {
                measureAngleStop = (xyPoint)pictureBoxVideo.PointToClient(MousePosition);
                e.Graphics.DrawLine(pen1, measureAngleStart.ToPoint(), measureAngleStop.ToPoint());
                angle = (float)measureAngleStart.AngleTo(measureAngleStop);
                string txt = String.Format("Angle: {0:0.00}", angle);
                showLabel(txt, stringpos, e.Graphics);
            }
            else
            {
                xyPoint absolute = (xyPoint)actualPosWorld + realPosition;
                string txt = String.Format("Relative {0:0.00} ; {1:0.00}\r\nAbsolute {2:0.00} ; {3:0.00}", realPosition.X, realPosition.Y, absolute.X, absolute.Y);
                showLabel(txt, stringpos, e.Graphics);
            }
/*            if (teachTP1)
            {
                angle = (float)measureAngleStart.AngleTo(measureAngleStop);
                e.Graphics.DrawString(String.Format("Angle: {0:0.00}", angle), new Font("Microsoft Sans Serif", 8), brushText, stringpos);
            }*/
            float scale = (float)(xmid / actualScaling * cameraZoom);
            e.Graphics.ScaleTransform(scale, -scale);
            float offX = (float)(xmid / scale - actualPosWorld.X);
            float offY = (float)(ymid / scale + actualPosWorld.Y);
            e.Graphics.TranslateTransform(offX, -offY);       // apply offset
            // show drawing from MainForm (static members of class GCodeVisualization)
            if (showOverlay)
            {
                e.Graphics.DrawPath(penRuler, GCodeVisuAndTransform.pathRuler);
                e.Graphics.DrawPath(penMarker, GCodeVisuAndTransform.pathMarker);
                e.Graphics.DrawPath(penDown, GCodeVisuAndTransform.pathPenDown);
                e.Graphics.DrawPath(penUp, GCodeVisuAndTransform.pathPenUp);
                //           e.Graphics.DrawEllipse(penTeach, new Rectangle((int)actualPosMarker.X-2, (int)actualPosMarker.Y - 2, (int)actualPosMarker.X + 2, (int)actualPosMarker.Y + 2));
            }
        }
        private void showLabel(string txt, System.Drawing.Point stringpos, Graphics graph)
        {
            Font fnt = new Font("Lucida Console", 8);
            var size = graph.MeasureString(txt, fnt);
            var rect = new RectangleF(stringpos.X-2, stringpos.Y-2, size.Width+1, size.Height+1);
            graph.FillRectangle(Brushes.White, rect);           //Filling a rectangle before drawing the string.
            graph.DrawString(txt, fnt, brushText, stringpos);
        }
        // Calculate click position and send coordinates via event
        private void pictureBoxVideo_Click(object sender, MouseEventArgs e)//, EventArgs e)
        {
            double relposx = 2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).X) / pictureBoxVideo.Size.Width - 0.5);
            double relposy = -2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).Y) / pictureBoxVideo.Size.Height - 0.5);
            if (teachingTop)
            {
                teachingTop = false;
                double radius = Math.Sqrt(relposx * relposx + relposy * relposy);
                cameraScalingTop = cameraTeachRadiusTop / radius;
                cameraPosTop = actualPosMachine.Z;
            }
            else if (teachingBot)
            {
                teachingBot = false;
                double radius = Math.Sqrt(relposx * relposx + relposy * relposy);
                cameraScalingBot = cameraTeachRadiusBot / radius;
                cameraPosBot = actualPosMachine.Z;
            }
            else if ((e.Button == MouseButtons.Left) && !measureAngle)
                OnRaiseXYEvent(new XYEventArgs(0, 1, realPosition, "G91")); // move relative and slow
            pictureBoxVideo.Invalidate();
        }


        public event EventHandler<XYEventArgs> RaiseXYEvent;
        protected virtual void OnRaiseXYEvent(XYEventArgs e)
        {
            EventHandler<XYEventArgs> handler = RaiseXYEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        // select video source from list
        private void camSourceSubmenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            ((ToolStripMenuItem)camSourceToolStripMenuItem.DropDownItems[cameraIndex]).Checked = false;
            cameraIndex = (byte)clickedItem.Tag;
            clickedItem.Checked = true;
            selectCameraSource(cameraIndex, cameraResolution);
        }
        // change zooming
        private void nUDCameraZoom_ValueChanged(object sender, EventArgs e)
        {
            cameraZoom = (float)nUDCameraZoom.Value;
        }
        // activate teaching of camera view range upper position
        private void upperPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            teachingTop = true;
            teachingBot = false;
        }
        // activate teaching of camera view range lower position
        private void lowerPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            teachingBot = true;
            teachingTop = false;
        }
        // get values of teach range textbox
        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            double old = cameraTeachRadiusTop;
            if (!Double.TryParse(toolStripTextBox2.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraTeachRadiusTop))
                cameraTeachRadiusTop = old;
            if (cameraTeachRadiusTop <= 0)
                cameraTeachRadiusTop = 20;
            //            toolStripTextBox2.Text = cameraTeachRadiusTop.ToString();
        }
        private void toolStripTextBox2_Leave(object sender, EventArgs e)
        {   toolStripTextBox2.Text = cameraTeachRadiusBot.ToString();
        }
        // get values of teach range textbox
        private void toolStripTextBox3_TextChanged(object sender, EventArgs e)
        {
            double old = cameraTeachRadiusBot;
            if (!Double.TryParse(toolStripTextBox3.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraTeachRadiusBot))
                cameraTeachRadiusBot = old;
            if (cameraTeachRadiusBot <= 0)
                cameraTeachRadiusBot = 10;
            //           toolStripTextBox3.Text = cameraTeachRadiusBot.ToString();
        }
        private void toolStripTextBox3_Leave(object sender, EventArgs e)
        {   toolStripTextBox3.Text = cameraTeachRadiusBot.ToString();
        }


        // send event to teach Zero offset
        private void btnSetOffsetZero_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(0,0, 0, "G92")); // set new coordinates
        }
        // send event to teach Marker offset
        private void btnSetOffsetMarker_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(0, 1, actualPosMarker, "G92"));        // set new coordinates
        }
        // sent event to apply offset
        private void btnCamCoordTool_Click(object sender, EventArgs e)
        {   if (cBCamCoordMove.Checked)
                OnRaiseXYEvent(new XYEventArgs(0, 1, (xyPoint)actualPosWorld, "G54; G0G90"));  // switch coord system and move
            else
                OnRaiseXYEvent(new XYEventArgs(0, 1, (xyPoint)actualPosWorld, "G54"));         // only switch
            btnCamCoordTool.BackColor = Color.Lime;
            btnCamCoordCam.BackColor = SystemColors.Control;
        }
        // sent event to apply offset
        private void btnCamCoordCam_Click(object sender, EventArgs e)
        {   if (cBCamCoordMove.Checked)
                OnRaiseXYEvent(new XYEventArgs(0, 1, (xyPoint)actualPosWorld, "G59; G0G90"));  // switch coord system and move
            else
                OnRaiseXYEvent(new XYEventArgs(0, 1, (xyPoint)actualPosWorld, "G59"));         // only switch
            btnCamCoordCam.BackColor = Color.Lime;
            btnCamCoordTool.BackColor = SystemColors.Control;
        }
        // show actual offset from tool position
        private void teachToolStripMenuItem_Click(object sender, EventArgs e)       // teach offset of G59 coord system
        {   OnRaiseXYEvent(new XYEventArgs(0, 1, (xyPoint)actualPosWorld, "G10 L2 P6 "));   // move relative and fast
        }

        // measure angle 
        private bool measureAngle = false;
        private xyPoint measureAngleStart = new xyPoint(0, 0);
        private xyPoint measureAngleStop = new xyPoint(0, 0);
        private void pictureBoxVideo_MouseDown(object sender, MouseEventArgs e)
        {
            //cmsPictureBox.Visible = true;
            if (e.Button == MouseButtons.Right)
            {
                measureAngleStart = (xyPoint)pictureBoxVideo.PointToClient(MousePosition);
                measureAngle = true;
            }
            if (e.Delta > 0)
            {
                nUDCameraZoom.Value += nUDCameraZoom.Increment;
                MessageBox.Show(e.Delta.ToString());
            }
            if (e.Delta < 0)
                nUDCameraZoom.Value-= nUDCameraZoom.Increment;
        }

        private void pictureBoxVideo_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {   if (nUDCameraZoom.Value < nUDCameraZoom.Maximum)
                    nUDCameraZoom.Value += nUDCameraZoom.Increment;
            }
            if (e.Delta < 0)
            {   if (nUDCameraZoom.Value > nUDCameraZoom.Minimum)
                    nUDCameraZoom.Value -= nUDCameraZoom.Increment;
            }
        }

        private void pictureBoxVideo_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                measureAngleStop = (xyPoint)pictureBoxVideo.PointToClient(MousePosition);
                angle = (float)measureAngleStart.AngleTo(measureAngleStop);
                measureAngle = false;
                lblAngle.Text = String.Format("{0:0.00}°", angle);
                if (angle != 0)
                    cmsPictureBox.Visible = false;
                else
                    cmsPictureBox.Visible = true;
            }
        }

        private void btnApplyAngle_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(angle, 1, new xyPoint(0,0), "a"));
        }
        // change camera rotation
        private void toolStripTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!double.TryParse(toolStripTextBox1.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraRotation))
                    cameraRotation = 0;
                toolStripTextBox1.Text = cameraRotation.ToString();
            }
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            applyColor(textToolStripMenuItem, "cameraColorText");
            brushText = new SolidBrush(Properties.Settings.Default.cameraColorText);
        }
        private void crossHairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            applyColor(crossHairsToolStripMenuItem, "cameraColorCross");
            colCross = Properties.Settings.Default.cameraColorCross;
            pen1 = new Pen(colCross, 1f);
        }

        private void applyColor(ToolStripMenuItem btn, string settings)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                setButtonColors(btn, colorDialog1.Color);
                Properties.Settings.Default[settings] = colorDialog1.Color;
                //                saveSettings();
            }
        }
        private void setButtonColors(ToolStripMenuItem btn, Color col)
        {
            btn.BackColor = col;
            btn.ForeColor = ContrastColor(col);
        }
        private Color ContrastColor(Color color)
        {
            int d = 0;
            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }

        // Process image
        private DoublePoint shapeCenter, picCenter;
        private bool shapeFound = false;
        private void ProcessImage(Bitmap bitmap)
        {
            // lock image
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // step 1 - turn background to black
            ColorFiltering colorFilter = new ColorFiltering();

            colorFilter.Red = new IntRange(Properties.Settings.Default.camFilterRed1, Properties.Settings.Default.camFilterRed2);
            colorFilter.Green = new IntRange(Properties.Settings.Default.camFilterGreen1, Properties.Settings.Default.camFilterGreen2);
            colorFilter.Blue = new IntRange(Properties.Settings.Default.camFilterBlue1, Properties.Settings.Default.camFilterBlue2);
            colorFilter.FillOutsideRange = Properties.Settings.Default.camFilterOutside;

            colorFilter.ApplyInPlace(bitmapData);

            // step 2 - locating objects
            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = (int)Properties.Settings.Default.camShapeSizeMin * (int)cameraZoom;
            blobCounter.MinWidth = (int)Properties.Settings.Default.camShapeSizeMin * (int)cameraZoom;
            blobCounter.MaxHeight = (int)Properties.Settings.Default.camShapeSizeMax * (int)cameraZoom;
            blobCounter.MaxWidth = (int)Properties.Settings.Default.camShapeSizeMax * (int)cameraZoom;

            blobCounter.ProcessImage(bitmapData);

            Blob[] blobs = blobCounter.GetObjectsInformation();
            bitmap.UnlockBits(bitmapData);

            // step 3 - check objects' type and highlight
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
            shapeChecker.MinAcceptableDistortion = (float)Properties.Settings.Default.camShapeDist;
            shapeChecker.RelativeDistortionLimit = (float)Properties.Settings.Default.camShapeDistMax;

            Graphics g = Graphics.FromImage(bitmap);
            Pen yellowPen = new Pen(Color.Yellow, 5); // circles
            Pen redPen = new Pen(Color.Red, 10); // circles
            Pen greenPen = new Pen(Color.Green, 5);   // known triangle

            double lowestDistance = xmid;
            double distance;
            shapeFound = false;
            AForge.Point center;
            double shapeRadius = 1;
            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                System.Single radius;
                // is circle ?
                //          g.DrawPolygon(greenPen, ToPointsArray(edgePoints));

                if (Properties.Settings.Default.camShapeCircle && shapeChecker.IsCircle(edgePoints, out center, out radius))
                {
                    shapeFound = true;
                    distance = center.DistanceTo((AForge.Point)picCenter);
                    g.DrawEllipse(yellowPen,
                        (float)(shapeCenter.X - shapeRadius), (float)(shapeCenter.Y - shapeRadius),
                        (float)(shapeRadius * 2), (float)(shapeRadius * 2));

                    if (lowestDistance > Math.Abs(distance))
                    {
                        lowestDistance = Math.Abs(distance);
                        shapeCenter = center;
                        shapeRadius = radius;
                    }
                }
                List<IntPoint> corners;
                if (Properties.Settings.Default.camShapeRect && shapeChecker.IsQuadrilateral(edgePoints, out corners))  //.IsConvexPolygon
                {
                    IntPoint minxy, maxxy, centxy;
                    shapeFound = true;
                    PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);
                    g.DrawPolygon(yellowPen, ToPointsArray(corners));
                    PointsCloud.GetBoundingRectangle(corners, out minxy, out maxxy);
                    centxy = (minxy + maxxy) / 2;
                    distance = picCenter.DistanceTo(centxy);// PointsCloud.GetCenterOfGravity(corners));
                    if (lowestDistance > Math.Abs(distance))
                    {
                        lowestDistance = Math.Abs(distance);
                        shapeCenter = centxy;// PointsCloud.GetCenterOfGravity(corners);
                        shapeRadius = maxxy.DistanceTo(minxy) / 2;// 50;
                    }
                }

            }
            if (shapeFound)
            {
                g.DrawEllipse(redPen,
               (float)(shapeCenter.X - shapeRadius * 1.2), (float)(shapeCenter.Y - shapeRadius * 1.2),
               (float)(shapeRadius * 2.4), (float)(shapeRadius * 2.4));
            }

            yellowPen.Dispose();
            redPen.Dispose();
            greenPen.Dispose();
            g.Dispose();

            // put new image to clipboard
            //           Clipboard.SetDataObject(bitmap);
            // and to picture box
            pictureBoxVideo.BackgroundImage = bitmap;
            //UpdatePictureBoxPosition();

        }
        // Convert list of AForge.NET's points to array of .NET points
        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];
            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }
            return array;
        }

        private void cBShapeDetection_CheckedChanged(object sender, EventArgs e)
        {
            if (cBShapeDetection.Checked)
                btnAutoCenter.Enabled = true;
            else
                btnAutoCenter.Enabled = false;
        }
        
        private void btnAutoCenter_Click(object sender, EventArgs e)
        {
            if (shapeFound)
            {
                float actualScaling = getActualScaling();
                xyPoint tmp;
                tmp.X = 2 * (Convert.ToDouble(shapeCenter.X) / pictureBoxVideo.Size.Width - 0.5) * actualScaling / cameraZoom;
                tmp.Y = -2 * (Convert.ToDouble(shapeCenter.Y) / pictureBoxVideo.Size.Height - 0.5) * actualScaling * ratio / cameraZoom;
                OnRaiseXYEvent(new XYEventArgs(0, 1, tmp/2, "G91")); // move relative and slow
                                                                      //               MessageBox.Show(x.ToString() + "  " + y.ToString()+"\r\n"+ shapeCenter.X.ToString()+"  "+ shapeCenter.Y.ToString());
                lblCenterPos.Text = string.Format("X: {0:0.000}  Y: {1:0.000}", tmp.X, tmp.Y);
            }
            else
                lblCenterPos.Text = "No shape found";
        }

        private float getActualScaling()
        {
            double diff = cameraPosTop - cameraPosBot;
            if (diff == 0) diff = 1;
            double m = (cameraScalingTop - cameraScalingBot) / diff;
            double n = cameraScalingTop - m * cameraPosTop;
            float actualScaling = (float)Math.Abs(actualPosMachine.Z * m + n);
            if (actualScaling < 1) actualScaling = 1;
            return actualScaling;
        }

        private void shapeSetLoad(string txt)
        {
            string[] value = txt.Split('|');
            int i = 1;
            try
            {
                Properties.Settings.Default.camFilterRed1 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterRed2 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterGreen1 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterGreen2 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterBlue1 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterBlue2 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterOutside = (value[i++] == "True") ? true : false;
                Properties.Settings.Default.camShapeCircle = (value[i++] == "True") ? true : false;
                Properties.Settings.Default.camShapeRect = (value[i++] == "True") ? true : false;
                Properties.Settings.Default.camShapeSizeMin = Convert.ToDecimal(value[i++]);
                Properties.Settings.Default.camShapeSizeMax = Convert.ToDecimal(value[i++]);
                Properties.Settings.Default.camShapeDist = Convert.ToDecimal(value[i++]);
                Properties.Settings.Default.camShapeDistMax = Convert.ToDecimal(value[i++]);
                Properties.Settings.Default.Save();
            }
            catch { }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = comboBox1.SelectedIndex;
            if (selectedIndex == 0) { shapeSetLoad(Properties.Settings.Default.camShapeSet1); }
            if (selectedIndex == 1) { shapeSetLoad(Properties.Settings.Default.camShapeSet2); }
            if (selectedIndex == 2) { shapeSetLoad(Properties.Settings.Default.camShapeSet3); }
            if (selectedIndex == 3) { shapeSetLoad(Properties.Settings.Default.camShapeSet4); }
        }

        private void teachpoint1_process_Click(object sender, EventArgs e)
        {   teachPoint1 = actualPosMarker;
            teachPoint2 = teachPoint1; teachPoint3 = teachPoint1;
            measureAngleStart = teachPoint1;
            OnRaiseXYEvent(new XYEventArgs(0, 1, actualPosMarker, "G92"));        // set new coordinates
            //teachTP1 = true;
        }

        private void teachpoint2_process_Click(object sender, EventArgs e)
        {
            teachPoint2 = actualPosMarker;
            double angle1 = teachPoint1.AngleTo(teachPoint2);
            double dist1  = teachPoint1.DistanceTo(teachPoint2);
            double angle2 = teachPoint1.AngleTo((xyPoint)actualPosWorld);
            double dist2 = teachPoint1.DistanceTo((xyPoint)actualPosWorld);
            double angle = angle1 - angle2;
            lblAngle.Text = String.Format("{0:0.00}°", angle);

            OnRaiseXYEvent(new XYEventArgs(angle, dist2/dist1, teachPoint1, "a"));       // rotate arround TP1
            //teachTP1 = false;
        }

        private void ControlCameraForm_KeyDown(object sender, KeyEventArgs e)
        {   if (e.KeyCode == Keys.Space)
            { showOverlay = false; } }

        private void ControlCameraForm_KeyUp(object sender, KeyEventArgs e)
        {   if (e.KeyCode == Keys.Space)
            { showOverlay = true; } }

        private void showOverlayGraphicsToolStripMenuItem_Click(object sender, EventArgs e)
        {   showOverlay = showOverlay ? false : true;
            showOverlayGraphicsToolStripMenuItem.Checked = showOverlay;
        }

        private void pictureBoxVideo_MouseEnter(object sender, EventArgs e)
        {
            if (!pictureBoxVideo.Focused)
                pictureBoxVideo.Focus();      
        }

        private void pictureBoxVideo_MouseLeave(object sender, EventArgs e)
        {
            if (pictureBoxVideo.Focused)
                pictureBoxVideo.Parent.Focus();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string txt="";
            MessageBox.Show(txt);
        }

        private void fillComboBox(int index, string txt)
        {
            if (index == 1) { comboBox1.Items[0] = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (index == 2) { comboBox1.Items[1] = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (index == 3) { comboBox1.Items[2] = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (index == 4) { comboBox1.Items[3] = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
        }
    }
}
