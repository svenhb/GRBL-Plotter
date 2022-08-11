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
/*
    Thanks to http://code-bude.net/2011/06/02/webcam-benutzen-in-csharp/
*/
/* 
 * 2022-01-21 line 461 if ((realPoints.Count < 2) || (VisuGCode.fiducialsCenter.Count < 2))
 * 2022-03-23 listSettings()
*/

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
// AForge Library http://www.aforgenet.com/framework/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlCameraForm : Form
    {
        private enum CameraMounting { Fix = 0, MoveXY = 1, MoveXYZ = 2 };
        private readonly string[] mountingText = { "cameraMountFix", "cameraMountXy", "cameraMountXyz", "", "" };
        private CameraMounting CameraMount = CameraMounting.MoveXY;
        private enum SetupType { None, Distortion, TeachFix, TeachXy, TeachXyzTop, TeachXyzBot, MeasureAngle, SetToolPos, TeachRefPoint };	// TeachFix, TeachXy, TeachXyzTop
        private SetupType setup = SetupType.None;

        // distortion
        private static int quadCounter = 0;
        private static QuadrilateralTransformation quadFilter;
        private static readonly List<IntPoint> quadCorners = new List<IntPoint>();
        private static bool removeDistortion = true;

        // measure angle 
        private static XyPoint measureAngleStart = new XyPoint(0, 0);
        private static XyPoint measureAngleStop = new XyPoint(0, 0);
        private static XyPoint angleRotationCenter = new XyPoint(0, 0);
        private static readonly List<XyPoint> realPoints = new List<XyPoint>();
        private static XyPoint realPointsOffset = new XyPoint();

        private static readonly bool showLog = false;
        private System.Drawing.Point ToPoint(IntPoint tmp)
        { return new System.Drawing.Point(tmp.X, tmp.Y); }
        private AForge.IntPoint ToIntPoint(System.Drawing.Point tmp)
        { return new AForge.IntPoint(tmp.X, tmp.Y); }

        private readonly Pen penTeach = new Pen(Color.LightPink, 0.5F);
        private readonly Pen penUp = new Pen(Color.Green, 0.1F);
        private readonly Pen penDown = new Pen(Color.Red, 0.4F);
        private readonly Pen penRuler = new Pen(Color.Blue, 0.1F);
        private readonly Pen penTool = new Pen(Color.Black, 0.5F);
        private readonly Pen penMarker = new Pen(Color.DeepPink, 1F);
        private readonly Pen penDimension = new Pen(Color.DarkGray, 1F);
        private HatchBrush brushMachineLimit = new HatchBrush(HatchStyle.Horizontal, Color.Yellow);

        private void SaveCameraDependence()
        {
            Properties.Settings.Default.cameraMount = (int)CameraMount;
            if (CameraMount == CameraMounting.Fix)
            {
                Properties.Settings.Default.cameraIndexFix = cameraIndex;
                Properties.Settings.Default.cameraIndexNameFix = cameraIndexName;
                Properties.Settings.Default.cameraRotationFix = cameraRotation;
            }
            else
            {
                Properties.Settings.Default.cameraIndexXy = cameraIndex;
                Properties.Settings.Default.cameraIndexNameXy = cameraIndexName;
                Properties.Settings.Default.cameraRotationXy = cameraRotation;
            }
            Properties.Settings.Default.Save();
        }
        private void SetCameraMountDependence()
        {
            if (CameraMount == CameraMounting.Fix)
            {
                cameraIndex = Properties.Settings.Default.cameraIndexFix;
                cameraIndexName = Properties.Settings.Default.cameraIndexNameFix;
                cameraIndex = GetMatchingCamIndex(cameraIndex, cameraIndexName);
                toolStripTextBox4.Text = cameraTeachRadiusFix.ToString();

                cameraRotation = Properties.Settings.Default.cameraRotationFix;
                quadCorners.Clear();
                quadCounter = 0;
                quadCorners.Add(ToIntPoint(Properties.Settings.Default.cameraDistortionFixP0));
                quadCorners.Add(ToIntPoint(Properties.Settings.Default.cameraDistortionFixP1));
                quadCorners.Add(ToIntPoint(Properties.Settings.Default.cameraDistortionFixP2));
                quadCorners.Add(ToIntPoint(Properties.Settings.Default.cameraDistortionFixP3));
                quadFilter = new QuadrilateralTransformation(quadCorners, pictureBoxVideo.Width, pictureBoxVideo.Height);
                filterIndex = filterIndexFix;
            }
            else
            {
                cameraIndex = Properties.Settings.Default.cameraIndexXy;
                cameraIndexName = Properties.Settings.Default.cameraIndexNameXy;
                cameraIndex = GetMatchingCamIndex(cameraIndex, cameraIndexName);
                toolStripTextBox4.Text = cameraTeachRadiusXy.ToString();

                cameraRotation = Properties.Settings.Default.cameraRotationXy;
                quadCorners.Clear();
                quadCounter = 0;
                quadCorners.Add(ToIntPoint(Properties.Settings.Default.cameraDistortionXyP0));
                quadCorners.Add(ToIntPoint(Properties.Settings.Default.cameraDistortionXyP1));
                quadCorners.Add(ToIntPoint(Properties.Settings.Default.cameraDistortionXyP2));
                quadCorners.Add(ToIntPoint(Properties.Settings.Default.cameraDistortionXyP3));
                quadFilter = new QuadrilateralTransformation(quadCorners, pictureBoxVideo.Width, pictureBoxVideo.Height);
                filterIndex = filterIndexXy;
            }
            toolStripTextBox2.Text = String.Format(culture, "{0}", cameraTeachRadiusXyzTop);
            toolStripTextBox3.Text = String.Format(culture, "{0}", cameraTeachRadiusXyzBot);

            if (filterIndex >= comboBox1.Items.Count) { filterIndex = 0; }
            comboBox1.SelectedIndex = filterIndex;

            if (cameraRotation > 360) cameraRotation = 0;
            toolStripTextBox1.Text = String.Format(culture, "{0}", cameraRotation);
            if (showLog) Logger.Trace("SetCameraMountDependence mount:{0} index:{1} name:'{2}' rotation:{3:0.0}", CameraMount, cameraIndex, cameraIndexName, cameraRotation);
        }
        private byte GetMatchingCamIndex(byte index, string camName)
        {
            try
            {
                if (camName != videosources[index].Name)
                {
                    Logger.Error("GetMatchingCamIndex camIndex:{0}  assosiatedName:'{1}'  storedName:'{2}'", index, videosources[index].Name, camName);
                    for (byte i = 0; i < videosources.Count; i++)
                    {
                        if (camName == videosources[i].Name)
                        {
                            return i;
                            //        break;
                        }
                    }
                }
                return index;
            }
            catch (Exception err) { Logger.Error(err, "GetMatchingCamIndex "); return index; }
        }
        private void SetPens()
        {
            penTeachMark = new Pen(colCross, 1f);

            penUp.Color = Properties.Settings.Default.gui2DColorPenUp;
            penDown.Color = Properties.Settings.Default.gui2DColorPenDown;
            penRuler.Color = Properties.Settings.Default.gui2DColorRuler;
            penTool.Color = Properties.Settings.Default.gui2DColorTool;
            penMarker.Color = Properties.Settings.Default.gui2DColorMarker;
            penDimension.Color = Properties.Settings.Default.gui2DColorDimension;
            penRuler.Width = (float)Properties.Settings.Default.gui2DWidthRuler;
            penUp.Width = (float)Properties.Settings.Default.gui2DWidthPenUp;
            penDown.Width = (float)Properties.Settings.Default.gui2DWidthPenDown;
            penTool.Width = (float)Properties.Settings.Default.gui2DWidthTool;
            penMarker.Width = (float)Properties.Settings.Default.gui2DWidthMarker;
            penDimension.LineJoin = LineJoin.Round;
            penDimension.Width = 2 * (float)Properties.Settings.Default.gui2DWidthPenDown;
            brushMachineLimit = new HatchBrush(HatchStyle.DiagonalCross, Properties.Settings.Default.gui2DColorMachineLimit, Color.Transparent);
        }
        private void CalcScalingFix()
        {
            double relMousePosX = Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).X - cameraZeroFixXInPx);
            double relMousePosY = Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).Y - cameraZeroFixYInPx);
            double myRadiusInPx = Math.Sqrt(relMousePosX * relMousePosX + relMousePosY * relMousePosY);
            if (myRadiusInPx > 0)
            { cameraScalingFix = myRadiusInPx / cameraTeachRadiusFix; }
            SetToolStrip(Color.Lime, string.Format("{0}:{1:0} px  {2}:{3:0.000}", Localization.GetString("cameraSetRadius"), myRadiusInPx, Localization.GetString("cameraScaling"), cameraScalingFix), true);
        }
        private void CalcScalingXy()
        {
            double relposx = 2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).X) / pictureBoxVideo.Size.Width - 0.5);
            double relposy = -2 * (Convert.ToDouble(pictureBoxVideo.PointToClient(MousePosition).Y) / pictureBoxVideo.Size.Height - 0.5);
            double myRadiusInPx = Math.Sqrt(relposx * relposx + relposy * relposy);
            if (myRadiusInPx > 0)
            { cameraScalingXy = cameraTeachRadiusXy / myRadiusInPx; }
            SetToolStrip(Color.Lime, string.Format("{0}:{1:0} px  {2}:{3:0.000}", Localization.GetString("cameraSetRadius"), myRadiusInPx, Localization.GetString("cameraScaling"), cameraScalingFix), true);
        }

        private double GetActualScaling()
        {
            if (CameraMount == CameraMounting.Fix)
                return cameraScalingFix;
            else if (CameraMount == CameraMounting.MoveXY)
                return cameraScalingXy;

            double diff = cameraPosZTop - cameraPosZBot;
            if (diff == 0) diff = 1;
            double m = (cameraScalingXyzTop - cameraScalingXyzBot) / diff;
            double n = cameraScalingXyzTop - m * cameraPosZTop;
            double actualScaling = Math.Abs(Grbl.posMachine.Z * m + n);
            if (actualScaling < 1) actualScaling = 1;
            return actualScaling;
        }

        private void BtnAutoCenter_Click(object sender, EventArgs e)
        {
            if (shapeFound)
            {
                XyPoint tmp;
                //        double actualScaling = GetActualScaling();
                if (CameraMount == CameraMounting.Fix)
                {
                    tmp = TranslateFromPicCoordinate(shapeCenterInPx);
                    OnRaiseXYEvent(new XYEventArgs(0, 1, tmp, "G0G90 "));        // set new coordinates
                    realPoints.Add(tmp);
                    ListRealPoints();
                }
                else
                {
                    tmp = TranslateFromPicCoordinate(shapeCenterInPx);
                    OnRaiseXYEvent(new XYEventArgs(0, 1, tmp, "G91")); // move relative and slow
                }
                lblCenterPos.Text = string.Format("X: {0:0.00}  Y: {1:0.00}", tmp.X, tmp.Y);
            }
            else
                lblCenterPos.Text = "No shape found";
        }

        private XyPoint TranslateFromPicCoordinate(System.Drawing.Point tmp)
        { return TranslateFromPicCoordinate(new AForge.Point(tmp.X, tmp.Y)); }
        private XyPoint TranslateFromPicCoordinate(AForge.Point picCoordinate)
        {
            XyPoint tmp = new XyPoint();
            double actualScaling = GetActualScaling();
            if (CameraMount == CameraMounting.Fix)
            {
                tmp.X = Convert.ToDouble(picCoordinate.X - cameraZeroFixXInPx) / actualScaling / cameraZoom;// - Properties.Settings.Default.cameraZeroFixMachineX;
                tmp.Y = (cameraZeroFixYInPx - Convert.ToDouble(picCoordinate.Y)) / actualScaling / cameraZoom;// - Properties.Settings.Default.cameraZeroFixMachineY;
                XyPoint fixMachineOffset = new XyPoint(Properties.Settings.Default.cameraZeroFixMachineX, Properties.Settings.Default.cameraZeroFixMachineY);
                tmp += fixMachineOffset - (XyPoint)Grbl.posWCO;
            }
            else
            {
                tmp.X = 2 * (Convert.ToDouble(picCoordinate.X) / pictureBoxVideo.Size.Width - 0.5) * actualScaling / cameraZoom;
                tmp.Y = -2 * (Convert.ToDouble(picCoordinate.Y) / pictureBoxVideo.Size.Height - 0.5) * actualScaling * ratio / cameraZoom;
            }
            return tmp;
        }

        private AForge.Point TranslateToPicCoordinate(XyPoint realCoordinate)
        {
            AForge.Point tmp = new AForge.Point();

            double actualScaling = GetActualScaling();
            if (CameraMount == CameraMounting.Fix)
            {
                XyPoint fixMachineOffset = new XyPoint(Properties.Settings.Default.cameraZeroFixMachineX, Properties.Settings.Default.cameraZeroFixMachineY);
                tmp.X = cameraZeroFixXInPx + (float)(actualScaling * (realCoordinate.X + Grbl.posWCO.X - fixMachineOffset.X));
                tmp.Y = cameraZeroFixYInPx - (float)(actualScaling * (realCoordinate.Y + Grbl.posWCO.Y - fixMachineOffset.Y));
            }
            else
            {
                tmp.X = (float)(2 * (realCoordinate.X / pictureBoxVideo.Size.Width - 0.5) * actualScaling / cameraZoom);
                tmp.Y = (float)(-2 * (realCoordinate.Y / pictureBoxVideo.Size.Height - 0.5) * actualScaling * ratio / cameraZoom);

            }
            return tmp;
        }
        private int fiducialDetectionProgressCounter = 0;
        private int fiducialDetectionGrblNotIdleCounter = 0;
        private int fiducialDetectionFail = 0;
        private readonly int fiducialDetectionFailMax = 3;
        private bool fiducialDetection = false;
        private XyPoint realPos1, realPos2;
        private void BtnStartAutomatic_Click(object sender, EventArgs e)
        {
            cBShapeDetection.Checked = false;
            cBShapeShowFilter.Checked = false;
            fiducialDetectionProgressCounter = 1;
            fiducialDetectionGrblNotIdleCounter = 0;
            VisuGCode.MarkSelectedFigure(-1);
            if (CbUseShapeRecognition.Checked)
                timerFlowControl.Interval = 1000;
            else
                timerFlowControl.Interval = 200;

            frameCounterMax = 5;
            timerFlowControl.Start();
        }

        private void TimerFlowControl_Tick(object sender, EventArgs e)	// timer flow control
        {
            AutomaticFiducialDetection();
        }
        private void ListFiducials()
        {
            TbSetPoints.Text = "Fiducial coordinates (mm):\r\n";
            int i = 1;
            foreach (XyPoint tmp in VisuGCode.fiducialsCenter)
            { TbSetPoints.Text += string.Format("{0}) X:{1:0.0} Y:{2:0.0}\r\n", (i++), tmp.X, tmp.Y); }
        }
        private void ListRealPoints()
        {
            TbRealPoints.Text = "Assigned coordinates (mm):\r\n";
            int i = 1;
            foreach (XyPoint tmp in realPoints)
            { TbRealPoints.Text += string.Format("{0}) X:{1:0.0} Y:{2:0.0}\r\n", (i++), tmp.X, tmp.Y); }
        }
        private void AutomaticFiducialDetection()       // flow control, triggered by timer
        {
            if (fiducialDetectionProgressCounter == 0)
                return;
            switch (fiducialDetectionProgressCounter)
            {
                case 1: // set teachpoint - find actual position
                    {
                        if (VisuGCode.fiducialsCenter.Count == 0)
                        {
                            SetToolStrip(Color.Fuchsia, "Fiducial detection: No list of fiducials from GCode - STOP automatic", true);
                            TbSetPoints.Text = "No list of fiducials from GCode";
                            fiducialDetectionProgressCounter = 0;
                            frameCounterMax = 10;
                            break;
                        }
                        if (!CbUseShapeRecognition.Checked)
                        {
                            if (realPoints.Count == 0)
                            {
                                SetToolStrip(Color.Fuchsia, "Fiducial detection: No list of manual assigned points - STOP automatic", true);
                                TbSetPoints.Text = "No list of manual assigned points";
                                fiducialDetectionProgressCounter = 0;
                                frameCounterMax = 10;
                                break;
                            }
                        }

                        ListFiducials();

                        refPointInPx = TranslateToPicCoordinate(VisuGCode.fiducialsCenter[0]);
                        if (CameraMount != CameraMounting.Fix)
                            //   refPointInPx -= new AForge.Point((float)Grbl.posWork.X, (float)Grbl.posWork.Y);
                            refPointInPx = new AForge.Point((float)pictureBoxVideo.Size.Width / 2, (float)pictureBoxVideo.Size.Height / 2);

                        shapeCenterInPx = refPointInPx; // default

                        teachPoint1 = (XyPoint)VisuGCode.fiducialsCenter[0];
                        Grbl.PosMarker = new XyzPoint((XyPoint)VisuGCode.fiducialsCenter[0], 0);
                        SetToolStrip(Color.Lime, string.Format("Fiducial detection: 1) TP1/mm X:{0:0.00} Y:{1:0.00}  TP1/px X:{2:0.00} Y:{3:0.00}  count:{4}", teachPoint1.X, teachPoint1.Y, refPointInPx.X, refPointInPx.Y, VisuGCode.fiducialsCenter.Count), true);
                        if (showLog) Logger.Trace("Fiducial detection: 1) picPx X:{0:0.00} Y:{1:0.00}", refPointInPx.X, refPointInPx.Y);

                        VisuGCode.CreateMarkerPath();
                        fiducialDetectionFail = 0;

                        if (CbUseShapeRecognition.Checked)
                        {
                            //               fiducialDetection = true;       // activate shape detection
                            shapeFound = false;
                            if (CameraMount == CameraMounting.Fix)
                            {
                                fiducialDetection = true;               // activate shape detection
                                fiducialDetectionProgressCounter += 2;  // jump over "wait for idle"
                            }
                            else
                            {
                                OnRaiseXYEvent(new XYEventArgs(0, 1, teachPoint1, "G90 G0"));   // move to fiducial position if automatic recognition for more accuracy
                                if (showLog) Logger.Trace("Move to X:{0}  Y:{1}", teachPoint1.X, teachPoint1.Y);
                                fiducialDetectionProgressCounter++;
                            }
                        }
                        else
                            fiducialDetectionProgressCounter += 2;  // jump over "wait for idle"
                        break;
                    }
                case 2: // still moving?
                    {
                        if (Grbl.Status != GrblState.idle)
                        {
                            if (fiducialDetectionGrblNotIdleCounter++ > 10)
                            {
                                fiducialDetection = false; fiducialDetectionProgressCounter = 0;
                                SetToolStrip(Color.Fuchsia, string.Format("Fiducial detection: 1) TP1/mm X:{0:0.00} Y:{1:0.00}  not reached - STOP automatic", teachPoint1.X, teachPoint1.Y), true);
                                Logger.Error("Fiducial detection: 2, Grbl-Idle not reached");
                            }
                            break;
                        }
                        fiducialDetectionProgressCounter++;
                        fiducialDetection = true;				// activate shape detection
                        break;
                    }
                case 3:
                    {
                        fiducialDetection = false;
                        if (CbUseShapeRecognition.Checked)
                        {
                            if (!shapeFound)
                            {
                                fiducialDetectionFail++;
                                if (fiducialDetectionFail > fiducialDetectionFailMax)
                                {
                                    fiducialDetectionProgressCounter = 0;
                                    frameCounterMax = 10;
                                    SetToolStrip(Color.Fuchsia, "Fiducial detection: no fiducials found in image - STOP automatic", true);
                                    break;
                                }
                                SetToolStrip(Color.Yellow, string.Format("Fiducial detection: no fiducials found try {0}/{1}", fiducialDetectionFail, fiducialDetectionFailMax), true);
                                break;
                            }
                            realPos1 = TranslateFromPicCoordinate(shapeCenterInPx);
                            SetToolStrip(Color.Lime, string.Format("Fiducial detection: 1) shape/px X:{0:0.00} Y:{1:0.00} shape/mm X:{2:0.00} Y:{3:0.00} - correct offset of current coordinate system", shapeCenterInPx.X, shapeCenterInPx.Y, realPos1.X, realPos1.Y), true);
                            if (showLog) Logger.Trace("Shape X:{0} Y:{1}   real X:{2}  Y:{3}", shapeCenterInPx.X, shapeCenterInPx.Y, realPos1.X, realPos1.Y);
                        }
                        else
                            realPos1 = realPoints[0];

                        if (showLog) Logger.Trace("Fiducial detection: 1) real  X:{0:0.00} Y:{1:0.00}", realPos1.X, realPos1.Y);
                        TbRealPoints.Text = string.Format("Assinged position:\r\nX:{0:0.00} Y:{1:0.00} offset", realPos1.X, realPos1.Y);

                        if (CameraMount == CameraMounting.Fix)
                        {
                            OnRaiseXYEvent(new XYEventArgs(0, 1, new XyPoint(-realPos1.X + teachPoint1.X + Grbl.posWork.X, -realPos1.Y + teachPoint1.Y + Grbl.posWork.Y), "G10 L20 P0"));        // set new coordinates
                            if (!CbUseShapeRecognition.Checked) // adapt manual collected coordinates
                            {
                                realPointsOffset = realPoints[0] - teachPoint1;
                                realPoints[0] = teachPoint1;
                            }
                        }
                        else
                        {
                            if (CbUseShapeRecognition.Checked) // position was moved
                                OnRaiseXYEvent(new XYEventArgs(0, 1, new XyPoint(-realPos1.X + teachPoint1.X, -realPos1.Y + teachPoint1.Y), "G10 L20 P0"));        // set new coordinates
                            else
                            {   // position was not moved for more accuracy - apply offset 
                                OnRaiseXYEvent(new XYEventArgs(0, 1, new XyPoint(-realPos1.X + teachPoint1.X + Grbl.posWork.X, -realPos1.Y + teachPoint1.Y + Grbl.posWork.Y), "G10 L20 P0"));        // set new coordinates
                                realPointsOffset = realPoints[0] - teachPoint1;
                                realPoints[0] = teachPoint1;
                            }
                        }

                        shapeFound = false;
                        fiducialDetectionProgressCounter++;
                        break;
                    }
                case 4: // set next teachpoint
                    {
                        if (CbUseShapeRecognition.Checked)
                        {
                            if (VisuGCode.fiducialsCenter.Count < 2)
                            {
                                SetToolStrip(Color.Fuchsia, "Fiducial detection: Too less fiducials from GCode for next step - STOP automatic", true);
                                fiducialDetection = false;
                                fiducialDetectionProgressCounter = 7;
                                break;
                            }
                        }
                        else
                        {
                            if ((realPoints.Count < 2) || (VisuGCode.fiducialsCenter.Count < 2))
                            {
                                SetToolStrip(Color.Fuchsia, "Fiducial detection: Too less assigned points for next step - STOP automatic", true);
                                fiducialDetection = false;
                                fiducialDetectionProgressCounter = 7;
                                break;
                            }
                        }
                        refPointInPx = TranslateToPicCoordinate(VisuGCode.fiducialsCenter[1]);
                        if (CameraMount != CameraMounting.Fix)
                            refPointInPx = picCenter;

                        teachPoint2 = (XyPoint)VisuGCode.fiducialsCenter[1];
                        Grbl.PosMarker = new XyzPoint((XyPoint)VisuGCode.fiducialsCenter[1], 0);
                        SetToolStrip(Color.Lime, string.Format("Fiducial detection: 2) TP2/mm X:{0:0.00} Y:{1:0.00}  TP2/px X:{2:0.00} Y:{3:0.00}", teachPoint2.X, teachPoint2.Y, refPointInPx.X, refPointInPx.Y), true);
                        if (showLog) Logger.Trace("Fiducial detection: 2) picPx   X:{0:0.00} Y:{1:0.00}", refPointInPx.X, refPointInPx.Y);

                        VisuGCode.CreateMarkerPath();
                        fiducialDetectionFail = 0;
                        fiducialDetection = true;

                        if (CbUseShapeRecognition.Checked)
                        {
                            fiducialDetection = false;
                            shapeFound = false;
                            if (CameraMount == CameraMounting.Fix)
                            {
                                fiducialDetection = true;               // activate shape detection
                                fiducialDetectionProgressCounter += 2;  // jump over "wait for idle"
                            }
                            else
                            {
                                OnRaiseXYEvent(new XYEventArgs(0, 1, teachPoint2, "G90 G0"));   // move to fiducial position
                                fiducialDetectionProgressCounter++;
                            }
                        }
                        else
                            fiducialDetectionProgressCounter += 2;  // jump over "wait for idle"
                        break;
                    }
                case 5: // still moving?
                    {
                        if (Grbl.Status != GrblState.idle)
                        {
                            if (fiducialDetectionGrblNotIdleCounter++ > 10)
                            {
                                fiducialDetection = false; fiducialDetectionProgressCounter = 0;
                                Logger.Error("Fiducial detection: 5, Grbl-Idle not reached");
                            }
                            break;
                        }
                        fiducialDetectionProgressCounter++;
                        fiducialDetection = true;               // activate shape detection
                        break;
                    }
                case 6:
                    {
                        if (CbUseShapeRecognition.Checked)
                        {
                            if (!shapeFound)
                            {
                                fiducialDetectionFail++;
                                if (fiducialDetectionFail > fiducialDetectionFailMax)
                                {
                                    fiducialDetectionProgressCounter = 0;
                                    frameCounterMax = 10;
                                    SetToolStrip(Color.Fuchsia, "Fiducial detection: no fiducials found in image - STOP automatic", true);
                                    fiducialDetection = false;
                                    break;
                                }
                                SetToolStrip(Color.Yellow, string.Format("Fiducial detection: no fiducials found try {0}/{1}", fiducialDetectionFail, fiducialDetectionFailMax), true);
                                break;
                            }
                            realPos2 = TranslateFromPicCoordinate(shapeCenterInPx);
                            if (CameraMount != CameraMounting.Fix)
                                realPos2 += (XyPoint)Grbl.posWork;// teachPoint1;        // set new coordinates

                            if (showLog) Logger.Trace("ShapePx X:{0} Y:{1}   real X:{2:0.00}  Y:{3:0.00}", shapeCenterInPx.X, shapeCenterInPx.Y, realPos2.X, realPos2.Y);
                        }
                        else
                            realPos2 = realPoints[1];


                        double angle1 = teachPoint1.AngleTo(teachPoint2);       // coordinates from graphics in 2D-view
                        double dist1 = teachPoint1.DistanceTo(teachPoint2);
                        double angle2 = teachPoint1.AngleTo(realPos2);          // realPos1 was shifted to teachPoint1
                        double dist2 = teachPoint1.DistanceTo(realPos2);
                        double angleResult = angle1 - angle2;
                        double scale = dist2 / dist1;
                        if (!Properties.Settings.Default.cameraScaleOnRotate) { scale = 1; }

                        SetToolStrip(Color.Lime, string.Format("Fiducial detection: 2) shape/px X:{0} Y:{1} shape/mm X:{2:0.00} Y:{3:0.00} - correct angle:{4:0.00} scale:{5:0.00}", shapeCenterInPx.X, shapeCenterInPx.Y, realPos2.X, realPos2.Y, angleResult, scale), true);
                        if (showLog) Logger.Trace("Fiducial detection: 2) real  X:{0:0.00} Y:{1:0.00} rotate", realPos2.X, realPos2.Y);

                        if (realPos1.DistanceTo(realPos2) < 1)
                        {
                            TbRealPoints.Text += string.Format("\r\nX:{0:0.00} Y:{1:0.00} NO rotation", realPos2.X, realPos2.Y);
                            fiducialDetectionProgressCounter++;
                            fiducialDetection = false;
                            break;
                        }
                        else
                        {
                            TbRealPoints.Text += string.Format("\r\nX:{0:0.00} Y:{1:0.00} rotation", realPos2.X, realPos2.Y);

                            OnRaiseXYEvent(new XYEventArgs(angleResult, scale, teachPoint1, "a"));       // rotate arround TP1
                            if (!CbUseShapeRecognition.Checked)
                            {
                                realPoints[1] = realPoints[1] - realPointsOffset;
                                ListRealPoints();
                            }

                            fiducialDetectionProgressCounter++;
                            fiducialDetection = false;
                            break;
                        }
                    }
                case 7:
                    {
                        fiducialDetectionProgressCounter = 0;
                        frameCounterMax = 10;
                        ResetToolStrip();
                        break;
                    }
            }
        }

        // Process image
        private AForge.Point shapeCenterInPx, picCenter, refPointInPx;
        private bool shapeFound = false;
        private Bitmap original;
        private void ProcessShapeDetection(Bitmap bitmap)
        {	// http://www.aforgenet.com/articles/shape_checker/

            RectangleF cloneRect = new RectangleF(0, 0, bitmap.Width, bitmap.Height);
            original = bitmap.Clone(cloneRect, bitmap.PixelFormat);

            bool showFiltered = cBShapeShowFilter.Checked;

            // lock image
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite, bitmap.PixelFormat);

            //		HistogramEqualization histoFilter = new HistogramEqualization( );
            //		histoFilter.ApplyInPlace( bitmapData );

            // step 1 - turn background to black
            ColorFiltering colorFilter = new ColorFiltering
            {
                Red = new IntRange(Properties.Settings.Default.camFilterRed1, Properties.Settings.Default.camFilterRed2),
                Green = new IntRange(Properties.Settings.Default.camFilterGreen1, Properties.Settings.Default.camFilterGreen2),
                Blue = new IntRange(Properties.Settings.Default.camFilterBlue1, Properties.Settings.Default.camFilterBlue2),
                FillOutsideRange = Properties.Settings.Default.camFilterOutside
            };

            colorFilter.ApplyInPlace(bitmapData);

            double blobScaling = cameraScalingFix;
            if (CameraMount == CameraMounting.MoveXY)
            { blobScaling = cameraScalingXy / 2; }

            int shapeMin = (int)((float)Properties.Settings.Default.camShapeSizeMin * cameraZoom * blobScaling);
            int shapeMax = (int)((float)Properties.Settings.Default.camShapeSizeMax * cameraZoom * blobScaling);
            // step 2 - locating objects
            BlobCounter blobCounter = new BlobCounter
            {
                FilterBlobs = true,
                MinHeight = shapeMin,
                MinWidth = shapeMin,
                MaxHeight = shapeMax,
                MaxWidth = shapeMax
            };
            //       Logger.Trace("Blob min:{0} max:{1}  scale:{2}",shapeMin,shapeMax,blobScaling);

            blobCounter.ProcessImage(bitmapData);

            Blob[] blobs = blobCounter.GetObjectsInformation();

            bitmap.UnlockBits(bitmapData);

            // step 3 - check objects' type and highlight
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker
            {
                MinAcceptableDistortion = (float)Properties.Settings.Default.camShapeDist,
                RelativeDistortionLimit = (float)Properties.Settings.Default.camShapeDistMax
            };


            Graphics g;
            if (showFiltered)
                g = Graphics.FromImage(bitmap);// (original);  // bitmap
            else
                g = Graphics.FromImage(original);// (original);  // bitmap


            Pen yellowPen = new Pen(Color.Yellow, 5); // circles
            Pen redPen = new Pen(Color.Red, 10); // circles
            Pen greenPen = new Pen(Color.Green, 5);   // known triangle

            double lowestDistance = xmid;
            double distance;
            shapeFound = false;
            //    AForge.Point center;
            double shapeRadius = 1;

            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                //    System.Single myRadius;
                if (Properties.Settings.Default.camShapeCircle && shapeChecker.IsCircle(edgePoints, out AForge.Point center, out System.Single myRadius))
                {
                    //           Logger.Trace("Shape r:{0}  min:{1}  max:{2}   refPosPx {3:0.0} {4:0.0}   centerPosPx {5:0.0} {6:0.0}", myRadius, shapeMin, shapeMax, refPointInPx.X, refPointInPx.Y, center.X, center.Y);
                    if ((center.X < 1) || (center.Y < 1))
                        continue;
                    shapeFound = true;
                    distance = center.DistanceTo((AForge.Point)refPointInPx);
                    if (!fiducialDetection)
                    {
                        g.DrawEllipse(yellowPen,
                        (float)(center.X - myRadius), (float)(center.Y - myRadius),
                        (float)(myRadius * 2), (float)(myRadius * 2));
                    }

                    if ((lowestDistance > Math.Abs(distance)))// && (myRadius > shapeMin))
                    {
                        lowestDistance = Math.Abs(distance);
                        shapeCenterInPx = center;
                        shapeRadius = myRadius;
                        if (showLog) Logger.Trace("Shape r:{0}  min:{1}  max:{2}   refPosPx {3:0.0} {4:0.0}   centerPosPx {5:0.0} {6:0.0}", myRadius, shapeMin, shapeMax, refPointInPx.X, refPointInPx.Y, center.X, center.Y);
                    }
                }
                //   List<IntPoint> corners;
                if (Properties.Settings.Default.camShapeRect && shapeChecker.IsQuadrilateral(edgePoints, out List<IntPoint> corners))  //.IsConvexPolygon
                {
                    IntPoint centxy;        // minxy, maxxy,
                    if (!fiducialDetection)
                    { g.DrawPolygon(yellowPen, ToPointsArray(corners)); }

                    PointsCloud.GetBoundingRectangle(corners, out IntPoint minxy, out IntPoint maxxy);
                    centxy = (minxy + maxxy) / 2;
                    if ((centxy.X < 1) || (centxy.Y < 1))
                        continue;
                    shapeFound = true;
                    distance = picCenter.DistanceTo(centxy);// PointsCloud.GetCenterOfGravity(corners));
                    if (lowestDistance > Math.Abs(distance))
                    {
                        lowestDistance = Math.Abs(distance);
                        shapeCenterInPx = centxy;// PointsCloud.GetCenterOfGravity(corners);
                        shapeRadius = maxxy.DistanceTo(minxy) / 2;// 50;
                    }
                }

            }
            if (shapeFound)
            {
                g.DrawEllipse(redPen,
               (float)(shapeCenterInPx.X - shapeRadius * 1.2), (float)(shapeCenterInPx.Y - shapeRadius * 1.2),
               (float)(shapeRadius * 2.4), (float)(shapeRadius * 2.4));
                if (!fiducialDetection)
                    SetToolStrip(Color.Lime, string.Format("Diameter {0:0.0}px {1:0.0}mm   MinPx:{2:0.0}", 2 * shapeRadius, 2 * shapeRadius / cameraScalingFix, (int)((float)Properties.Settings.Default.camShapeSizeMin * cameraZoom * cameraScalingFix)));
            }

            yellowPen.Dispose();
            redPen.Dispose();
            greenPen.Dispose();
            g.Dispose();
            if (showFiltered)
                pictureBoxVideo.Image = bitmap;   //BackgroundImage
            else
                pictureBoxVideo.Image = original;
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

        private static void ShapeSetLoad(string txt)
        {
            string[] value = txt.Split('|');
            if (value.Length < 10)
                return;
            int i = 1;
            try
            {
                Properties.Settings.Default.camFilterRed1 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterRed2 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterGreen1 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterGreen2 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterBlue1 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterBlue2 = Convert.ToInt16(value[i++]);
                Properties.Settings.Default.camFilterOutside = (value[i++] == "True");		// true if (value[i++] == "True")
                Properties.Settings.Default.camShapeCircle = (value[i++] == "True");
                Properties.Settings.Default.camShapeRect = (value[i++] == "True");
                Properties.Settings.Default.camShapeSizeMin = Convert.ToDecimal(value[i++]);
                Properties.Settings.Default.camShapeSizeMax = Convert.ToDecimal(value[i++]);
                Properties.Settings.Default.camShapeDist = Convert.ToDecimal(value[i++]);
                Properties.Settings.Default.camShapeDistMax = Convert.ToDecimal(value[i++]);
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "ShapeSetLoad ");
            }
        }

        private void LoadExampleImages()
        {
            string fileExampleFrame = Datapath.Examples + "\\" + "exampleFrameFix.jpg";
            if (File.Exists(fileExampleFrame))
            {
                exampleFrameFix = (System.Drawing.Image)(new Bitmap(System.Drawing.Image.FromFile(fileExampleFrame), new Size(640, 480)));
                Logger.Info("Load example frame '{0}'", fileExampleFrame);
            }
            else
            {
                exampleFrameFix = new Bitmap(640, 480);
                using (Graphics gr = Graphics.FromImage(exampleFrameFix))
                {
                    gr.Clear(Color.LightGray);
                    ShowLabel(gr, new System.Drawing.Point(50, 200), string.Format("File not found: {0}", fileExampleFrame));
                }
            }
            fileExampleFrame = Datapath.Examples + "\\" + "exampleFrameXy.jpg";
            if (File.Exists(fileExampleFrame))
            {
                exampleFrameXy = (System.Drawing.Image)(new Bitmap(System.Drawing.Image.FromFile(fileExampleFrame), new Size(640, 480)));
                Logger.Info("Load example frame '{0}'", fileExampleFrame);
            }
            else
            {
                exampleFrameXy = new Bitmap(640, 480);
                using (Graphics gr = Graphics.FromImage(exampleFrameXy))
                {
                    gr.Clear(Color.LightGray);
                    ShowLabel(gr, new System.Drawing.Point(50, 200), string.Format("File not found: {0}", fileExampleFrame));
                }
            }
        }

        private void ListSettings()
        {
            Logger.Info("Cam fix Rot.:{0:0.000}  Scaling:{1:0.000}  Offset-X:{2:0.000}  Offset-Y:{3:0.000}", Properties.Settings.Default.cameraRotationFix, cameraScalingFix, Properties.Settings.Default.cameraZeroFixMachineX, Properties.Settings.Default.cameraZeroFixMachineY);
            Logger.Info("Cam  xy Rot.:{0:0.000}  Scaling:{1:0.000} ", Properties.Settings.Default.cameraRotationXy, cameraScalingXy);
            Logger.Info("Cam   z Rot.:{0:0.000}  Scl-Top:{1:0.000}  Scl-Bot:{2:0.000}", Properties.Settings.Default.cameraRotationXy, cameraScalingXyzTop, cameraScalingXyzBot);
            double blobScaling = cameraScalingFix;
            if (CameraMount == CameraMounting.MoveXY)
            { blobScaling = cameraScalingXy / 2; }
            Logger.Info("Shape Min:{0:0.000}  Max:{1:0.000}  CamZoom:{2}  BlobScale:{3}", Properties.Settings.Default.camShapeSizeMin, Properties.Settings.Default.camShapeSizeMax, cameraZoom, blobScaling);
            Logger.Info("Shape Distortion:{0}  DistortionMax:{1}", Properties.Settings.Default.camShapeDist, Properties.Settings.Default.camShapeDistMax);

            Logger.Info("Shape Red1:{0}  Red2:{1}  Green1:{2}  Green2:{3}  Blue1:{4}  Blue2:{5}  Outside:{6}", Properties.Settings.Default.camFilterRed1, Properties.Settings.Default.camFilterRed2,
                                                                                Properties.Settings.Default.camFilterGreen1, Properties.Settings.Default.camFilterGreen2,
                                                                                Properties.Settings.Default.camFilterBlue1, Properties.Settings.Default.camFilterBlue2, Properties.Settings.Default.camFilterOutside);

        }
    }
}
