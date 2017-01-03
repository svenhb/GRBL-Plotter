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
/*
    Thanks to http://code-bude.net/2011/06/02/webcam-benutzen-in-csharp/
*/
/*
 *  2017-01-01  check form-location and fix strange location
*/

using System;
using System.Drawing;
using System.Windows.Forms;

// AForge Library http://www.aforgenet.com/framework/
using AForge.Video.DirectShow;

namespace GRBL_Plotter
{
    public partial class ControlCameraForm : Form
    {
        private int cameraIndex = 0;
        private int cameraResolution = 640;
        private float cameraRotation = 180;
        private double cameraTeachRadiusTop = 30;
        private double cameraTeachRadiusBot = 10;
        private double cameraScalingTop = 20;
        private double cameraScalingBot = 20;
        private double cameraPosTop = 0;
        private double cameraPosBot = -50;
        private double cameraToolOffsetX = 0;
        private double cameraToolOffsetY = 0;
        private double relposX;
        private double relposY;
        private float cameraZoom = 1;
        private bool teachingTop = false;
        private bool teachingBot = false;

        private xyzPoint actualPosWorld;
        private xyzPoint actualPosMachine;
        private xyzPoint actualPosMarker;

        public xyzPoint setPosWorld
        { set { actualPosWorld=value; } }
        public xyzPoint setPosMachine
        { set { actualPosMachine = value; } }
        public void setPosMarker(double x, double y)
        { actualPosMarker.X = x; actualPosMarker.Y = y; actualPosMarker.Z = 0;  }


        public ControlCameraForm()
        {  InitializeComponent(); }

