/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2021 Sven Hasemann contact: svenhb@web.de

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
/* 2018-04-02 add shape recognition and code clean-up
 * 2017-01-01 check form-location and fix strange location
 * 2019-02-05 switch to global variables grbl.posWork
 * 2019-04-17 Line 391, 393 Convert.ToByte by Rob Zeilinga
 * 2019-08-15 add logger
 * 2019-10-25 remove icon to reduce resx size, load icon on run-time
 * 2021-01-22 hide pathPenUp, if set
 * 2021-07-02 code clean up / code quality
*/

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
// AForge Library http://www.aforgenet.com/framework/
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

//#pragma warning disable CA1303	// Do not pass literals as localized parameters
//#pragma warning disable CA1305

namespace GrblPlotter
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
        XyPoint realPosition;

        private float cameraZoom = 1;
        private bool teachingTop = false;
        private bool teachingBot = false;
        private bool showOverlay = true;

        private Color colText = Color.Lime;
        private Brush brushText = Brushes.Lime;
        private Color colCross = Color.Yellow;

        private XyPoint teachPoint1;
        private XyPoint teachPoint2;
   //     private XyPoint teachPoint3;
        private int coordG = 54;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

        public int SetCoordG
        {
            set
            {
                coordG = value;
                btnCamCoordTool.BackColor = SystemColors.Control;
                btnCamCoordCam.BackColor = SystemColors.Control;
                if (coordG == 54) { btnCamCoordTool.BackColor = Color.Lime; }
                else if (coordG == 59) { btnCamCoordCam.BackColor = Color.Lime; }
            }
            get { return coordG; }
        }

        public ControlCameraForm()
        {
            Logger.Trace("++++++ ControlCameraForm START ++++++");
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        VideoCaptureDevice videoSource;
        FilterInfoCollection videosources;
        // load settings, get list of video-sources, set video-source
        private void Camera_form_Load(object sender, EventArgs e)
        {
            cameraIndex = Properties.Settings.Default.cameraIndex;
            cameraRotation = Properties.Settings.Default.cameraRotation;
            if (cameraRotation > 360)
                cameraRotation = 0;
            toolStripTextBox1.Text = String.Format(culture, "{0}", cameraRotation);
            cameraTeachRadiusTop = Properties.Settings.Default.cameraTeachRadiusTop;
            cameraTeachRadiusBot = Properties.Settings.Default.cameraTeachRadiusBot;
            toolStripTextBox2.Text = String.Format(culture, "{0}", cameraTeachRadiusTop);
            toolStripTextBox3.Text = String.Format(culture, "{0}", cameraTeachRadiusBot);
            cameraScalingTop = Properties.Settings.Default.cameraScalingTop;
            cameraScalingBot = Properties.Settings.Default.cameraScalingBot;
            cameraPosTop = Properties.Settings.Default.cameraPosTop;
            cameraPosBot = Properties.Settings.Default.cameraPosBot;

            colText = Properties.Settings.Default.cameraColorText;
            SetButtonColors(textToolStripMenuItem, colText);
            brushText = new SolidBrush(colText);

            colCross = Properties.Settings.Default.cameraColorCross;
            SetButtonColors(crossHairsToolStripMenuItem, colCross);
            pen1 = new Pen(colCross, 1f);

            penUp.Color = Properties.Settings.Default.gui2DColorPenUp;
            penDown.Color = Properties.Settings.Default.gui2DColorPenDown;
            penRuler.Color = Properties.Settings.Default.gui2DColorRuler;
            penTool.Color = Properties.Settings.Default.gui2DColorTool;
            penMarker.Color = Properties.Settings.Default.gui2DColorMarker;
            penRuler.Width = (float)Properties.Settings.Default.gui2DWidthRuler;
            penUp.Width = (float)Properties.Settings.Default.gui2DWidthPenUp;
            penDown.Width = (float)Properties.Settings.Default.gui2DWidthPenDown;
            penTool.Width = (float)Properties.Settings.Default.gui2DWidthTool;
            penMarker.Width = (float)Properties.Settings.Default.gui2DWidthMarker;

            videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videosources != null)
            {
                ToolStripMenuItem[] items = new ToolStripMenuItem[videosources.Count]; // You would obviously calculate this value at runtime

                for (int i = 0; i < videosources.Count; i++)
                {
                    items[i] = new ToolStripMenuItem
                    {
                        Name = "camselect" + i.ToString(culture),
                        Tag = i,
                        Text = videosources[i].Name,
                        Checked = false
                    };
                    items[i].Click += new EventHandler(CamSourceSubmenuItem_Click);
                }
                if (videosources.Count > 0)
                {
                    if (cameraIndex >= videosources.Count)
                        cameraIndex = 0;
                    videoSource = new VideoCaptureDevice(videosources[cameraIndex].MonikerString);
                    camSourceToolStripMenuItem.DropDownItems.AddRange(items);
                    SelectCameraSource(cameraIndex, cameraResolution);
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
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }

            FillComboBox(1, Properties.Settings.Default.camShapeSet1);
            FillComboBox(2, Properties.Settings.Default.camShapeSet2);
            FillComboBox(3, Properties.Settings.Default.camShapeSet3);
            FillComboBox(4, Properties.Settings.Default.camShapeSet4);
            //comboBox1.Text
        }
        // save settings
        private void Camera_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.locationCamForm = Location;
            Properties.Settings.Default.cameraIndex = cameraIndex;
            Properties.Settings.Default.cameraRotation = cameraRotation;
            Properties.Settings.Default.cameraScalingTop = cameraScalingTop;
            Properties.Settings.Default.cameraScalingBot = cameraScalingBot;
            Properties.Settings.Default.cameraTeachRadiusTop = cameraTeachRadiusTop;
            Properties.Settings.Default.cameraTeachRadiusBot = cameraTeachRadiusBot;
            Properties.Settings.Default.cameraPosTop = cameraPosTop;
            Properties.Settings.Default.cameraPosBot = cameraPosBot;
            Properties.Settings.Default.Save();
            Logger.Trace("++++++ ControlCameraForm Stop ++++++");
        }
        // try to set video-source, set event-handler
        private void SelectCameraSource(int index, int resolution)
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
            catch (Exception ex){ Logger.Error(ex, "SelectCameraSource "); throw; }
            videoSource.NewFrame += new AForge.Video.NewFrameEventHandler(VideoSource_NewFrame);
            videoSource.Start();
        }

        int frameCounter = 0;
        // event-handler of video - rotate image and display
        void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
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
            if (img != null)
            {
                using (Graphics gfx = Graphics.FromImage(img))
                {
                    gfx.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);
                    gfx.RotateTransform(rotationAngle);
                    gfx.ScaleTransform(zoom, zoom);
                    gfx.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);
                    gfx.DrawImage(img, new System.Drawing.Point(0, 0));
                }
            }
            return img;
        }

        private void Camera_form_FormClosed(object sender, FormClosedEventArgs e)
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
        private void PictureBoxVideo_Paint(object sender, PaintEventArgs e)
        {
            double diff = cameraPosTop - cameraPosBot;
            if (diff == 0) diff = 1;
            double m = (cameraScalingTop - cameraScalingBot) / diff;
            double n = cameraScalingTop - m * cameraPosTop;
            float actualScaling = (float)Math.Abs(Grbl.posMachine.Z * m + n);
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
                string txt = "Next click will teach scaling\r\n of camera view to " + cameraTeachRadiusTop.ToString(culture);
                ShowLabel(txt, stringpos, e.Graphics);
            }
            else if (teachingBot)
            {
                radius = (int)(cameraTeachRadiusBot * cameraZoom * xmid / actualScaling);
                e.Graphics.DrawEllipse(pen1, new Rectangle(xmid - radius, ymid - radius, 2 * radius, 2 * radius));
                string txt = "Next click will teach scaling\r\n of camera view to " + cameraTeachRadiusBot.ToString(culture);
                ShowLabel(txt, stringpos, e.Graphics);
            }
            else if (measureAngle)
            {
                measureAngleStop = (XyPoint)pictureBoxVideo.PointToClient(MousePosition);
                e.Graphics.DrawLine(pen1, measureAngleStart.ToPoint(), measureAngleStop.ToPoint());
                angle = (float)measureAngleStart.AngleTo(measureAngleStop);
                string txt = String.Format(culture, "Angle: {0:0.00}", angle);
                ShowLabel(txt, stringpos, e.Graphics);
            }
            else
            {
                XyPoint absolute = (XyPoint)Grbl.posWork + realPosition;
                string txt = String.Format(culture, "Relative {0:0.00} ; {1:0.00}\r\nAbsolute {2:0.00} ; {3:0.00}", realPosition.X, realPosition.Y, absolute.X, absolute.Y);
                ShowLabel(txt, stringpos, e.Graphics);
            }
            /*            if (teachTP1)
                        {
                            angle = (float)measureAngleStart.AngleTo(measureAngleStop);
                            e.Graphics.DrawString(String.Format("Angle: {0:0.00}", angle), new Font("Microsoft Sans Serif", 8), brushText, stringpos);
                        }*/
            float scale = (float)(xmid / actualScaling * cameraZoom);
            e.Graphics.ScaleTransform(scale, -scale);
            float offX = (float)(xmid / scale - Grbl.posWork.X);
            float offY = (float)(ymid / scale + Grbl.posWork.Y);
            e.Graphics.TranslateTransform(offX, -offY);       // apply offset
            // show drawing from MainForm (static members of class GCodeVisualization)
            if (showOverlay)
            {
                e.Graphics.DrawPath(penRuler, VisuGCode.pathRuler);
                e.Graphics.DrawPath(penMarker, VisuGCode.pathMarker);
                e.Graphics.DrawPath(penDown, VisuGCode.pathPenDown);
                if (Properties.Settings.Default.gui2DPenUpShow)
                    e.Graphics.DrawPath(penUp, VisuGCode.pathPenUp);
                //           e.Graphics.DrawEllipse(penTeach, new Rectangle((int)actualPosMarker.X-2, (int)actualPosMarker.Y - 2, (int)actualPosMarker.X + 2, (int)actualPosMarker.Y + 2));
            }
        }
        private void ShowLabel(string txt, System.Drawing.Point stringpos, Graphics graph)
        {
            Font fnt = new Font("Lucida Console", 8);
            var size = graph.MeasureString(txt, fnt);
            var rect = new RectangleF(stringpos.X - 2, stringpos.Y - 2, size.Width + 1, size.Height + 1);
            graph.FillRectangle(Brushes.White, rect);           //Filling a rectangle before drawing the string.
            graph.DrawString(txt, fnt, brushText, stringpos);
            fnt.Dispose();
        }
        // Calculate click position and send coordinates via event
        private void PictureBoxVideo_Click(object sender, MouseEventArgs e)//, EventArgs e)
        {
            double relposx = 2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).X) / pictureBoxVideo.Size.Width - 0.5);
            double relposy = -2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).Y) / pictureBoxVideo.Size.Height - 0.5);
            double myRadius;
            if(teachingTop)
            {
                teachingTop = false;
                myRadius = Math.Sqrt(relposx * relposx + relposy * relposy);
                if (myRadius > 0)
                { cameraScalingTop = cameraTeachRadiusTop / myRadius; }
                else
                    MessageBox.Show("Radius is '0', no update for scaling of upper position", "Attention");
                cameraPosTop = Grbl.posMachine.Z;
                Logger.Trace(culture, "Teach top X:{0} Y:{1} r:{2} scaling:{3} pos-top:{4}", relposx, relposy, myRadius, cameraScalingTop, cameraPosTop);
            }
            else if (teachingBot)
            {
                teachingBot = false;
                myRadius = Math.Sqrt(relposx * relposx + relposy * relposy);
                if (myRadius > 0)
                { cameraScalingBot = cameraTeachRadiusBot / myRadius; }
                else
                    MessageBox.Show("Radius is '0', no update for scaling of lower position", "Attention");
                cameraPosBot = Grbl.posMachine.Z;
                Logger.Trace(culture, "Teach bottom X:{0} Y:{1} r:{2} scaling:{3} pos-bot:{4}", relposx, relposy, myRadius, cameraScalingBot, cameraPosBot);
            }
            else if ((e.Button == MouseButtons.Left) && !measureAngle)
                OnRaiseXYEvent(new XYEventArgs(0, 1, realPosition, "G91")); // move relative and slow
            pictureBoxVideo.Invalidate();
        }


        internal event EventHandler<XYEventArgs> RaiseXYEvent;
        protected virtual void OnRaiseXYEvent(XYEventArgs e)
        {
            RaiseXYEvent?.Invoke(this, e);
        }

        // select video source from list
        private void CamSourceSubmenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            ((ToolStripMenuItem)camSourceToolStripMenuItem.DropDownItems[cameraIndex]).Checked = false;
            cameraIndex = Convert.ToByte(clickedItem.Tag, culture);     // (byte)clickedItem.Tag;
            clickedItem.Checked = true;
            SelectCameraSource(Convert.ToByte(cameraIndex), cameraResolution);
        }
        // change zooming
        private void NudCameraZoom_ValueChanged(object sender, EventArgs e)
        {
            cameraZoom = (float)nUDCameraZoom.Value;
        }
        // activate teaching of camera view range upper position
        private void UpperPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            teachingTop = true;
            teachingBot = false;
        }
        // activate teaching of camera view range lower position
        private void LowerPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            teachingBot = true;
            teachingTop = false;
        }
        // get values of teach range textbox
        private void ToolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            double old = cameraTeachRadiusTop;
            if (!Double.TryParse(toolStripTextBox2.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraTeachRadiusTop))
                cameraTeachRadiusTop = old;
            if (cameraTeachRadiusTop <= 0)
                cameraTeachRadiusTop = 20;
            //            toolStripTextBox2.Text = cameraTeachRadiusTop.ToString();
        }
        private void ToolStripTextBox2_Leave(object sender, EventArgs e)
        {
            toolStripTextBox2.Text = cameraTeachRadiusBot.ToString(culture);
        }
        // get values of teach range textbox
        private void ToolStripTextBox3_TextChanged(object sender, EventArgs e)
        {
            double old = cameraTeachRadiusBot;
            if (!Double.TryParse(toolStripTextBox3.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraTeachRadiusBot))
                cameraTeachRadiusBot = old;
            if (cameraTeachRadiusBot <= 0)
                cameraTeachRadiusBot = 10;
            //           toolStripTextBox3.Text = cameraTeachRadiusBot.ToString();
        }
        private void ToolStripTextBox3_Leave(object sender, EventArgs e)
        {
            toolStripTextBox3.Text = cameraTeachRadiusBot.ToString(culture);
        }


        // send event to teach Zero offset
        private void BtnSetOffsetZero_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(0, 0, 0, "G92")); // set new coordinates
        }
        // send event to teach Marker offset
        private void BtnSetOffsetMarker_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(0, 1, (XyPoint)Grbl.PosMarker, "G92"));        // set new coordinates
        }
        // sent event to apply offset
        private void BtnCamCoordTool_Click(object sender, EventArgs e)
        {
            if (cBCamCoordMove.Checked)
                OnRaiseXYEvent(new XYEventArgs(0, 1, (XyPoint)Grbl.posWork, "G54; G0G90"));  // switch coord system and move
            else
                OnRaiseXYEvent(new XYEventArgs(0, 1, (XyPoint)Grbl.posWork, "G54"));         // only switch
            btnCamCoordTool.BackColor = Color.Lime;
            btnCamCoordCam.BackColor = SystemColors.Control;
        }
        // sent event to apply offset
        private void BtnCamCoordCam_Click(object sender, EventArgs e)
        {
            if (cBCamCoordMove.Checked)
                OnRaiseXYEvent(new XYEventArgs(0, 1, (XyPoint)Grbl.posWork, "G59; G0G90"));  // switch coord system and move
            else
                OnRaiseXYEvent(new XYEventArgs(0, 1, (XyPoint)Grbl.posWork, "G59"));         // only switch
            btnCamCoordCam.BackColor = Color.Lime;
            btnCamCoordTool.BackColor = SystemColors.Control;
        }
        // show actual offset from tool position
        private void TeachToolStripMenuItem_Click(object sender, EventArgs e)       // teach offset of G59 coord system
        {
            OnRaiseXYEvent(new XYEventArgs(0, 1, (XyPoint)Grbl.posWork, "G10 L2 P6 "));   // move relative and fast
        }

        // measure angle 
        private bool measureAngle = false;
        private XyPoint measureAngleStart = new XyPoint(0, 0);
        private XyPoint measureAngleStop = new XyPoint(0, 0);
        private XyPoint angleRotationCenter = new XyPoint(0, 0);
        private void PictureBoxVideo_MouseDown(object sender, MouseEventArgs e)
        {
            //cmsPictureBox.Visible = true;
            if (e.Button == MouseButtons.Right)
            {
                measureAngleStart = (XyPoint)pictureBoxVideo.PointToClient(MousePosition);
                angleRotationCenter = (XyPoint)Grbl.posWork + realPosition;
                measureAngle = true;
            }
            if (e.Delta > 0)
            {
                nUDCameraZoom.Value += nUDCameraZoom.Increment;
                MessageBox.Show(e.Delta.ToString());
            }
            if (e.Delta < 0)
                nUDCameraZoom.Value -= nUDCameraZoom.Increment;
        }

        private void PictureBoxVideo_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (nUDCameraZoom.Value < nUDCameraZoom.Maximum)
                    nUDCameraZoom.Value += nUDCameraZoom.Increment;
            }
            if (e.Delta < 0)
            {
                if (nUDCameraZoom.Value > nUDCameraZoom.Minimum)
                    nUDCameraZoom.Value -= nUDCameraZoom.Increment;
            }
        }

        private void PictureBoxVideo_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                measureAngleStop = (XyPoint)pictureBoxVideo.PointToClient(MousePosition);
                angle = (float)measureAngleStart.AngleTo(measureAngleStop);
                measureAngle = false;
                lblAngle.Text = String.Format("{0:0.00}°", angle);
                if (angle != 0)
                    cmsPictureBox.Visible = false;
                else
                    cmsPictureBox.Visible = true;
            }
        }

        private void BtnApplyAngle_Click(object sender, EventArgs e)
        {
            if (cBRotateArround0.Checked)
                OnRaiseXYEvent(new XYEventArgs(angle, 1, new XyPoint(0, 0), "a"));
            else
                OnRaiseXYEvent(new XYEventArgs(angle, 1, angleRotationCenter, "a"));

        }

        // change camera rotation
        private void ToolStripTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!double.TryParse(toolStripTextBox1.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraRotation))
                    cameraRotation = 0;
                toolStripTextBox1.Text = cameraRotation.ToString();
            }
        }

        private void TextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyColor(textToolStripMenuItem, "cameraColorText");
            brushText = new SolidBrush(Properties.Settings.Default.cameraColorText);
        }
        private void CrossHairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyColor(crossHairsToolStripMenuItem, "cameraColorCross");
            colCross = Properties.Settings.Default.cameraColorCross;
            pen1 = new Pen(colCross, 1f);
        }

        private void ApplyColor(ToolStripMenuItem btn, string settings)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                SetButtonColors(btn, colorDialog1.Color);
                Properties.Settings.Default[settings] = colorDialog1.Color;
                //                saveSettings();
            }
        }
        private static void SetButtonColors(ToolStripMenuItem btn, Color col)
        {
            btn.BackColor = col;
            btn.ForeColor = ContrastColor(col);
        }
        private static Color ContrastColor(Color color)
        {
            int d;
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
            ColorFiltering colorFilter = new ColorFiltering
            {
                Red = new IntRange(Properties.Settings.Default.camFilterRed1, Properties.Settings.Default.camFilterRed2),
                Green = new IntRange(Properties.Settings.Default.camFilterGreen1, Properties.Settings.Default.camFilterGreen2),
                Blue = new IntRange(Properties.Settings.Default.camFilterBlue1, Properties.Settings.Default.camFilterBlue2),
                FillOutsideRange = Properties.Settings.Default.camFilterOutside
            };

            colorFilter.ApplyInPlace(bitmapData);

            // step 2 - locating objects
            BlobCounter blobCounter = new BlobCounter
            {
                FilterBlobs = true,
                MinHeight = (int)Properties.Settings.Default.camShapeSizeMin * (int)cameraZoom,
                MinWidth = (int)Properties.Settings.Default.camShapeSizeMin * (int)cameraZoom,
                MaxHeight = (int)Properties.Settings.Default.camShapeSizeMax * (int)cameraZoom,
                MaxWidth = (int)Properties.Settings.Default.camShapeSizeMax * (int)cameraZoom
            };

            blobCounter.ProcessImage(bitmapData);

            Blob[] blobs = blobCounter.GetObjectsInformation();
            bitmap.UnlockBits(bitmapData);

            // step 3 - check objects' type and highlight
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker
            {
                MinAcceptableDistortion = (float)Properties.Settings.Default.camShapeDist,
                RelativeDistortionLimit = (float)Properties.Settings.Default.camShapeDistMax
            };

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
                System.Single myRadius;
                // is circle ?
                //          g.DrawPolygon(greenPen, ToPointsArray(edgePoints));

                if (Properties.Settings.Default.camShapeCircle && shapeChecker.IsCircle(edgePoints, out center, out myRadius))
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
                        shapeRadius = myRadius;
                    }
                }
                List<IntPoint> corners;
                if (Properties.Settings.Default.camShapeRect && shapeChecker.IsQuadrilateral(edgePoints, out corners))  //.IsConvexPolygon
                {
                    IntPoint minxy, maxxy, centxy;
                    shapeFound = true;
                    //  PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);
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
            pictureBoxVideo.BackgroundImage = bitmap;
        }
        // Convert list of AForge.NET's points to array of .NET points
        private static System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];
            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }
            return array;
        }

        private void CbShapeDetection_CheckedChanged(object sender, EventArgs e)
        {
            if (cBShapeDetection.Checked)
                btnAutoCenter.Enabled = true;
            else
                btnAutoCenter.Enabled = false;
        }

        private void BtnAutoCenter_Click(object sender, EventArgs e)
        {
            if (shapeFound)
            {
                float actualScaling = GetActualScaling();
                XyPoint tmp;
                tmp.X = 2 * (Convert.ToDouble(shapeCenter.X) / pictureBoxVideo.Size.Width - 0.5) * actualScaling / cameraZoom;
                tmp.Y = -2 * (Convert.ToDouble(shapeCenter.Y) / pictureBoxVideo.Size.Height - 0.5) * actualScaling * ratio / cameraZoom;
                OnRaiseXYEvent(new XYEventArgs(0, 1, tmp / 2, "G91")); // move relative and slow
                                                                       //               MessageBox.Show(x.ToString() + "  " + y.ToString()+"\r\n"+ shapeCenter.X.ToString()+"  "+ shapeCenter.Y.ToString());
                lblCenterPos.Text = string.Format("X: {0:0.000}  Y: {1:0.000}", tmp.X, tmp.Y);
            }
            else
                lblCenterPos.Text = "No shape found";
        }

        private float GetActualScaling()
        {
            double diff = cameraPosTop - cameraPosBot;
            if (diff == 0) diff = 1;
            double m = (cameraScalingTop - cameraScalingBot) / diff;
            double n = cameraScalingTop - m * cameraPosTop;
            float actualScaling = (float)Math.Abs(Grbl.posMachine.Z * m + n);
            if (actualScaling < 1) actualScaling = 1;
            return actualScaling;
        }

        private static void ShapeSetLoad(string txt)
        {
            string[] value = txt.Split('|');
            int i = 1;
            try
            {
//#pragma warning disable CA1305 
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
//#pragma warning restore CA1305
            }
            catch (Exception ex){ Logger.Error(ex, "ShapeSetLoad "); throw; }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = comboBox1.SelectedIndex;
            if (selectedIndex == 0) { ShapeSetLoad(Properties.Settings.Default.camShapeSet1); }
            if (selectedIndex == 1) { ShapeSetLoad(Properties.Settings.Default.camShapeSet2); }
            if (selectedIndex == 2) { ShapeSetLoad(Properties.Settings.Default.camShapeSet3); }
            if (selectedIndex == 3) { ShapeSetLoad(Properties.Settings.Default.camShapeSet4); }
        }

        private void Teachpoint1_process_Click(object sender, EventArgs e)
        {
            teachPoint1 = (XyPoint)Grbl.PosMarker;
            teachPoint2 = teachPoint1; //teachPoint3 = teachPoint1;
            measureAngleStart = teachPoint1;
            OnRaiseXYEvent(new XYEventArgs(0, 1, (XyPoint)Grbl.PosMarker, "G92"));        // set new coordinates
        }

        private void Teachpoint2_process_Click(object sender, EventArgs e)
        {
            teachPoint2 = (XyPoint)Grbl.PosMarker;
            double angle1 = teachPoint1.AngleTo(teachPoint2);
            double dist1 = teachPoint1.DistanceTo(teachPoint2);
            double angle2 = teachPoint1.AngleTo((XyPoint)Grbl.posWork);
            double dist2 = teachPoint1.DistanceTo((XyPoint)Grbl.posWork);
            double angleResult = angle1 - angle2;
            lblAngle.Text = String.Format(culture, "{0:0.00}°", angleResult);

            OnRaiseXYEvent(new XYEventArgs(angleResult, dist2 / dist1, teachPoint1, "a"));       // rotate arround TP1
        }

        private void ControlCameraForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            { showOverlay = false; }
        }

        private void ControlCameraForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            { showOverlay = true; }
        }

        private void ShowOverlayGraphicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showOverlay = !showOverlay;
            showOverlayGraphicsToolStripMenuItem.Checked = showOverlay;
        }

        private void PictureBoxVideo_MouseEnter(object sender, EventArgs e)
        {
            if (!pictureBoxVideo.Focused)
                pictureBoxVideo.Focus();
        }

        private void PictureBoxVideo_MouseLeave(object sender, EventArgs e)
        {
            if (pictureBoxVideo.Focused)
                pictureBoxVideo.Parent.Focus();
        }

        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string txt = "";
            MessageBox.Show(txt);
        }

        private void ResetOffsetG921ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(0, 1, (XyPoint)Grbl.posWork, "G92.1"));       // rotate arround TP1
        }

        private void FillComboBox(int index, string txt)
        {
            if (index == 1) { comboBox1.Items[0] = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (index == 2) { comboBox1.Items[1] = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (index == 3) { comboBox1.Items[2] = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (index == 4) { comboBox1.Items[3] = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
        }
    }
}
