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
 * 2021-09-12 split some code to ControlCameraFormMisc.cs
 *            - add distortion correction, shape recognizion: don't show applied filter
 * 2021-12-11 line 1097 check if ((cameraIndex >= 0) && (cameraIndex < videosources.Count)) 
 * 2022-03-23 listSettings if (cBShapeDetection.Checked)
 * 2023-01-02 exception line 1202, add try, catch
 * 2023-01-22 line 546 use lock (Object is currently in use elsewhere. Source: System.Drawing Target:)
 * 2023-01-24 check range of index in line 1218
 * 2023-08-01 check if (!e.Bounds.IsEmpty)
 * 2025-03-12 l:1452 add class ini-file at the end
*/

using AForge;
using AForge.Imaging.Filters;
// AForge Library http://www.aforgenet.com/framework/
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlCameraForm : Form
    {
        private byte cameraIndex = 0;           // Index may change if other cam is missing

        private string cameraIndexName = "";    // name will not change
        private readonly int cameraResolutionX = 640;//3840;//640;
        //      private int cameraResolutionY = 2160;//640;
        private double cameraRotation = 180;

        private byte filterIndex = 0;
        private byte filterIndexXy = 0;
        private byte filterIndexFix = 0;

        private double cameraTeachRadiusXyzTop = 30;
        private double cameraTeachRadiusXyzBot = 10;
        private double cameraTeachRadiusXy = 10;
        private double cameraTeachRadiusFix = 100;

        private double cameraScalingXyzTop = 20;
        private double cameraScalingXyzBot = 20;
        private double cameraScalingXy = 20;
        private double cameraScalingFix = 20;

        private double cameraPosZTop = 0;
        private double cameraPosZBot = -50;

        private float cameraZeroFixXInPx = 10;
        private float cameraZeroFixYInPx = 20;

        private float cameraZoom = 1;

        private bool showOverlay = true;

        private Color colText = Color.Lime;
        private Brush brushText = Brushes.Lime;
        private Color colCross = Color.Yellow;

        private XyPoint realPosition;
        private XyPoint teachPoint1;
        private XyPoint teachPoint2;
        private XyPoint lastClickedCoordinate;
        //    private XyPoint positionOffsetMachine = new XyPoint();

        //     private XyPoint teachPoint3;
        private int coordG = 54;
        private readonly string zeroCmd0 = "G10 L20 P0";    // "G92"
                                                            //     private readonly string zeroCmd0 = "G92";   		// "G92"		line 910 G92.1

        private System.Drawing.Image exampleFrame;
        private System.Drawing.Image exampleFrameFix;
        private System.Drawing.Image exampleFrameXy;

        private bool useFreezFrame = false;

        private VideoCaptureDevice videoSource;
        private FilterInfoCollection videosources;
        private int frameCounter = 0;
        private int frameCounterMax = 10;


        internal event EventHandler<XYEventArgs> RaiseXYEvent;


        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

        public int SetCoordG	// called by other form to set actual G-Nr.
        {
            set
            {
                coordG = value;
                btnCamCoordTool.BackColor = SystemColors.Control;
                btnCamCoordCam.BackColor = SystemColors.Control;
                if (coordG == 54) { btnCamCoordTool.BackColor = Color.Lime; }
                else if (coordG == 59) { btnCamCoordCam.BackColor = Color.Lime; }
                ListFiducials();
            }
            get { return coordG; }
        }
        public void NewDrawing()
        { ListFiducials(); }

        public ControlCameraForm()
        {
            Logger.Trace("++++++ ControlCameraForm START ++++++");
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        // load settings, get list of video-sources, set video-source
        private void Camera_form_Load(object sender, EventArgs e)
        {
            int index = Properties.Settings.Default.cameraMount;
            if (index < 0) index = 0;
            if (index > (int)CameraMounting.MoveXYZ) index = (int)CameraMounting.MoveXYZ;
            CameraMount = (CameraMounting)index;

            ToolStripMenuItem[] mountItems = new ToolStripMenuItem[3]; // You would obviously calculate this value at runtime
            mountItems[0] = new ToolStripMenuItem { Tag = 0, Text = Localization.GetString("cameraMountFix"), Checked = ((int)CameraMount == 0) };
            mountItems[1] = new ToolStripMenuItem { Tag = 1, Text = Localization.GetString("cameraMountXy"), Checked = ((int)CameraMount == 1) };
            mountItems[2] = new ToolStripMenuItem { Tag = 2, Text = Localization.GetString("cameraMountXyz"), Checked = ((int)CameraMount == 2) };
            mountItems[0].Click += new EventHandler(ToolStripCameraMount_SelectedIndexChanged);
            mountItems[1].Click += new EventHandler(ToolStripCameraMount_SelectedIndexChanged);
            mountItems[2].Click += new EventHandler(ToolStripCameraMount_SelectedIndexChanged);
            cameraMountToolStripMenuItem.DropDownItems.Clear();
            cameraMountToolStripMenuItem.DropDownItems.AddRange(mountItems);

            this.Text = Localization.GetString(mountingText[(int)CameraMount]);

            // find cams, fill toolstrip
            videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videosources != null)
            {
                ToolStripMenuItem[] videoItems = new ToolStripMenuItem[videosources.Count]; // You would obviously calculate this value at runtime
                int i;
                for (i = 0; i < videosources.Count; i++)
                {
                    videoItems[i] = new ToolStripMenuItem
                    {
                        Name = "camselect" + i.ToString(culture),
                        Tag = i,
                        Text = videosources[i].Name,
                        Checked = false
                    };
                    videoItems[i].Click += new EventHandler(CamSourceSubmenuItem_Click);
                    Logger.Trace("Set cam selector {0}) '{1}'", i, videoItems[i].Text);
                }
                camSourceToolStripMenuItem.DropDownItems.Clear();
                camSourceToolStripMenuItem.DropDownItems.AddRange(videoItems);
            }

            cameraTeachRadiusFix = Properties.Settings.Default.cameraTeachRadiusFix;
            cameraTeachRadiusXy = Properties.Settings.Default.cameraTeachRadiusXy;
            cameraTeachRadiusXyzTop = Properties.Settings.Default.cameraTeachRadiusXyzTop;
            cameraTeachRadiusXyzBot = Properties.Settings.Default.cameraTeachRadiusXyzBot;

            // get last selected cam
            FillComboBox();
            SetCameraMountDependence();	// depending on fix or xy: load camera -index, -name, -rotation, -distortion-values, 

            cameraScalingFix = Properties.Settings.Default.cameraScalingFix;
            if (cameraScalingFix == 0) cameraScalingFix = 1;
            cameraScalingXy = Properties.Settings.Default.cameraScalingXy;
            if (cameraScalingXy == 0) cameraScalingXy = 1;
            cameraScalingXyzTop = Properties.Settings.Default.cameraScalingXyzTop;
            cameraScalingXyzBot = Properties.Settings.Default.cameraScalingXyzBot;
            cameraPosZTop = Properties.Settings.Default.cameraPosTop;
            cameraPosZBot = Properties.Settings.Default.cameraPosBot;

            cameraZeroFixXInPx = (float)Properties.Settings.Default.cameraZeroFixX;
            cameraZeroFixYInPx = (float)Properties.Settings.Default.cameraZeroFixY;

            filterIndexFix = Properties.Settings.Default.cameraFilterIndexFix;
            filterIndexXy = Properties.Settings.Default.cameraFilterIndexXy;

            SetToolStrip(Color.White, string.Format("{0} {1} use cam '{2}'", Localization.GetString("cameraSetMount"), mountingText[(int)CameraMount], cameraIndexName));
            SetMenuVisibility();

            colText = Properties.Settings.Default.cameraColorText;
            SetButtonColors(textToolStripMenuItem, colText);
            brushText = new SolidBrush(colText);

            colCross = Properties.Settings.Default.cameraColorCross;
            SetButtonColors(crossHairsToolStripMenuItem, colCross);

            SetPens();

            xmid = pictureBoxVideo.Size.Width / 2;
            ymid = pictureBoxVideo.Size.Height / 2;
            picCenter.X = xmid;
            picCenter.Y = ymid;
            refPointInPx = picCenter;

            ratio = (double)pictureBoxVideo.Size.Height / pictureBoxVideo.Size.Width;

            Location = Properties.Settings.Default.locationCamForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }


            if (Properties.Settings.Default.cameraSimulateImage)
            {
                LoadExampleImages();
                useFreezFrame = true; ;
                if (useFreezFrame)
                {
                    if (CameraMount == CameraMounting.Fix)
                    { exampleFrame = exampleFrameFix; }
                    else
                    { exampleFrame = exampleFrameXy; }

                    timerFreezFrame.Start();
                }
                else
                { timerFreezFrame.Stop(); }
                simulateCameraToolStripMenuItem.Checked = useFreezFrame;
            }

            // apply last cam at the end
            if (videosources != null)
            {
                if (videosources.Count > 0)
                {
                    if (cameraIndex >= videosources.Count)
                        cameraIndex = 0;
                    videoSource = new VideoCaptureDevice(videosources[cameraIndex].MonikerString);
                    SelectCameraSource(cameraIndex, cameraResolutionX);
                    Logger.Trace("OnLoad 2 select cam {0}) '{1}' ", cameraIndex, cameraIndexName);
                }
                else
                    MessageBox.Show(Localization.GetString("cameraNoCamera"));
            }
            toolStripCameraMount.Click += ToolStripCameraMount_SelectedIndexChanged;

            /*    if (false)  // perhaps needed in future
                {
                    positionOffsetMachine.X = (double)Properties.Settings.Default.machineLimitsHomeX;
                    positionOffsetMachine.Y = (double)Properties.Settings.Default.machineLimitsHomeY;
                }
                else*/
            //        positionOffsetMachine = new XyPoint();
        }
        // save settings
        private void Camera_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.locationCamForm = Location;
            SaveCameraDependence();
            Properties.Settings.Default.cameraScalingXyzTop = cameraScalingXyzTop;
            Properties.Settings.Default.cameraScalingXyzBot = cameraScalingXyzBot;
            Properties.Settings.Default.cameraScalingXy = cameraScalingXy;
            Properties.Settings.Default.cameraScalingFix = cameraScalingFix;
            Properties.Settings.Default.cameraTeachRadiusXyzTop = cameraTeachRadiusXyzTop;
            Properties.Settings.Default.cameraTeachRadiusXyzBot = cameraTeachRadiusXyzBot;
            Properties.Settings.Default.cameraTeachRadiusXy = cameraTeachRadiusXy;
            Properties.Settings.Default.cameraTeachRadiusFix = cameraTeachRadiusFix;
            Properties.Settings.Default.cameraPosTop = cameraPosZTop;
            Properties.Settings.Default.cameraPosBot = cameraPosZBot;
            Properties.Settings.Default.cameraZeroFixX = cameraZeroFixXInPx;
            Properties.Settings.Default.cameraZeroFixY = cameraZeroFixYInPx;

            Properties.Settings.Default.Save();

            exampleFrameFix?.Dispose();
            exampleFrameXy?.Dispose();

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.NewFrame -= VideoSource_NewFrame;
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource = null;
            }
            Logger.Trace("++++++ ControlCameraForm Stop ++++++");
        }
        private void Camera_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (videoSource != null)// && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource = null;
            }
        }
        // event-handler of video - rotate image and display
        void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            if (!useFreezFrame)
            {
                frameCounter++;
                if (frameCounter > frameCounterMax)
                {
                    if (cBShapeDetection.Checked || fiducialDetection)
                        ProcessShapeDetection((Bitmap)ProcessImage((Bitmap)eventArgs.Frame.Clone(), (float)cameraRotation, cameraZoom));
                    else
                        pictureBoxVideo.Image = ProcessImage((Bitmap)eventArgs.Frame.Clone(), (float)cameraRotation, cameraZoom);
                    //        pictureBoxVideo.BackgroundImage = ProcessImage((Bitmap)eventArgs.Frame.Clone(), (float)cameraRotation, cameraZoom);
                    frameCounter = 0;
                }
            }
        }
        // rotate image from: http://code-bude.net/2011/07/12/bilder-rotieren-mit-csharp-bitmap-rotateflip-vs-gdi-graphics/

        private void SimulateCameraToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            useFreezFrame = simulateCameraToolStripMenuItem.Checked;
            if (useFreezFrame)
            {
                if ((exampleFrameFix == null) || (exampleFrameXy == null))
                    LoadExampleImages();
                if (CameraMount == CameraMounting.Fix)
                    exampleFrame = exampleFrameFix;
                else
                    exampleFrame = exampleFrameXy;

                timerFreezFrame.Start();
            }
            else
                timerFreezFrame.Stop();
            Properties.Settings.Default.cameraSimulateImage = useFreezFrame;
            Properties.Settings.Default.Save();
        }
        private void TimerFreezFrame_Tick(object sender, EventArgs e)
        {
            if (useFreezFrame)
            {
                if (exampleFrame == null)
                {
                    useFreezFrame = false;
                    return;
                }
                if (cBShapeDetection.Checked || fiducialDetection)
                    ProcessShapeDetection((Bitmap)ProcessImage((Bitmap)exampleFrame.Clone(), (float)cameraRotation, cameraZoom));
                else
                    pictureBoxVideo.Image = ProcessImage((Bitmap)exampleFrame.Clone(), (float)cameraRotation, cameraZoom);
            }
        }
        public static System.Drawing.Image ProcessImage(System.Drawing.Image img, float rotationAngle, float zoom)
        {
            if (img != null)
            {
                if ((quadFilter != null) && removeDistortion)
                    img = quadFilter.Apply((Bitmap)img);
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


        // onPaint event
        private int xmid;
        private int ymid;
        private int radiusInPx;
        private double ratio;
        private float measuredAngle = 0;
        private double measuredDistance = 0;
        private Pen penTeachMark = new Pen(Color.Yellow, 1f);
        private void PictureBoxVideo_Paint(object sender, PaintEventArgs e)
        {
            double actualScaling = GetActualScaling();

            //     XyPoint fixMachineOffset = new XyPoint(Properties.Settings.Default.cameraZeroFixMachineX, Properties.Settings.Default.cameraZeroFixMachineY);

            realPosition = TranslateFromPicCoordinate(pictureBoxVideo.PointToClient(MousePosition));// - (XyPoint)Grbl.posWCO;
            if (CameraMount == CameraMounting.Fix)
            {
                //realPosition = TranslateFromPicCoordinate(pictureBoxVideo.PointToClient(MousePosition));// - (XyPoint)Grbl.posWCO;
                DrawCrossHairs(e.Graphics, new IntPoint((int)cameraZeroFixXInPx, (int)cameraZeroFixYInPx), 5, 200);
            }
            else
            {
                //realPosition = TranslateFromPicCoordinate(pictureBoxVideo.PointToClient(MousePosition));// + (XyPoint)Grbl.posWork;	// inkl. GetActualScaling()
                DrawCrossHairs(e.Graphics, new IntPoint(xmid, ymid), 5, 200);
            }

            double relMousePosX = Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).X) / pictureBoxVideo.Size.Width;
            double relMousePosY = Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).Y) / pictureBoxVideo.Size.Height;
            int stringposX = 10;
            if (relMousePosX > 0.8) stringposX = -180;
            int stringposY = -40;
            if (relMousePosY < 0.2) stringposY = 40;
            System.Drawing.Point stringpos = new System.Drawing.Point(pictureBoxVideo.PointToClient(MousePosition).X + stringposX, pictureBoxVideo.PointToClient(MousePosition).Y + stringposY);

            if (setup == SetupType.TeachRefPoint)
            {
                foreach (XyPoint tmpP in realPoints)
                { DrawCrossHairs(e.Graphics, (IntPoint)TranslateToPicCoordinate(tmpP), 2, 10); }
            }
            switch (setup)
            {
                case SetupType.Distortion:
                    {
                        foreach (IntPoint tmpP in quadCorners)
                        { DrawCrossHairs(e.Graphics, tmpP, 2, 10); }
                        break;
                    }
                case SetupType.TeachFix:
                    {
                        radiusInPx = (int)(actualScaling * cameraTeachRadiusFix * cameraZoom);
                        if (radiusInPx > 0)
                        {
                            e.Graphics.DrawEllipse(penTeachMark, new Rectangle((int)cameraZeroFixXInPx - radiusInPx, (int)cameraZeroFixYInPx - radiusInPx, 2 * radiusInPx, 2 * radiusInPx));
                            string txt = Localization.GetString("cameraNextClick") + " " + cameraTeachRadiusFix.ToString(culture);
                            ShowLabel(e.Graphics, stringpos, txt);
                        }
                        break;
                    }
                case SetupType.TeachXy:
                    {
                        radiusInPx = (int)(cameraTeachRadiusXy * cameraZoom * xmid / actualScaling);
                        if (radiusInPx > 0)
                        {
                            e.Graphics.DrawEllipse(penTeachMark, new Rectangle(xmid - radiusInPx, ymid - radiusInPx, 2 * radiusInPx, 2 * radiusInPx));
                            string txt = Localization.GetString("cameraNextClick") + " " + cameraTeachRadiusXy.ToString(culture);
                            ShowLabel(e.Graphics, stringpos, txt);
                        }
                        break;
                    }
                case SetupType.TeachXyzTop:
                    {
                        radiusInPx = (int)(cameraTeachRadiusXyzTop * cameraZoom * xmid / actualScaling);
                        if (radiusInPx > 0)
                        {
                            e.Graphics.DrawEllipse(penTeachMark, new Rectangle(xmid - radiusInPx, ymid - radiusInPx, 2 * radiusInPx, 2 * radiusInPx));
                            string txt = Localization.GetString("cameraNextClick") + " " + cameraTeachRadiusXyzTop.ToString(culture);
                            ShowLabel(e.Graphics, stringpos, txt);
                        }
                        break;
                    }
                case SetupType.TeachXyzBot:
                    {
                        radiusInPx = (int)(cameraTeachRadiusXyzBot * cameraZoom * xmid / actualScaling);
                        if (radiusInPx > 0)
                        {
                            e.Graphics.DrawEllipse(penTeachMark, new Rectangle(xmid - radiusInPx, ymid - radiusInPx, 2 * radiusInPx, 2 * radiusInPx));
                            string txt = Localization.GetString("cameraNextClick") + " " + cameraTeachRadiusXyzBot.ToString(culture);
                            ShowLabel(e.Graphics, stringpos, txt);
                        }
                        break;
                    }
                case SetupType.MeasureAngle:
                    {
                        measureAngleStop = (XyPoint)pictureBoxVideo.PointToClient(MousePosition);
                        e.Graphics.DrawLine(penTeachMark, measureAngleStart.ToPoint(), measureAngleStop.ToPoint());
                        measuredAngle = (float)measureAngleStart.AngleTo(measureAngleStop);
                        string txt = String.Format(culture, "Angle: {0:0.00}", measuredAngle);
                        ShowLabel(e.Graphics, stringpos, txt);
                        break;
                    }
                case SetupType.None:
                default:
                    {
                        string txt;
                        if (measuredDistance > 10)
                        {
                            e.Graphics.DrawLine(penTeachMark, measureAngleStart.ToPoint(), measureAngleStop.ToPoint());
                            //        measuredAngle = (float)measureAngleStart.AngleTo(measureAngleStop);
                            txt = String.Format(culture, "Angle: {0:0.00}", measuredAngle);
                            ShowLabel(e.Graphics, stringpos, txt);
                        }

                        XyPoint absolute = (XyPoint)Grbl.posWork + realPosition;
                        System.Drawing.Point picPos = pictureBoxVideo.PointToClient(MousePosition);

                        if (CameraMount == CameraMounting.Fix)
                        {
                            //    realPosition += fixMachineOffset - (XyPoint)Grbl.posWCO;
                            XyPoint machine = realPosition - (XyPoint)Grbl.posWCO;// + (XyPoint)Grbl.posMachine;// + realPosition;
                            txt = String.Format(culture, "Work    {0:0.00} ; {1:0.00}\r\n", realPosition.X, realPosition.Y);
                            txt += String.Format(culture, "Machine {0:0.00} ; {1:0.00}\r\n", machine.X, machine.Y);
                            txt += String.Format(culture, "WCO     {0:0.00} ; {1:0.00}\r\n", Grbl.posWCO.X, Grbl.posWCO.Y);
                            txt += String.Format(culture, "Px {0};{1}", picPos.X, picPos.Y);
                            //    txt = string.Format("\r\nreal {0:0.00} ; {1:0.00} \r\nwork {2:0.00} ; {3:0.00}\r\nmachine {4:0.00} ; {5:0.00}\r\nWCO {6:0.00} ; {7:0.00}", realPosition.X, realPosition.Y, Grbl.posWork.X, Grbl.posWork.Y, Grbl.posMachine.X, Grbl.posMachine.Y, Grbl.posWCO.X, Grbl.posWCO.Y);
                        }
                        else
                        {
                            txt = String.Format(culture, "Relative {0:0.00} ; {1:0.00}\r\nAbsolute {2:0.00} ; {3:0.00}\r\n Px {4};{5}", realPosition.X, realPosition.Y, absolute.X, absolute.Y, picPos.X, picPos.Y);
                        }

                        ShowLabel(e.Graphics, stringpos, txt);
                        break;
                    }
            }

            if (CameraMount == CameraMounting.Fix)
            {
                e.Graphics.ScaleTransform((float)actualScaling, -(float)actualScaling);
                //           float offX = (float)(cameraZeroFixXInPx / actualScaling) + (float)Grbl.posWCO.X;// - (float)Properties.Settings.Default.cameraZeroFixMachineX;
                //           float offY = (float)(-cameraZeroFixYInPx / actualScaling) + (float)Grbl.posWCO.Y;// - (float)Properties.Settings.Default.cameraZeroFixMachineY;
                float offX = (float)(cameraZeroFixXInPx / actualScaling) + (float)Grbl.posWCO.X - (float)Properties.Settings.Default.cameraZeroFixMachineX;
                float offY = (float)(-cameraZeroFixYInPx / actualScaling) + (float)Grbl.posWCO.Y - (float)Properties.Settings.Default.cameraZeroFixMachineY;
                e.Graphics.TranslateTransform(offX, offY);       // apply offset
            }
            else
            {
                float scale = (float)(xmid / actualScaling * cameraZoom);
                if (scale == 0) scale = 1;
                e.Graphics.ScaleTransform(scale, -scale);
                float offX = (float)(xmid / scale - Grbl.posWork.X);
                float offY = (float)(ymid / scale + Grbl.posWork.Y);
                e.Graphics.TranslateTransform(offX, -offY);       // apply offset
            }

            // show drawing from MainForm (static members of class GCodeVisualization)
            if (showOverlay)
            {
                lock (penRuler)
                    e.Graphics.DrawPath(penRuler, VisuGCode.pathRuler);
                lock (penMarker)
                    e.Graphics.DrawPath(penMarker, VisuGCode.pathMarker);
                lock (penDown)
                    e.Graphics.DrawPath(penDown, VisuGCode.pathPenDown);

                if (penupPathToolStripMenuItem.Checked)
                    e.Graphics.DrawPath(penUp, VisuGCode.pathPenUp);
                if (machineLimitsToolStripMenuItem.Checked)
                    e.Graphics.FillPath(brushMachineLimit, VisuGCode.pathMachineLimit);
                if (dimensionToolStripMenuItem.Checked)
                    e.Graphics.DrawPath(penDimension, VisuGCode.pathDimension);
                if (CameraMount == CameraMounting.Fix)
                    e.Graphics.DrawPath(penRuler, VisuGCode.pathTool);
            }
        }

        private void ShowLabel(Graphics path, System.Drawing.Point stringpos, string txt)
        {
            Font fnt = new Font("Lucida Console", 8);
            var size = path.MeasureString(txt, fnt);
            var rect = new RectangleF(stringpos.X - 2, stringpos.Y - 2, size.Width + 1, size.Height + 1);
            path.FillRectangle(Brushes.White, rect);           //Filling a rectangle before drawing the string.
            path.DrawString(txt, fnt, brushText, stringpos);
            fnt.Dispose();
        }

        private void DrawCrossHairs(Graphics path, IntPoint center, int gap, int chlen)
        { DrawCrossHairs(path, new System.Drawing.Point(center.X, center.Y), gap, chlen); }
        private void DrawCrossHairs(Graphics path, System.Drawing.Point center, int gap, int chlen)
        {
            try
            {
                path.DrawLine(penTeachMark, new System.Drawing.Point(center.X - chlen, center.Y), new System.Drawing.Point(center.X - gap, center.Y));
                path.DrawLine(penTeachMark, new System.Drawing.Point(center.X + gap, center.Y), new System.Drawing.Point(center.X + chlen, center.Y));
                path.DrawLine(penTeachMark, new System.Drawing.Point(center.X, center.Y - chlen), new System.Drawing.Point(center.X, center.Y - gap));
                path.DrawLine(penTeachMark, new System.Drawing.Point(center.X, center.Y + gap), new System.Drawing.Point(center.X, center.Y + chlen));
            }
            catch (Exception err) { Logger.Error(err, "DrawCrossHairs "); }
        }
        // Calculate click position and send coordinates via event



        /***************************************************************
        *** Handle PictureBoxVideo related
        ***************************************************************/
        #region PictureBoxVideo
        private void PictureBoxVideo_Click(object sender, MouseEventArgs e)//, EventArgs e)
        {
            // normalized position in range -1 to 1, center is 0
            double relposx = 2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).X) / pictureBoxVideo.Size.Width - 0.5);
            double relposy = -2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).Y) / pictureBoxVideo.Size.Height - 0.5);
            double myRadiusInPx;

            switch (setup)
            {
                case SetupType.SetToolPos:		// set cross hairs position in fix camera mount
                    {
                        cameraZeroFixXInPx = e.X;// (float)(relMousePosX * cameraScaling);
                        cameraZeroFixYInPx = e.Y;// (float)(relMousePosY * cameraScaling);
                        StopSetup();
                        Properties.Settings.Default.cameraZeroFixX = cameraZeroFixXInPx;
                        Properties.Settings.Default.cameraZeroFixY = cameraZeroFixYInPx;
                        SetToolStrip(Color.Lime, string.Format("Set X:{0:0.000}  Y:{1:0.000}", cameraZeroFixXInPx, cameraZeroFixYInPx));
                        break;
                    }
                case SetupType.Distortion:
                    {
                        if (quadCounter < 4)
                        {
                            quadCorners.Add(new IntPoint(e.X, e.Y));
                            quadCounter++;
                            SetToolStrip(Color.Yellow, string.Format("{0} {1} / 4 : X:{2} Y:{3}", Localization.GetString("cameraDistortionMarker"), quadCounter, e.X, e.Y));
                            if (quadCounter == 4)
                            {
                                quadFilter = new QuadrilateralTransformation(quadCorners, pictureBoxVideo.Width, pictureBoxVideo.Height);
                                if (CameraMount == CameraMounting.Fix)
                                {
                                    Properties.Settings.Default.cameraDistortionFixP0 = ToPoint(quadCorners[0]);
                                    Properties.Settings.Default.cameraDistortionFixP1 = ToPoint(quadCorners[1]);
                                    Properties.Settings.Default.cameraDistortionFixP2 = ToPoint(quadCorners[2]);
                                    Properties.Settings.Default.cameraDistortionFixP3 = ToPoint(quadCorners[3]);
                                }
                                else
                                {
                                    Properties.Settings.Default.cameraDistortionXyP0 = ToPoint(quadCorners[0]);
                                    Properties.Settings.Default.cameraDistortionXyP1 = ToPoint(quadCorners[1]);
                                    Properties.Settings.Default.cameraDistortionXyP2 = ToPoint(quadCorners[2]);
                                    Properties.Settings.Default.cameraDistortionXyP3 = ToPoint(quadCorners[3]);
                                }

                                Properties.Settings.Default.cameraDistortionEnable = removeDistortion = true;
                                Properties.Settings.Default.Save();
                                distortionToolStripMenuItem.Checked = true;
                                SetCameraRotation();
                                StopSetup();
                            }
                        }
                        break;
                    }
                case SetupType.TeachFix:		// teach circle radius in fix camera mount
                    {
                        CalcScalingFix();
                        //		Logger.Trace(culture, "TeachFix radius X:{0} Y:{1} r:{2} scaling:{3} ", realPosition.X, realPosition.Y, myRadiusInPx, cameraScalingFix);
                        StopSetup();
                        break;
                    }
                case SetupType.TeachXy:			// teach circle radius in xy camera mount
                    {
                        CalcScalingXy();
                        //		Logger.Trace(culture, "TeachXy radius X:{0} Y:{1} r:{2} scaling:{3} ", realPosition.X, realPosition.Y, myRadiusInPx, cameraScalingXy);
                        StopSetup();
                        break;
                    }
                case SetupType.TeachXyzTop:		// teach circle radius in xyz camera mount
                    {
                        //    teachingTop = false;
                        myRadiusInPx = Math.Sqrt(relposx * relposx + relposy * relposy);	// normalized
                        if (myRadiusInPx > 0)
                        { cameraScalingXyzTop = cameraTeachRadiusXyzTop / myRadiusInPx; }
                        else
                            MessageBox.Show("Radius is '0', no update for scaling of upper position", "Attention");
                        cameraPosZTop = Grbl.posMachine.Z;
                        Logger.Trace(culture, "Teach top X:{0} Y:{1} r:{2} scaling:{3} pos-top:{4}", relposx, relposy, myRadiusInPx, cameraScalingXyzTop, cameraPosZTop);
                        StopSetup();
                        break;
                    }
                case SetupType.TeachXyzBot:		// teach circle radius in xyz camera mount
                    {
                        //     teachingBot = false;
                        myRadiusInPx = Math.Sqrt(relposx * relposx + relposy * relposy);
                        if (myRadiusInPx > 0)
                        { cameraScalingXyzBot = cameraTeachRadiusXyzBot / myRadiusInPx; }
                        else
                            MessageBox.Show("Radius is '0', no update for scaling of lower position", "Attention");
                        cameraPosZBot = Grbl.posMachine.Z;
                        Logger.Trace(culture, "Teach bottom X:{0} Y:{1} r:{2} scaling:{3} pos-bot:{4}", relposx, relposy, myRadiusInPx, cameraScalingXyzBot, cameraPosZBot);
                        StopSetup();
                        break;
                    }
                case SetupType.None:
                default:
                    {
                        XyPoint clickRealPos = realPosition;
                        if (CameraMount != CameraMounting.Fix)
                            clickRealPos = realPosition + (XyPoint)Grbl.posWork;// + realPosition;

                        if (Control.ModifierKeys == Keys.Control)   // set marker position in 2D view
                        {
                            DistanceByLine marker = VisuGCode.SetPosMarkerNearBy(clickRealPos, false);// - (XyPoint)Grbl.posWCO
                            Grbl.PosMarker = marker.actualPos;// - Grbl.posWCO;
                            VisuGCode.CreateMarkerPath();
                            refPointInPx = TranslateToPicCoordinate((XyPoint)Grbl.PosMarker);
                        }
                        else if (Control.ModifierKeys == Keys.Alt)   // set marker position in 2D view
                        {
                            Grbl.PosMarker = new XyzPoint(clickRealPos, 0);// - Grbl.posWCO;
                            VisuGCode.CreateMarkerPath();
                            refPointInPx = TranslateToPicCoordinate((XyPoint)Grbl.PosMarker);
                        }
                        else if (Control.ModifierKeys == Keys.Shift)   // set marker position in 2D view
                        {
                            realPoints.Add((XyPoint)clickRealPos);
                            setup = SetupType.TeachRefPoint;
                            ListRealPoints();
                            CbUseShapeRecognition.Checked = false;
                        }
                        else if (e.Button == MouseButtons.Left)
                        {
                            if ((int)CameraMount > 0)
                                OnRaiseXYEvent(new XYEventArgs(0, 1, realPosition, "G91")); // move relative and slow	-	MainFormGetCodeTransform.cs
                            else
                                OnRaiseXYEvent(new XYEventArgs(0, 1, realPosition, "G91")); // move absolute work
                        }
                        break;
                    }
            }
            pictureBoxVideo.Invalidate();
        }

        private void PictureBoxVideo_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left))
            {
                switch (setup)
                {
                    case SetupType.TeachFix:
                        {
                            CalcScalingFix();
                            pictureBoxVideo.Invalidate();
                            break;
                        }
                    case SetupType.TeachXy:
                        {
                            CalcScalingXy();
                            pictureBoxVideo.Invalidate();
                            break;
                        }
                }
            }
        }

        private void PictureBoxVideo_MouseDown(object sender, MouseEventArgs e)
        {
            //cmsPictureBox.Visible = true;
            lblAngle.BackColor = Color.Transparent;			// highlight
            measuredDistance = 0;

            if (e.Button == MouseButtons.Right)
            {
                measureAngleStart = (XyPoint)pictureBoxVideo.PointToClient(MousePosition);
                angleRotationCenter = (XyPoint)Grbl.posWork + realPosition;
                setup = SetupType.MeasureAngle;
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
            if (CameraMount == CameraMounting.Fix)
                return;
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
                measuredAngle = (float)measureAngleStart.AngleTo(measureAngleStop);
                measuredDistance = measureAngleStart.DistanceTo(measureAngleStop);

                System.Drawing.Point tmp = pictureBoxVideo.PointToClient(MousePosition);
                lastClickedCoordinate = TranslateFromPicCoordinate(new AForge.Point(tmp.X, tmp.Y));

                if (measuredDistance < 10)
                {
                    compensateAngleToolStripMenuItem.Visible = false;
                    setWork00ToHereToolStripMenuItem.Visible = true;
                }
                else
                {
                    compensateAngleToolStripMenuItem.Visible = true;
                    setWork00ToHereToolStripMenuItem.Visible = false;
                    tabControl1.SelectedIndex = 2;                      // show angle tab
                    lblAngle.BackColor = Color.Yellow;          // highlight
                                                                //        tabPage3.Select();
                }
                setup = SetupType.None;
                lblAngle.Text = String.Format("{0:0.00}°", measuredAngle);
            }
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

        #endregion

        protected virtual void OnRaiseXYEvent(XYEventArgs e)
        {
            RaiseXYEvent?.Invoke(this, e);
        }

        // change zooming
        private void NudCameraZoom_ValueChanged(object sender, EventArgs e)
        {
            cameraZoom = (float)nUDCameraZoom.Value;
        }


        // send event to teach Zero offset
        private void BtnSetOffsetZero_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(0, 0, 0, zeroCmd0)); // set new coordinates
        }
        // send event to teach Marker offset
        private void BtnSetOffsetMarker_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(0, 1, (XyPoint)Grbl.PosMarker, zeroCmd0));        // set new coordinates
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

        private void BtnApplyAngle_Click(object sender, EventArgs e)
        {
            if (cBRotateArround0.Checked)
                OnRaiseXYEvent(new XYEventArgs(measuredAngle, 1, new XyPoint(0, 0), "a"));
            else
                OnRaiseXYEvent(new XYEventArgs(measuredAngle, 1, angleRotationCenter + (XyPoint)Grbl.posWCO, "a"));

        }


        /******************************************************************************
        ***** manual shape detection setup
        ******************************************************************************/
        #region shapedetection
        private void CbShapeDetection_CheckedChanged(object sender, EventArgs e)
        {
            if (cBShapeDetection.Checked)
            {
                btnAutoCenter.Enabled = true;
                ListSettings();
            }
            else
                btnAutoCenter.Enabled = false;
        }



        private void FillComboBox()		// on Form load
        {
            if (comboBox1.InvokeRequired)
                comboBox1.Invoke((MethodInvoker)delegate
                {
                    FillComboBox();
                });
            else
            {
                string txt;
                comboBox1.Items.Clear();
                txt = Properties.Settings.Default.camShapeSet1;
                if (txt.IndexOf('|') < 1)
                { txt = Properties.Settings.Default.camShapeSet1 = "Paper|0|100|0|100|0|100|True|True|True|20|200|0.5|1|"; Logger.Error("FillComboBox set default for 1: {0}", txt); }
                comboBox1.Items.Add((string.IsNullOrEmpty(txt)) ? "not set" : txt.Substring(0, txt.IndexOf('|')));

                txt = Properties.Settings.Default.camShapeSet2;
                if (txt.IndexOf('|') < 1)
                { txt = Properties.Settings.Default.camShapeSet2 = "Paper2|0|100|0|100|0|100|True|True|True|20|200|0.5|1|"; Logger.Error("FillComboBox set default for 2: {0}", txt); }
                comboBox1.Items.Add((string.IsNullOrEmpty(txt)) ? "not set" : txt.Substring(0, txt.IndexOf('|')));

                txt = Properties.Settings.Default.camShapeSet3;
                if (txt.IndexOf('|') < 1)
                { txt = Properties.Settings.Default.camShapeSet3 = "PCB|0|150|0|150|0|150|False|False|True|20|100|0.5|1|"; Logger.Error("FillComboBox set default for 3: {0}", txt); }
                comboBox1.Items.Add((string.IsNullOrEmpty(txt)) ? "not set" : txt.Substring(0, txt.IndexOf('|')));

                txt = Properties.Settings.Default.camShapeSet4;
                if (txt.IndexOf('|') < 1)
                { txt = Properties.Settings.Default.camShapeSet4 = "Wood|0|106|0|237|123|246|True|True|False|10|100|0.5|2.0|"; Logger.Error("FillComboBox set default for 4: {0}", txt); }
                comboBox1.Items.Add((string.IsNullOrEmpty(txt)) ? "not set" : txt.Substring(0, txt.IndexOf('|')));

                Properties.Settings.Default.Save();

                byte selectedIndex = 0;
                if (CameraMount == CameraMounting.Fix)
                    selectedIndex = filterIndexFix = Properties.Settings.Default.cameraFilterIndexFix;
                else
                    selectedIndex = filterIndexXy = Properties.Settings.Default.cameraFilterIndexXy;

                if (selectedIndex >= comboBox1.Items.Count) { Logger.Error("FillComboBox stored index too high: {0}", selectedIndex); selectedIndex = 0; }
                comboBox1.SelectedIndex = (int)selectedIndex;
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte selectedIndex = (byte)comboBox1.SelectedIndex;
            if (selectedIndex == 0) { ShapeSetLoad(Properties.Settings.Default.camShapeSet1); }
            if (selectedIndex == 1) { ShapeSetLoad(Properties.Settings.Default.camShapeSet2); }
            if (selectedIndex == 2) { ShapeSetLoad(Properties.Settings.Default.camShapeSet3); }
            if (selectedIndex == 3) { ShapeSetLoad(Properties.Settings.Default.camShapeSet4); }
            if (CameraMount == CameraMounting.Fix)
                Properties.Settings.Default.cameraFilterIndexFix = filterIndexFix = selectedIndex;
            else
                Properties.Settings.Default.cameraFilterIndexXy = filterIndexXy = selectedIndex;
        }

        #endregion


        private void ControlCameraForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            { showOverlay = false; }
            else if (e.KeyCode == Keys.A)       // add to fiducial list
            {
                VisuGCode.fiducialsCenter.Add(new XyPoint((XyPoint)Grbl.PosMarker));
                ListFiducials();
            }
            else if (e.KeyCode == Keys.C)       // clear fiducial list
            {
                if (e.Modifiers == Keys.Shift)
                {
                    realPoints.Clear();
                    ListRealPoints();
                }
                else
                {
                    VisuGCode.fiducialsCenter.Clear();
                    ListFiducials();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (setup == SetupType.Distortion)
                {
                    Properties.Settings.Default.cameraDistortionEnable = removeDistortion = false;
                    distortionToolStripMenuItem.Checked = false;
                    quadCounter = 0;
                }
                setup = SetupType.None;
                ResetToolStrip();
            }
        }

        private void ControlCameraForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            { showOverlay = true; }
        }

        /*******************************************************************
        ***** Right click menu picturebox
        *******************************************************************/
        #region rightClickMenu

        private void SetWork00ToHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CameraMount == CameraMounting.Fix)
                OnRaiseXYEvent(new XYEventArgs(0, 1, new XyPoint(-lastClickedCoordinate.X + Grbl.posWork.X, -lastClickedCoordinate.Y + Grbl.posWork.Y), "G10 L20 P0"));        // set new coordinates
                                                                                                                                                                               //          OnRaiseXYEvent(new XYEventArgs(0, 1, lastClickedCoordinate, "G10 L2 P0"));        // set new coordinates
            else
                OnRaiseXYEvent(new XYEventArgs(0, 1, new XyPoint(-lastClickedCoordinate.X, -lastClickedCoordinate.Y), "G10 L20 P0"));        // set new coordinates

        }

        private void SetAngleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BtnApplyAngle_Click(sender, e);
        }

        /*****************************************************************************/
        private void FiducialAddCoordinateIn2DViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;                      // show fiducial list
            XyPoint clickRealPos = lastClickedCoordinate;
            if (CameraMount != CameraMounting.Fix)
                clickRealPos = lastClickedCoordinate + (XyPoint)Grbl.posWork;// + realPosition;
            Grbl.PosMarker = new XyzPoint(clickRealPos, 0);// - Grbl.posWCO;
            VisuGCode.fiducialsCenter.Add(new XyPoint((XyPoint)Grbl.PosMarker));
            VisuGCode.CreateMarkerPath();
            refPointInPx = TranslateToPicCoordinate((XyPoint)Grbl.PosMarker);
        }

        private void FiducialRemoveLastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;						// show fiducial list
            if (VisuGCode.fiducialsCenter.Count > 0)
                VisuGCode.fiducialsCenter.RemoveAt(VisuGCode.fiducialsCenter.Count - 1);
            ListFiducials();
        }

        private void FiducialListClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;                      // show fiducial list
            VisuGCode.fiducialsCenter.Clear();
            ListFiducials();
        }
        /*****************************************************************************/
        private void ReferenceAddPointInImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;                      // show fiducial list
            XyPoint clickRealPos = lastClickedCoordinate;
            if (CameraMount != CameraMounting.Fix)
                clickRealPos = lastClickedCoordinate + (XyPoint)Grbl.posWork;// + realPosition;
            realPoints.Add((XyPoint)clickRealPos);
            setup = SetupType.TeachRefPoint;
            ListRealPoints();
            CbUseShapeRecognition.Checked = false;
        }

        private void ReferenceRemoveLastPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;						// show fiducial list
            if (realPoints.Count > 0)
                realPoints.RemoveAt(realPoints.Count - 1);
            ListRealPoints();
        }

        private void ReferenceClearListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;                      // show fiducial list
            realPoints.Clear();
            ListRealPoints();
        }
        /*****************************************************************************/
        private void ShowOverlayGraphicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showOverlay = !showOverlay;
            showOverlayGraphicsToolStripMenuItem.Checked = showOverlay;
        }


        #endregion


        // called after teaching
        private void StopSetup()
        {
            setup = SetupType.None;
            ResetToolStrip();
        }
        private void ResetToolStrip()
        {
            toolStripStatusLabel1.Text = "";
            toolStripStatusLabel1.BackColor = SystemColors.Control;
        }

        private void SetToolStrip(Color col, string txt, bool log = false)
        {
            toolStripStatusLabel1.Text = txt;
            toolStripStatusLabel1.BackColor = col;
            if (log)
            { Logger.Info("{0}", txt); }
        }



        /**************************************
         *** Menu Setup ***
         **************************************/
        #region setupmenu
        // select camera mounting
        private void ToolStripCameraMount_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            ((ToolStripMenuItem)cameraMountToolStripMenuItem.DropDownItems[(int)CameraMount]).Checked = false;
            clickedItem.Checked = true;
            CameraMount = (CameraMounting)clickedItem.Tag;
            ((ToolStripMenuItem)cameraMountToolStripMenuItem.DropDownItems[(int)CameraMount]).Checked = true;

            this.Text = Localization.GetString(mountingText[(int)CameraMount]);
            SetToolStrip(Color.White, string.Format("{0} {1} use cam '{2}'", Localization.GetString("cameraSetMount"), mountingText[(int)CameraMount], cameraIndexName));

            if (useFreezFrame)
            {
                if ((exampleFrameFix == null) || (exampleFrameXy == null))
                    LoadExampleImages();
                if (CameraMount == CameraMounting.Fix)
                    exampleFrame = exampleFrameFix;
                else
                    exampleFrame = exampleFrameXy;

                timerFreezFrame.Start();
            }
            else
                timerFreezFrame.Stop();

            if ((cameraIndex >= 0) && (cameraIndex < videosources.Count))
                ((ToolStripMenuItem)camSourceToolStripMenuItem.DropDownItems[cameraIndex]).Checked = false;
            SetCameraMountDependence();
            if ((cameraIndex < 0) || (cameraIndex >= videosources.Count))
                cameraIndex = 0;
            frameCounter = 0;
            if (camSourceToolStripMenuItem.DropDownItems.Count > 0)
                ((ToolStripMenuItem)camSourceToolStripMenuItem.DropDownItems[cameraIndex]).Checked = true;
            SelectCameraSource(cameraIndex, cameraResolutionX);
            SaveCameraDependence();
            SetMenuVisibility();
            cameraZoom = 640 / cameraResolutionX;
            nUDCameraZoom.Value = (decimal)cameraZoom;
        }
        private void SetMenuVisibility()
        {
            if (CameraMount == CameraMounting.Fix)
            {
                toolStripTextBox4.Text = cameraTeachRadiusFix.ToString();
                teachScalingToolStripMenuItem.Visible = false;
                teachScaling2ToolStripMenuItem.Visible = true;
                teachOffsetToolStripMenuItem.Visible = false;       // G54
                toolPositionToolStripMenuItem.Visible = true;
                GbZoom.Visible = false;
                if (tabControl1.Controls.Contains(tabPage4)) tabControl1.Controls.Remove(tabPage4);
            }
            else if (CameraMount == CameraMounting.MoveXY)
            {
                toolStripTextBox4.Text = cameraTeachRadiusXy.ToString();
                teachScalingToolStripMenuItem.Visible = false;
                teachScaling2ToolStripMenuItem.Visible = true;
                teachOffsetToolStripMenuItem.Visible = false;
                toolPositionToolStripMenuItem.Visible = false;
                GbZoom.Visible = true;
                if (tabControl1.Controls.Contains(tabPage4)) tabControl1.Controls.Remove(tabPage4);
            }
            else if (CameraMount == CameraMounting.MoveXYZ)
            {
                teachScalingToolStripMenuItem.Visible = true;
                teachScaling2ToolStripMenuItem.Visible = false;
                teachOffsetToolStripMenuItem.Visible = true;
                toolPositionToolStripMenuItem.Visible = false;
                GbZoom.Visible = true;
                if (!tabControl1.Controls.Contains(tabPage4)) tabControl1.Controls.Add(tabPage4);
            }
        }


        // select video source from list
        private void CamSourceSubmenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            ((ToolStripMenuItem)camSourceToolStripMenuItem.DropDownItems[cameraIndex]).Checked = false;
            cameraIndex = Convert.ToByte(clickedItem.Tag, culture);     // (byte)clickedItem.Tag;
            cameraIndexName = videosources[cameraIndex].Name;
            clickedItem.Checked = true;
            SelectCameraSource(cameraIndex, cameraResolutionX);
            SaveCameraDependence();
        }
        // try to set video-source, set event-handler
        private void SelectCameraSource(int index, int resolution)
        {
            try
            {
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SelectCameraSource - close source ");
            }

			if (index >= videosources.Count)
			{	index = videosources.Count - 1;}
		
			if (index < 0)
				return;
			
			videoSource = new VideoCaptureDevice(videosources[index].MonikerString);
			((ToolStripMenuItem)camSourceToolStripMenuItem.DropDownItems[index]).Checked = true;

            try
            {
                int i;
                if (videoSource.VideoCapabilities.Length > 0)
                {
                    for (i = 0; i < videoSource.VideoCapabilities.Length; i++)
                    {
                        Logger.Trace("SelectCameraSource {0}) width:{1} height:{2}", i, videoSource.VideoCapabilities[i].FrameSize.Width, videoSource.VideoCapabilities[i].FrameSize.Height);
                        if (videoSource.VideoCapabilities[i].FrameSize.Width == resolution)
                        {
                            videoSource.VideoResolution = videoSource.VideoCapabilities[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SelectCameraSource - open source ");
            }
            videoSource.NewFrame += new AForge.Video.NewFrameEventHandler(VideoSource_NewFrame);
            videoSource.Start();
        }

        // change camera rotation
        private void ToolStripTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetCameraRotation();
            }
        }

        private void SetCameraRotation()
        {
            if (!double.TryParse(toolStripTextBox1.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraRotation))
                cameraRotation = 0;
            toolStripTextBox1.Text = cameraRotation.ToString();
        }

        // click on check-symbol
        private void DistortionToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            removeDistortion = distortionToolStripMenuItem.Checked;
            pictureBoxVideo.Invalidate();
        }

        #region teachPosition
        // teach distortion		
        private void ResetAndTeachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            quadFilter = null;
            Properties.Settings.Default.cameraDistortionEnable = removeDistortion = false;
            distortionToolStripMenuItem.Checked = false;
            setup = SetupType.Distortion;
            quadCounter = 0;
            quadCorners.Clear();
            cameraRotation = 0;
            SetToolStrip(Color.Yellow, Localization.GetString("cameraSetDistortion"));
        }

        // teach cross hairs position
        private void SetToolPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setup = SetupType.SetToolPos;
            SetToolStrip(Color.Yellow, Localization.GetString("cameraSetCrossHairs"));
        }
        private void SetMachinePositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.cameraZeroFixMachineX = Grbl.posMachine.X;
            Properties.Settings.Default.cameraZeroFixMachineY = Grbl.posMachine.Y;
            SetToolStrip(Color.Lime, string.Format("posMachine X:{0} Y:{1}", Grbl.posMachine.X, Grbl.posMachine.Y));
        }

        // activate teaching of camera view range upper position
        private void UpperPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setup = SetupType.TeachXyzTop;
            SetToolStrip(Color.Yellow, string.Format("{0} {1:0.00} {2}. {3}", Localization.GetString("cameraTeachRadius"), cameraTeachRadiusXyzTop, Localization.GetString("cameraForUpperPos"), Localization.GetString("cameraQuit")));
        }
        // activate teaching of camera view range lower position
        private void LowerPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setup = SetupType.TeachXyzBot;
            SetToolStrip(Color.Yellow, string.Format("{0} {1:0.00} {2}. {3}", Localization.GetString("cameraTeachRadius"), cameraTeachRadiusXyzBot, Localization.GetString("cameraForLowerPos"), Localization.GetString("cameraQuit")));
        }

        // activate teaching of camera view range fix or Xy
        private void TeachRadiusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CameraMount == CameraMounting.Fix)
            {
                setup = SetupType.TeachFix;
                SetToolStrip(Color.Yellow, string.Format("{0} {1:0.00}. {2}", Localization.GetString("cameraTeachRadius"), cameraTeachRadiusFix, Localization.GetString("cameraQuit")));
            }
            else
            {
                setup = SetupType.TeachXy;
                SetToolStrip(Color.Yellow, string.Format("{0} {1:0.00}. {2}", Localization.GetString("cameraTeachRadius"), cameraTeachRadiusXy, Localization.GetString("cameraQuit")));
            }
            pictureBoxVideo.Invalidate();
        }

        // get set radius for cameraXyz top, bot
        private void ToolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            double old = cameraTeachRadiusXyzTop;
            if (!Double.TryParse(toolStripTextBox2.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraTeachRadiusXyzTop))
                cameraTeachRadiusXyzTop = old;
            if (cameraTeachRadiusXyzTop <= 0)
                cameraTeachRadiusXyzTop = 20;
        }
        private void ToolStripTextBox2_Leave(object sender, EventArgs e)
        {
            toolStripTextBox2.Text = cameraTeachRadiusXyzBot.ToString(culture);
        }
        private void ToolStripTextBox3_TextChanged(object sender, EventArgs e)
        {
            double old = cameraTeachRadiusXyzBot;
            if (!Double.TryParse(toolStripTextBox3.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cameraTeachRadiusXyzBot))
                cameraTeachRadiusXyzBot = old;
            if (cameraTeachRadiusXyzBot <= 0)
                cameraTeachRadiusXyzBot = 10;
        }
        private void ToolStripTextBox3_Leave(object sender, EventArgs e)
        {
            toolStripTextBox3.Text = cameraTeachRadiusXyzBot.ToString(culture);
        }

        // get set radius for cameraFix and -Xy
        private void ToolStripTextBox4_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(toolStripTextBox4.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double newVal))
            { newVal = 20; toolStripTextBox4.Text = newVal.ToString(); }
            if (newVal <= 0)
            { newVal = 20; toolStripTextBox4.Text = newVal.ToString(); }
            if (CameraMount == CameraMounting.Fix)
                cameraTeachRadiusFix = newVal;
            else
                cameraTeachRadiusXy = newVal;
        }
        private void ToolStripTextBox4_Leave(object sender, EventArgs e)
        {
            if (CameraMount == CameraMounting.Fix)
                toolStripTextBox4.Text = cameraTeachRadiusFix.ToString();
            else
                toolStripTextBox4.Text = cameraTeachRadiusXy.ToString();
        }
        #endregion

        #region colors
        private void TextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyColor(textToolStripMenuItem, "cameraColorText");
            brushText = new SolidBrush(Properties.Settings.Default.cameraColorText);
        }
        private void CrossHairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyColor(crossHairsToolStripMenuItem, "cameraColorCross");
            colCross = Properties.Settings.Default.cameraColorCross;
            penTeachMark = new Pen(colCross, 1f);
        }

        private void ApplyColor(ToolStripMenuItem btn, string settings)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                SetButtonColors(btn, colorDialog1.Color);
                Properties.Settings.Default[settings] = colorDialog1.Color;
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

        #endregion

        #endregion // menu setup


        // TabControl draw text on horizontal tabs
        private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;
            TabPage _tabPage = tabControl1.TabPages[e.Index];
            Rectangle _tabBounds = tabControl1.GetTabRect(e.Index);
            if (e.State == DrawItemState.Selected)
            {
                _textBrush = new SolidBrush(Color.Black);
                if (!e.Bounds.IsEmpty)
                    g.FillRectangle(Brushes.LightYellow, e.Bounds);
            }
            else
            {
                _textBrush = new System.Drawing.SolidBrush(e.ForeColor);
                if (!e.Bounds.IsEmpty)
                    g.FillRectangle(SystemBrushes.Control, e.Bounds);
            }
            Font _tabFont = new Font("Microsoft Sans Serif", 8f, FontStyle.Regular, GraphicsUnit.Point);
            StringFormat _stringFlags = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }
    }
	
    public partial class IniFile
    {
        internal static string sectionCamera = "Camera";
        internal static string[,] keyValueCamera = {
            {"Fix Index",      		"cameraIndexFix"},
            {"Fix Rotation",		"cameraRotationFix"},
            {"Fix Radius",			"cameraTeachRadiusFix"},
            {"Fix Scaling",			"cameraScalingFix"},
            {"Fix Offset X",		"cameraZeroFixX"},
            {"Fix Offset Y",		"cameraZeroFixY"},
			
            {"XY Index",      		"cameraIndexXy"},
            {"XY Rotation",			"cameraRotationXy"},
            {"XY Radius",			"cameraTeachRadiusXy"},
            {"XY Scaling",			"cameraScalingXy"},
            {"XY Offset X",			"cameraToolOffsetX"},
            {"XY Offset Y",			"cameraToolOffsetY"},
            {"XY Top Pos",			"cameraPosTop"},
            {"XY Top Radius",		"cameraTeachRadiusXyzTop"},
            {"XY Top Scaling",		"cameraScalingXyzTop"},
            {"XY Bottom Pos",		"cameraPosBot"},
            {"XY Bottom Radius",	"cameraTeachRadiusXyzBot"},
            {"XY Bottom Scaling",	"cameraScalingXyzBot"},
			
            {"Fiducial Name",		"importFiducialLabel"},
			{"Fiducial Skip",		"importFiducialSkipCode"},
            {"Parameter Set 1",		"camShapeSet1"},
            {"Parameter Set 2",		"camShapeSet2"},
            {"Parameter Set 3",		"camShapeSet3"},
            {"Parameter Set 4",		"camShapeSet4"}			
		};
	}
}