        VideoCaptureDevice videoSource;
        FilterInfoCollection videosources;
        // load settings, get list of video-sources, set video-source
        private void camera_form_Load(object sender, EventArgs e)
        {
            cameraIndex = Properties.Settings.Default.camerindex;
            cameraRotation = Properties.Settings.Default.camerarotation;
            if (cameraRotation > 360)
                cameraRotation = 0;
            toolStripTextBox1.Text = cameraRotation.ToString();
            cameraTeachRadiusTop = Properties.Settings.Default.cameraTeachRadiusTop;
            cameraTeachRadiusBot = Properties.Settings.Default.cameraTeachRadiusBot;
            toolStripTextBox2.Text = cameraTeachRadiusTop.ToString();
            toolStripTextBox3.Text = cameraTeachRadiusBot.ToString();
            cameraScalingTop = Properties.Settings.Default.camerascalingTop;
            cameraScalingBot = Properties.Settings.Default.camerascalingBot;
            cameraPosTop = Properties.Settings.Default.cameraPosTop;
            cameraPosBot = Properties.Settings.Default.cameraPosBot;
            cameraToolOffsetX = Properties.Settings.Default.cameraToolOffsetX;
            cameraToolOffsetY = Properties.Settings.Default.cameraToolOffsetY;
            lblOffset.Text = String.Format("Cam.Off.: X{0} Y{1}", cameraToolOffsetX, cameraToolOffsetY);

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
            ratio = (double)pictureBoxVideo.Size.Height / pictureBoxVideo.Size.Width;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(0, 0); }
        }
        // save settings
        private void camera_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.camerindex = cameraIndex;
            Properties.Settings.Default.camerarotation = cameraRotation;
            Properties.Settings.Default.camerascalingTop = cameraScalingTop;
            Properties.Settings.Default.camerascalingBot = cameraScalingBot;
            Properties.Settings.Default.cameraTeachRadiusTop = cameraTeachRadiusTop;
            Properties.Settings.Default.cameraTeachRadiusBot = cameraTeachRadiusBot;
            Properties.Settings.Default.cameraPosTop = cameraPosTop;
            Properties.Settings.Default.cameraPosBot = cameraPosBot;
            Properties.Settings.Default.cameraToolOffsetX = cameraToolOffsetX;
            Properties.Settings.Default.cameraToolOffsetY = cameraToolOffsetY;
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
                pictureBoxVideo.BackgroundImage = RotateImage((Bitmap)eventArgs.Frame.Clone(), cameraRotation, cameraZoom);
                frameCounter = 0;
            }
        }
        // rotate image from: http://code-bude.net/2011/07/12/bilder-rotieren-mit-csharp-bitmap-rotateflip-vs-gdi-graphics/
        public static Image RotateImage(Image img, float rotationAngle, float zoom)
        {
            using (Graphics gfx = Graphics.FromImage(img))
            {
                gfx.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);
                gfx.RotateTransform(rotationAngle);
                gfx.ScaleTransform(zoom, zoom);
                gfx.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);
                gfx.DrawImage(img, new Point(0, 0));
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
            double diff = cameraPosTop-cameraPosBot;
            if (diff == 0) diff = 1;
            double m = (cameraScalingTop - cameraScalingBot) / diff;
            double n = cameraScalingTop - m * cameraPosTop;
            float actualScaling = (float)Math.Abs(actualPosMachine.Z *m+n);
            if (actualScaling < 1) actualScaling = 5;

            int gap = 5;
            int chlen = 200;
            e.Graphics.DrawLine(pen1, new Point(xmid- chlen, ymid), new Point(xmid-gap, ymid));
            e.Graphics.DrawLine(pen1, new Point(xmid + gap, ymid), new Point(xmid+ chlen, ymid));
            e.Graphics.DrawLine(pen1, new Point(xmid, ymid- chlen), new Point(xmid, ymid-gap));
            e.Graphics.DrawLine(pen1, new Point(xmid, ymid+gap), new Point(xmid, ymid+ chlen));
            relposX =  2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).X) / pictureBoxVideo.Size.Width  - 0.5)* actualScaling / cameraZoom;
            relposY = -2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).Y) / pictureBoxVideo.Size.Height - 0.5) * actualScaling * ratio / cameraZoom;
            int stringposY = -40;
            if (relposY > 0) stringposY = 10;
            Point stringpos = new Point(pictureBoxVideo.PointToClient(MousePosition).X-60, pictureBoxVideo.PointToClient(MousePosition).Y+ stringposY);
            if (teachingTop)
            {
                radius = (int)(cameraTeachRadiusTop * cameraZoom * xmid / actualScaling);
                e.Graphics.DrawEllipse(pen1, new Rectangle(xmid - radius, ymid - radius, 2 * radius, 2 * radius));
                e.Graphics.DrawString("Next click will teach scaling\r\n of upper camera view to " + cameraTeachRadiusTop.ToString(), new Font("Microsoft Sans Serif", 8), Brushes.White, stringpos);
            }
            else if (teachingBot)
            {
                radius = (int)(cameraTeachRadiusBot * cameraZoom * xmid / actualScaling);
                e.Graphics.DrawEllipse(pen1, new Rectangle(xmid - radius, ymid - radius, 2 * radius, 2 * radius));
                e.Graphics.DrawString("Next click will teach scaling\r\n of upper camera view to " + cameraTeachRadiusBot.ToString(), new Font("Microsoft Sans Serif", 8), Brushes.White, stringpos);
            }
            else if (measureAngle)
            {
                calculateAngle();
                e.Graphics.DrawLine(pen1, measureAngleStartX, measureAngleStartY, measureAngleStopX, measureAngleStopY);
                e.Graphics.DrawString(String.Format("Angle: {0:0.00}", angle), new Font("Microsoft Sans Serif", 8), Brushes.White, stringpos);
            }
            else
            {
                double x2 = actualPosWorld.X + relposX;
                double y2 = actualPosWorld.Y + relposY;
                double x3 = x2 + cameraToolOffsetX;
                double y3 = y2 + cameraToolOffsetY;
                e.Graphics.DrawString(String.Format("Relative {0:0.00} ; {1:0.00}\r\nAbsolute {2:0.00} ; {3:0.00}\r\nCam.Off. {4:0.00} ; {5:0.00}", relposX, relposY, x2, y2,x3,y3), new Font("Lucida Console", 8), Brushes.White, stringpos);
            }

            float scale = (float)(xmid/actualScaling * cameraZoom);
            e.Graphics.ScaleTransform(scale,-scale);
            float offX = (float)(xmid/scale- actualPosWorld.X);
            float offY = (float)(ymid/scale+ actualPosWorld.Y);
            if (cBCamOffset.Checked)
            {   offX = (float)(xmid / scale - actualPosWorld.X - cameraToolOffsetX);
                offY = (float)(ymid / scale + actualPosWorld.Y + cameraToolOffsetY);
            }
            e.Graphics.TranslateTransform(offX, -offY);       // apply offset
            // show drawing from MainForm (static members of class GCodeVisualization)
            e.Graphics.DrawPath(penRuler, GCodeVisuAndTransform.pathRuler);
            e.Graphics.DrawPath(penMarker, GCodeVisuAndTransform.pathMarker);
            e.Graphics.DrawPath(penDown, GCodeVisuAndTransform.pathPenDown);
            e.Graphics.DrawPath(penUp, GCodeVisuAndTransform.pathPenUp);
        }

        private void calculateAngle()
        {
            measureAngleStopX = pictureBoxVideo.PointToClient(MousePosition).X;
            measureAngleStopY = pictureBoxVideo.PointToClient(MousePosition).Y;
            int distanceX = measureAngleStopX - measureAngleStartX;
            int distanceY = measureAngleStopY - measureAngleStartY;
            double radius = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            float cos1 = (float)distanceX / (float)radius;
            if (cos1 > 1) cos1 = 1;
            if (cos1 < -1) cos1 = -1;
            angle = 180 * (float)(Math.Acos(cos1) / Math.PI);
            if (distanceY > 0)
                angle = -angle;
        }

        // Calculate click position and send coordinates via event
        private void pictureBoxVideo_Click(object sender, EventArgs e)
        {
            double relposx = 2*(Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).X) / pictureBoxVideo.Size.Width-0.5);
            double relposy = -2*(Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).Y) / pictureBoxVideo.Size.Height-0.5);
            if (teachingTop)
            {   teachingTop = false;
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
            else if (!measureAngle)
                OnRaiseXYEvent(new XYEventArgs(relposX, relposY, "G91G1")); // move relative and slow
            pictureBoxVideo.Invalidate();
        }
        public event EventHandler<XYEventArgs> RaiseXYEvent;
        protected virtual void OnRaiseXYEvent(XYEventArgs e)
        {   EventHandler<XYEventArgs> handler = RaiseXYEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        // select video source from list
        private void camSourceSubmenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            cameraIndex = (int)clickedItem.Tag;
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
        }
        // activate teaching of camera view range lower position
        private void lowerPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            teachingBot = true;
        }
        // get values of teach range textbox
        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(toolStripTextBox2.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraTeachRadiusTop))
                cameraTeachRadiusTop = 20;
            toolStripTextBox2.Text = cameraTeachRadiusTop.ToString();
        }
        // get values of teach range textbox
        private void toolStripTextBox3_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(toolStripTextBox3.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraTeachRadiusBot))
                cameraTeachRadiusBot=10;
            toolStripTextBox3.Text = cameraTeachRadiusBot.ToString();
        }
        // send event to teach Zero offset
        private void btnSetOffsetZero_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(-cameraToolOffsetX, -cameraToolOffsetY, "G92")); // set new coordinates
        }
        // send event to teach Marker offset
        private void btnSetOffsetMarker_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(-cameraToolOffsetX + actualPosMarker.X, -cameraToolOffsetY + actualPosMarker.Y, "G92")); // set new coordinates
        }
        // sent event to apply offset
        private void btnCamOffsetPlus_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(-cameraToolOffsetX, -cameraToolOffsetY, "G91G0"));   // move relative and fast
        }
        // sent event to apply offset
        private void btnCamOffsetMinus_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(cameraToolOffsetX, cameraToolOffsetY, "G91G0"));   // move relative and fast
        }
        // show actual offset from tool position
        private void teachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cameraToolOffsetX = -actualPosWorld.X;
            cameraToolOffsetY = -actualPosWorld.Y;
            lblOffset.Text=String.Format("Camera Offset: X{0} Y{1}", cameraToolOffsetX, cameraToolOffsetY);
        }

        // measure angle 
        private bool measureAngle = false;
        private int measureAngleStartX = 0;
        private int measureAngleStartY = 0;
        private int measureAngleStopX = 0;
        private int measureAngleStopY = 0;
        private void pictureBoxVideo_MouseDown(object sender, MouseEventArgs e)
        {   if (e.Button == MouseButtons.Right)
            {
                measureAngleStartX = pictureBoxVideo.PointToClient(MousePosition).X;
                measureAngleStartY = pictureBoxVideo.PointToClient(MousePosition).Y;
                measureAngle = true;
            }
        }
        private void pictureBoxVideo_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                calculateAngle();
                measureAngle = false;
                lblAngle.Text = String.Format("{0:0.00}°",angle);
            }
        }

        private void btnApplyAngle_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(angle, angle, "a"));
        }
        // change camera rotation
        private void toolStripTextBox1_KeyUp(object sender, KeyEventArgs e)
        {   if (e.KeyCode == Keys.Enter)
            {   if (!float.TryParse(toolStripTextBox1.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraRotation))
                    cameraRotation = 0;
                toolStripTextBox2.Text = cameraRotation.ToString();
            }
        }
    }

    // establish own event to send XY coordinates
    public class XYEventArgs : EventArgs
    {
        private double posX, posY;
        string command;
        public XYEventArgs(double x,double y,string cmd)
        {
            posX=x;
            posY=y;
            command = cmd;
        }
        public double PosX
        {
            get { return posX; }
        }
        public double PosY
        {
            get { return posY; }
        }
        public string Command
        {
            get { return command; }
        }
    }

}
