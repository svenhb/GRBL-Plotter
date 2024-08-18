/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2024 Sven Hasemann contact: svenhb@web.de

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
/* MainFormPictureBox
 * Methods related to pictureBox1
 * pictureBox1_Click
 * pictureBox1_MouseMove
 * pictureBox1_Paint
 * pictureBox1_SizeChanged
 * 2019-01-28 3 digitis for coordinates
 * 2019-12-07 enable/disable show of ruler, info, pen-up
 * 2020-03-22 fix zoom-in -out behavior
 * 2020-08-20 linie 313, avoid collapse - findFigureMarkSelection(xmlMarkerType.Line, line, (foldLevel>0));
 * 2020-09-04 move initial view to lower edge - add maxposY
 * 2021-03-28 pictureBox1_MouseDown: only expand block (findFigureMarkSelection) if enabled
 * 2021-07-14 code clean up / code quality
 * 2021-09-06 also allow right mouse-btn to move view in PictureBox1_MouseMove
 * 2021-09-08 color-selector for BackgroundPath and Dimension
 * 2021-09-16 show pathPenDown OR pathObject (not both). Apply tileOffset during progress
 * 2021-09-30 line 243 take care of VisuGCode.largeDataAmount
 * 2021-12-27 line 253 check if any coloredPenPathAvailable
 * 2022-01-03 switch order of resetView = true line 736 and 755
 * 2022-01-21 snap on grid
 * 2022-04-04 line 550 _projector_form.Invalidate()
 * 2022-07-29 GraphicPropertiesSetup add try catch
 * 2022-09-13 line 114/137	if ((xRange == 0) || (yRange == 0)) picScaling = 1;
 * 2022-12-02 PictureBox1_MouseUp line 598 SetMarkerPos also if no CodeBlocks available
 * 2023-09-06 l:573 f:PictureBox1_MouseUp save and restore actual 2D-view pBoxTransform and last grid
			  l:178 f:PictureBox1_Paint VisuGCode.SetRulerDimension(drawingSize)
			  l:867 f:ZoomScroll don't reset view, on zoom=1
 * 2023-11-15 l:364 f:OnPaint_drawToolPath check for pathObjectPenColorOnlyNone	- show default color if all PenColors are "none"		  	
 * 2023-12-17 l:1038 f:CmsPicBoxReloadFile_Click add 2nd recent file load, if 1st is "lastProcessed.nc"
 * 2024-05-20 add "Apply last transform"
*/

using FastColoredTextBoxNS;
using GrblPlotter.MachineControl;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class MainForm : Form
    {
        #region pictureBox
        // onPaint drawing
        private readonly Pen penUp = new Pen(Color.Green, 0.1F);
        private readonly Pen penDown = new Pen(Color.Red, 0.4F);
        private readonly Pen penRotary = new Pen(Color.White, 0.4F);
        private readonly Pen penHeightMap = new Pen(Color.Yellow, 1F);
        private readonly Pen penSelection = new Pen(Color.Yellow, 1F);
        private readonly Pen penRuler = new Pen(Color.Blue, 0.1F);
        private readonly Pen penGrid1 = new Pen(Color.LightGray, 0.01F);
        private readonly Pen penGrid10 = new Pen(Color.LightSlateGray, 0.01F);
        private readonly Pen penGrid100 = new Pen(Color.Gray, 0.1F);
        private readonly Pen penTool = new Pen(Color.Black, 0.5F);
        private readonly Pen penMarker = new Pen(Color.DeepPink, 1F);
        private readonly Pen penLandMark = new Pen(Color.DarkGray, 1F);
        private readonly Pen penBackgroundPath = new Pen(Color.DarkGray, 1F);
        private readonly Pen penDimension = new Pen(Color.DarkGray, 1F);
        private readonly Pen penSimulation = new Pen(Color.Blue, 0.4F);
        private HatchBrush brushMachineLimit = new HatchBrush(HatchStyle.Horizontal, Color.Yellow);
        private readonly SolidBrush brushBackground = new SolidBrush(Color.White);
        private readonly SolidBrush brushBackgroundPath = new SolidBrush(Color.DarkGray);

        private DrawingProperties drawingSize = new DrawingProperties();

        private XyPoint picAbsPos = new XyPoint(0, 0);
        private Bitmap picBoxBackround;
        private bool showPicBoxBgImage = false;
        private bool showPathPenUp = true;
        private bool showPaths = false;
        private XyPoint posMoveStart = new XyPoint(0, 0);
        private XyPoint posMoveEnd = new XyPoint(0, 0);
        private XyPoint moveTranslation = new XyPoint(0, 0);
        private XyPoint moveTranslationOld = new XyPoint(0, 0);
        private bool posIsMoving = false;
        private bool mouseWasMoved = false;
        private Point mouseDownPos = new Point();
        private Point mouseUpPos = new Point();
        private Point mouseLastUpPos = new Point();
        private Point mouseMovePos = new Point();
        private readonly Font myFont7 = new Font("Lucida Console", 7);
        private readonly Font myFont8 = new Font("Lucida Console", 8);

        private readonly object lockObject = new object();
        private bool shiftedDisplay = false;

        private bool logSelection = false;  //true;

        private XmlMarkerType markerType = XmlMarkerType.Figure;
        private XmlMarkerType lastMarkerType = XmlMarkerType.Figure;

        private double picScaling = 1;
        private void CalculatePicScaling()
        {
            double minx = VisuGCode.drawingSize.minX;                  // extend dimensions
            double maxx = VisuGCode.drawingSize.maxX;
            double miny = VisuGCode.drawingSize.minY;
            double maxy = VisuGCode.drawingSize.maxY;
            double xRange = (maxx - minx);                                              // calculate new size
            double yRange = (maxy - miny);
            if (Properties.Settings.Default.machineLimitsFix)
            {
                double offset = (double)Properties.Settings.Default.machineLimitsRangeX / 40;       // view size
                xRange = (double)Properties.Settings.Default.machineLimitsRangeX + 2 * offset;
                yRange = (double)Properties.Settings.Default.machineLimitsRangeY + 2 * offset;
            }

            if ((xRange == 0) || (yRange == 0))
                picScaling = 1;
            else
                picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit
        }

        /******************************************************************
		***** On paint **********************
		*******************************************************************/
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            double minx = VisuGCode.drawingSize.minX;                  // extend dimensions
            double maxx = VisuGCode.drawingSize.maxX;
            double miny = VisuGCode.drawingSize.minY;
            double maxy = VisuGCode.drawingSize.maxY;
            double xRange = (maxx - minx);                                              // calculate new size
            double yRange = (maxy - miny);
            if (Properties.Settings.Default.machineLimitsFix)
            {
                double offset = (double)Properties.Settings.Default.machineLimitsRangeX / 40;       // view size
                minx = (double)Properties.Settings.Default.machineLimitsHomeX - Grbl.posWCO.X - offset;
                miny = (double)Properties.Settings.Default.machineLimitsHomeY - Grbl.posWCO.Y - offset;
                xRange = (double)Properties.Settings.Default.machineLimitsRangeX + 2 * offset;
                yRange = (double)Properties.Settings.Default.machineLimitsRangeY + 2 * offset;
            }


            if ((xRange == 0) || (yRange == 0))
                picScaling = 1;
            else
                picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit

            if ((picScaling > 0.001) && (picScaling < 10000))
            {
                double offsetX = 0;
                double offsetY = 0;
                try { offsetX = pBoxTransform.OffsetX; offsetY = pBoxTransform.OffsetY; } catch (Exception err) { Logger.Error(err, "PictureBox1_Paint - pBoxTransform1 nok"); }
                double relposX = (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).X) - offsetX) / Convert.ToDouble(pictureBox1.Width) / Convert.ToDouble(zoomFactor);
                double relposY = (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).Y) - offsetY) / Convert.ToDouble(pictureBox1.Height) / Convert.ToDouble(zoomFactor);
                double ratioVisu = xRange / yRange;
                double ratioPic = Convert.ToDouble(pictureBox1.Width) / pictureBox1.Height;

                double maxposY = yRange;
                if (ratioVisu > ratioPic)
                {
                    relposY = relposY * ratioVisu / ratioPic;
                    maxposY = xRange * pictureBox1.Height / pictureBox1.Width;
                }
                else
                { relposX = relposX * ratioPic / ratioVisu; }

                picAbsPos.X = relposX * xRange + minx;
                picAbsPos.Y = maxposY - relposY * yRange + miny;


                PointF viewDimMin = GetStandardizedPosition(new Point(0, 0));
                PointF viewDimMax = GetStandardizedPosition(new Point(pictureBox1.Width, pictureBox1.Height));
                drawingSize.SetX((float)Math.Floor(viewDimMin.X * xRange + minx), (float)Math.Round(viewDimMax.X * xRange + minx));
                drawingSize.SetY((float)Math.Floor(maxposY - viewDimMin.Y * yRange + miny), (float)Math.Round(maxposY - viewDimMax.Y * yRange + miny));
                drawingSize = VisuGCode.SetRulerDimension(drawingSize);

                if (posIsMoving)
                    posMoveEnd = picAbsPos;

                /* Label position */
                int offX = +5, offY = -10;
                if (pictureBox1.PointToClient(MousePosition).X > (pictureBox1.Width / 2)) { offX = -75; }
                if (pictureBox1.PointToClient(MousePosition).Y > (pictureBox1.Height / 2)) { offY = -30; }

                pBoxOrig = e.Graphics.Transform;		// get actual matrix for restorage later
                try { e.Graphics.Transform = pBoxTransform; } catch (Exception err) { Logger.Error(err, "PictureBox1_Paint - pBoxTransform2 nok"); }
                e.Graphics.ScaleTransform((float)picScaling, (float)-picScaling);           // apply scaling (flip Y)
                e.Graphics.TranslateTransform((float)-minx, (float)(-maxposY - miny));       // apply offset

                try
                {
                    if (showPaths)          // only show graphics path if something is loaded
                    {
                        lock (lockObject)
                        {
                            shiftedDisplay = false;
                            if (!showPicBoxBgImage)
                                OnPaint_drawToolPath(e.Graphics);   // draw real path if background image is not shown

                            e.Graphics.DrawPath(penTool, VisuGCode.pathTool);

                            if (shiftedDisplay)
                            {
                                e.Graphics.TranslateTransform(-(float)VisuGCode.ProcessedPath.offset2DView.X, -(float)VisuGCode.ProcessedPath.offset2DView.Y);       // apply offset
                                e.Graphics.DrawPath(penMarker, VisuGCode.pathMarker);
                                e.Graphics.TranslateTransform((float)VisuGCode.ProcessedPath.offset2DView.X, (float)VisuGCode.ProcessedPath.offset2DView.Y);       // remove offset
                            }
                            else
                                e.Graphics.DrawPath(penMarker, VisuGCode.pathMarker);
                        }
                        e.Graphics.Transform = pBoxOrig;	// restore original matrix

                        if (Properties.Settings.Default.gui2DInfoShow)
                        {
                            Point stringpos = new Point(pictureBox1.PointToClient(MousePosition).X + offX, pictureBox1.PointToClient(MousePosition).Y + offY);
                            //    Point stringposRot = new Point(pictureBox1.PointToClient(MousePosition).X - 20, pictureBox1.PointToClient(MousePosition).Y + 20);
                            //    Point stringposMov = new Point(pictureBox1.PointToClient(MousePosition).X + 20, pictureBox1.PointToClient(MousePosition).Y - 15);

                            if (Properties.Settings.Default.machineLimitsShow)  // 2023-08-31 issue #355	-	moved into if gui2DInfoShow
                            {
                                SolidBrush labelBrush = brushBackground;
                                if (transformType != SelectionHandle.Handle.None)
                                    labelBrush = new SolidBrush(Color.LightYellow);

                                e.Graphics.FillRectangle(labelBrush, new Rectangle(stringpos.X, stringpos.Y - 2, 75, 34));
                                e.Graphics.FillRectangle(brushBackground, new Rectangle(18, 3, 140, 60));
                                if (VisuGCode.selectedFigureInfo.Length > 0)
                                    e.Graphics.FillRectangle(brushBackground, new Rectangle(148, 3, 160, 60));
                            }
                            string unit = (Properties.Settings.Default.importUnitmm) ? "mm" : "Inch";

                            /* label at mouse pos */
                            if (transformType == SelectionHandle.Handle.None)
                                e.Graphics.DrawString(String.Format("Work-Pos:\r\nX:{0,7:0.000}\r\nY:{1,7:0.000}", picAbsPos.X, picAbsPos.Y), myFont8, Brushes.Black, stringpos);
                            else if (transformType == SelectionHandle.Handle.Rotate)
                                e.Graphics.DrawString(String.Format("Rotate by:\r\n{0:0.0}°", SelectionHandle.ActualTransform.angleDeg), myFont8, Brushes.Black, stringpos);
                            else if (transformType == SelectionHandle.Handle.Move)
                                e.Graphics.DrawString(String.Format("Move by:\r\nX:{0:0.0}\r\nY:{1:0.0}", (posMoveEnd.X - posMoveStart.X), (posMoveEnd.Y - posMoveStart.Y)), myFont8, Brushes.Black, stringpos);
                            else
                                e.Graphics.DrawString(String.Format("Zoom by:\r\nX:{0:0.0}%\r\nY:{1:0.0}%", (SelectionHandle.ActualTransform.scalingX * 100), (SelectionHandle.ActualTransform.scalingY * 100)), myFont8, Brushes.Black, stringpos);


                            /* label at upper left edge */
                            if (simuEnabled)
                                e.Graphics.DrawString(String.Format("Zooming   : {0,2:0.00}%\r\n" +
                                                                    "Ruler Unit: {1}\r\n" +
                                                                    "Marker-Pos:\r\n" +
                                                                    " X:{2,7:0.000}\r\n" +
                                                                    " Y:{3,7:0.000}\r\n" +
                                                                    " Z:{4,7:0.000}\r\n" +
                                                                    " a:{5,7:0.000}°", 100 * zoomFactor, unit,
                                Grbl.PosMarker.X, Grbl.PosMarker.Y, VisuGCode.Simulation.GetZ(), 180 * Grbl.PosMarkerAngle / Math.PI),
                                myFont7, Brushes.Black, new Point(20, 5));
                            else
                                e.Graphics.DrawString(String.Format("Zooming   : {0,2:0.00}%\r\n" +
                                                                    "Ruler Unit: {1}\r\n" +
                                                                    "Marker-Pos:\r\n" +
                                                                    " X:{2,7:0.000}\r\n" +
                                                                    " Y:{3,7:0.000}\r\n" +
                                                                    " Z:{4,7:0.000}", 100 * zoomFactor, unit,
                                Grbl.PosMarker.X, Grbl.PosMarker.Y, Grbl.PosMarker.Z), myFont7, Brushes.Black, new Point(20, 5));

                            if (VisuGCode.selectedFigureInfo.Length > 0)
                                e.Graphics.DrawString(VisuGCode.selectedFigureInfo, myFont7, Brushes.Black, new Point(150, 5));
                        }

                    }
                }
                catch (Exception err) { Logger.Error(err, "PictureBox1_Paint - showPath nok"); }
            }
        }

        private PointF GetStandardizedPosition(Point mousePos)
        {
            PointF tmp = new PointF((mousePos.X - pBoxTransform.OffsetX) / pictureBox1.Width / zoomFactor,
                                    (mousePos.Y - pBoxTransform.OffsetY) / pictureBox1.Height / zoomFactor);
            return tmp;
        }
        private void OnPaint_scaling(Graphics e)
        {
            double minx = VisuGCode.drawingSize.minX;                  // extend dimensions
            double maxx = VisuGCode.drawingSize.maxX;
            double miny = VisuGCode.drawingSize.minY;
            double maxy = VisuGCode.drawingSize.maxY;
            double xRange = (maxx - minx);                             // calculate new size
            double yRange = (maxy - miny);
            if ((xRange > 0) && (yRange > 0))
            {
                double _picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit
                e.ScaleTransform((float)_picScaling, (float)-_picScaling);        // apply scaling (flip Y)
                e.TranslateTransform((float)-minx, (float)(-yRange - miny));    // apply offset
            }
        }

        private void OnPaint_drawToolPath(Graphics e)
        {
            try
            {
                float factorWidth = 1;
                if (!Properties.Settings.Default.importUnitmm) factorWidth = 0.0393701f;
                if (Properties.Settings.Default.gui2DKeepPenWidth) factorWidth /= zoomFactor;
                penHeightMap.Width = (float)Properties.Settings.Default.gui2DWidthHeightMap * factorWidth;
                penSelection.Width = (float)Properties.Settings.Default.gui2DWidthHeightMap * factorWidth;
                penRuler.Width = (float)Properties.Settings.Default.gui2DWidthRuler * factorWidth;
                penUp.Width = (float)Properties.Settings.Default.gui2DWidthPenUp * factorWidth;
                penDown.Width = (float)Properties.Settings.Default.gui2DWidthPenDown * factorWidth;
                penRotary.Width = (float)Properties.Settings.Default.gui2DWidthRotaryInfo * factorWidth;
                penTool.Width = (float)Properties.Settings.Default.gui2DWidthTool * factorWidth;
                penMarker.Width = (float)Properties.Settings.Default.gui2DWidthMarker * factorWidth;
                penLandMark.Width = 2 * (float)Properties.Settings.Default.gui2DWidthPenDown * factorWidth;
                penDimension.Width = 2 * (float)Properties.Settings.Default.gui2DWidthPenDown * factorWidth;
                penSimulation.Width = (float)Properties.Settings.Default.gui2DWidthSimulation * factorWidth;


                if (Properties.Settings.Default.machineLimitsShow)
                    e.FillPath(brushMachineLimit, VisuGCode.pathMachineLimit);
                if (Properties.Settings.Default.gui2DToolTableShow)
                    e.DrawPath(penTool, VisuGCode.pathToolTable);
                if (Properties.Settings.Default.guiBackgroundShow && !VisuGCode.ShiftTilePaths)
                {
                    e.DrawPath(penBackgroundPath, VisuGCode.pathBackground);

                    e.DrawPath(penBackgroundPath, Graphic.pathBackground);
                    e.FillPath(brushBackgroundPath, Graphic.pathBackground);
                }
                if (Properties.Settings.Default.guiDimensionShow)
                    e.DrawPath(penDimension, VisuGCode.pathDimension);

                e.DrawPath(penHeightMap, VisuGCode.pathHeightMap);

                if (Properties.Settings.Default.gui2DRulerShow)
                {
                    if ((picScaling * zoomFactor) > 10)
                        e.DrawPath(penGrid1, VisuGCode.pathGrid1);          // grid   1mm
                    if ((picScaling * zoomFactor) > 2)
                        e.DrawPath(penGrid10, VisuGCode.pathGrid10);        // grid  10mm
                    if ((picScaling * zoomFactor) > 0.1)
                        e.DrawPath(penGrid100, VisuGCode.pathGrid100);      // grid 100mm
                    if ((picScaling * zoomFactor) > 0.01)
                        e.DrawPath(penGrid100, VisuGCode.pathGrid1000);     // grid 1000mm
                    e.DrawPath(penGrid100, VisuGCode.pathGrid10000);        // grid 10000mm
                    e.DrawPath(penRuler, VisuGCode.pathRuler);
                }

                bool coloredPenPathAvailable = false;
                for (int k = 0; k < VisuGCode.pathObject.Count; k++)        //added 2021-12-24
                {
                    if (VisuGCode.pathObject[k].path.PointCount > 0)
                    {
                        coloredPenPathAvailable = true;
                        break;
                    }
                }
                //       Logger.Trace("Paint color pen down {0} largeAmount:{1}  count:{2}  availale:{3}", Properties.Settings.Default.gui2DColorPenDownModeEnable ,VisuGCode.largeDataAmount,VisuGCode.pathObject.Count , coloredPenPathAvailable);
                if (Properties.Settings.Default.gui2DColorPenDownModeEnable && (VisuGCode.pathObject.Count > 0) && coloredPenPathAvailable && !VisuGCode.pathObjectPenColorOnlyNone)// && (VisuGCode.pathObject[0].path.PointCount > 0))    // Show PenDown path in colors from imported graphics
                {
                    //if (VisuGCode.pathObject.Count > 0)
                    {
                        if (VisuGCode.ShiftTilePaths)
                        {
                            // GCodeSimulate.cs VisuGCode.ProcessedPath.offset2DView updated via 
                            // MainFormStreaming.cs VisuGCode.ProcessedPath.ProcessedPathLine(e.CodeLineConfirmed);	
                            // VisuGCode.ProcessedPath.offset2DView depends on importGraphicClipShowOrigPositionShiftTileProcessed
                            shiftedDisplay = true;
                        }
                        if (shiftedDisplay)
                            e.TranslateTransform(-(float)VisuGCode.ProcessedPath.offset2DView.X, -(float)VisuGCode.ProcessedPath.offset2DView.Y);       // apply offset

                        try
                        {
                            //        Logger.Trace("Paint color pen down");
                            foreach (VisuGCode.PathData tmpPath in VisuGCode.pathObject)
                            {
                                //            Logger.Trace("Color {0}",tmpPath.color);
                                e.DrawPath(tmpPath.pen, tmpPath.path);
                                penSelection.Width = tmpPath.pen.Width;
                            }
                        }
                        catch (Exception err)
                        {
                            Logger.Error(err, "OnPaint_drawToolPath VisuGCode.pathObject count {0}", VisuGCode.pathObject.Count);
                        }

                        if (shiftedDisplay)
                            e.TranslateTransform((float)VisuGCode.ProcessedPath.offset2DView.X, (float)VisuGCode.ProcessedPath.offset2DView.Y);       // remove offset
                    }
                }
                else
                    e.DrawPath(penDown, VisuGCode.pathPenDown);

                if (shiftedDisplay)
                    e.TranslateTransform(-(float)VisuGCode.ProcessedPath.offset2DView.X, -(float)VisuGCode.ProcessedPath.offset2DView.Y);       // apply offset

                if (Properties.Settings.Default.ctrl4thUse)
                    e.DrawPath(penRotary, VisuGCode.pathRotaryInfo);

                if (posIsMoving)
                { e.DrawPath(penLandMark, VisuGCode.pathMarkSelection); }
                else
                    e.DrawPath(penSelection, VisuGCode.pathMarkSelection);

                if (SelectionHandle.IsActive) SelectionHandle.DrawPath(e, picScaling * zoomFactor);		// adapt handle size if scaling/zooming changed

                if (!(showPathPenUp ^ toolStripViewPenUp.Checked))  //Properties.Settings.Default.gui2DPenUpShow))
                    e.DrawPath(penUp, VisuGCode.pathPenUp);

                if (shiftedDisplay)
                    e.TranslateTransform((float)VisuGCode.ProcessedPath.offset2DView.X, (float)VisuGCode.ProcessedPath.offset2DView.Y);       // remove offset

                e.DrawPath(penSimulation, VisuGCode.Simulation.pathSimulation);
            }
            catch (Exception Ex)
            {
                Logger.Error(Ex, "OnPaint_drawToolPath ");
                throw;
            }
        }

        // Generante a background-image for pictureBox to avoid frequent drawing of pen-up/down paths
        private void OnPaint_setBackground()
        {
            if (Properties.Settings.Default.guiBackgroundImageEnable)
            {
                showPicBoxBgImage = true;
                pBoxTransform.Reset(); zoomFactor = 1; //zoomOffsetX = 0; zoomOffsetY = 0;
                pictureBox1.BackgroundImageLayout = ImageLayout.None;
                picBoxBackround = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                Graphics graphics = Graphics.FromImage(picBoxBackround);
                graphics.DrawString("Streaming, zooming disabled", myFont8, Brushes.Black, 1, 1);
                OnPaint_scaling(graphics);
                OnPaint_drawToolPath(graphics);     // draw path
                pictureBox1.BackgroundImage = new Bitmap(picBoxBackround);//Properties.Resources.modell;
            }
        }

        private void PictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (showPicBoxBgImage)
                OnPaint_setBackground();
            pictureBox1.Invalidate();
        }

        private SelectionHandle.Handle transformType = SelectionHandle.Handle.None;
        private GraphicsPath selectionPathOrig = new GraphicsPath();
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMovePos = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                posIsMoving = true;     // mouse-move

                if (SelectionHandle.IsActive)
                {
                    MoveSelectedFigure(posMoveStart, posMoveEnd);  // Move selection-handles and frame during mouse down 
                    if (logSelection) Logger.Trace("►►► PictureBox1_MouseMove MoveSelectedFigure start:{0:0.00};{1:0.00} end:{2:0.00};{3:0.00} Handle:{4}  Marker:{5}", posMoveStart.X, posMoveStart.Y, posMoveEnd.X, posMoveEnd.Y, transformType, markerType);
                }
                if (transformType == SelectionHandle.Handle.None)
                {   // else move view of whole graphic
                    if (PointDistance(mouseMovePos, mouseDownPos) > 20) mouseWasMoved = true;
                    moveTranslation = new XyPoint(e.X, e.Y) - moveTranslationOld;  // calc delta move
                    pBoxTransform.Translate((float)moveTranslation.X / zoomFactor, (float)moveTranslation.Y / zoomFactor);	// change view
                    moveTranslationOld = new XyPoint(e.X, e.Y);
                    if (logSelection) Logger.Trace("►►► PictureBox1_MouseMove move view start:{0:0.00};{1:0.00} end:{2:0.00};{3:0.00} ", posMoveStart.X, posMoveStart.Y, posMoveEnd.X, posMoveEnd.Y);
                }
                XyPoint diff = posMoveEnd - posMoveStart;
                if ((diff.X != 0) || (diff.Y != 0))
                    previousClick = 0;  // no doubleclick

                if (transformType == SelectionHandle.Handle.Rotate)
                    StatusStripSet(2, string.Format("{0} {1:0.00}°  {2}", Localization.GetString("statusStripeSelectionRotate"), SelectionHandle.ActualTransform.angleDeg, Localization.GetString("statusStripeSelectionRotateShift")), Color.Yellow);
                else if (transformType == SelectionHandle.Handle.SizeXY)
                    StatusStripSet(2, string.Format("{0} X:{1:0.00}%  Y:{2:0.00}%  {3}", Localization.GetString("statusStripeSelectionResize"), (SelectionHandle.ActualTransform.scalingX * 100), (SelectionHandle.ActualTransform.scalingY * 100), Localization.GetString("statusStripeSelectionResizeCtrl")), Color.Yellow);
                else if (transformType == SelectionHandle.Handle.SizeX)
                    StatusStripSet(2, string.Format("{0} X:{1:0.00}%  {2}", Localization.GetString("statusStripeSelectionResize"), (SelectionHandle.ActualTransform.scalingX * 100), Localization.GetString("statusStripeSelectionMoveShift")), Color.Yellow);
                else if (transformType == SelectionHandle.Handle.SizeY)
                    StatusStripSet(2, string.Format("{0} Y:{1:0.00}%  {2}", Localization.GetString("statusStripeSelectionResize"), (SelectionHandle.ActualTransform.scalingY * 100), Localization.GetString("statusStripeSelectionMoveShift")), Color.Yellow);
                else
                    StatusStripSet(2, string.Format("{0} X:{1:0.00}  Y:{2:0.00}   {3}", Localization.GetString("statusStripeSelectionMoved"), (posMoveEnd.X - posMoveStart.X), (posMoveEnd.Y - posMoveStart.Y), Localization.GetString("statusStripeSelectionMoveShift")), Color.Yellow);
            }
            else
            {
                if (SelectionHandle.IsActive) 	// if selection frame and handles are visible, change cursor, when over handle
                {
                    transformType = SelectionHandle.IsHandlePosition(picAbsPos);
                    if (transformType == SelectionHandle.Handle.Move) { Cursor = Cursors.SizeAll; }
                    else if (transformType == SelectionHandle.Handle.SizeX) { Cursor = Cursors.SizeWE; }
                    else if (transformType == SelectionHandle.Handle.SizeY) { Cursor = Cursors.SizeNS; }
                    else if (transformType == SelectionHandle.Handle.SizeXY) { Cursor = Cursors.SizeNESW; }
                    else if (transformType == SelectionHandle.Handle.Rotate) { Cursor = Cursors.SizeNESW; }
                }
            }
            pictureBox1.Invalidate();
        }

        /********************************************************************************
		/* Move selection-handles and frame during mouse down (path in SelectionHandle) *
		*********************************************************************************/
        private void MoveSelectedFigure(XyPoint posStart, XyPoint posEnd)   // move selected area
        {
            XyPoint diff = posEnd - posStart;
            if (VisuGCode.pathMarkSelection.PointCount > 1)     // move selected path
            {
                if (transformType != SelectionHandle.Handle.None)
                {
                    /* preview of change */
                    bool keepZoomRatio = (Panel.ModifierKeys == Keys.Control);
                    bool snapOnGrid = (Panel.ModifierKeys == Keys.Shift);
                    Matrix selectionMatrix = new Matrix();
                    PointF scaleFactor = SelectionHandle.GetScaleFactor(diff, keepZoomRatio, snapOnGrid);
                    VisuGCode.pathMarkSelection = (GraphicsPath)selectionPathOrig.Clone();

                    if (transformType == SelectionHandle.Handle.Move)
                    {
                        diff = SelectionHandle.Translate(diff, snapOnGrid);

                        if ((markerType == XmlMarkerType.Node) || (SelectionHandle.ActualTransform.markerType == XmlMarkerType.Node))
                        {
                            //	VisuGCode.pathMarkSelection.Reset();
                            SelectionHandle.UpdateSelectionPath(new PointF((float)posEnd.X, (float)posEnd.Y));
                        }
                        else
                        {
                            selectionMatrix.Translate((float)diff.X, (float)diff.Y);
                            VisuGCode.pathMarkSelection.Transform(selectionMatrix);
                        }
                    }
                    else
                    {
                        selectionMatrix.Translate((float)SelectionHandle.ActualTransform.transformPoint.X, (float)SelectionHandle.ActualTransform.transformPoint.Y);	// left-top point of selection-bounds
                    }
                    if (transformType == SelectionHandle.Handle.SizeX)
                        selectionMatrix.Scale(scaleFactor.X, 1);
                    else if (transformType == SelectionHandle.Handle.SizeY)
                        selectionMatrix.Scale(1, scaleFactor.Y);
                    else if (transformType == SelectionHandle.Handle.SizeXY)
                        selectionMatrix.Scale(scaleFactor.X, scaleFactor.Y);

                    else if (transformType == SelectionHandle.Handle.Rotate)
                    {
                        selectionMatrix.Reset();
                        selectionMatrix.Translate((float)SelectionHandle.ActualTransform.center.X, (float)SelectionHandle.ActualTransform.center.Y);
                        selectionMatrix.Rotate(SelectionHandle.GetAngleDeg(posStart, posEnd, snapOnGrid));       // angle in degre
                        selectionMatrix.Translate(-(float)SelectionHandle.ActualTransform.center.X, -(float)SelectionHandle.ActualTransform.center.Y);
                        VisuGCode.pathMarkSelection.Transform(selectionMatrix);
                    }
                    if ((transformType != SelectionHandle.Handle.Move) && (transformType != SelectionHandle.Handle.Rotate))
                    {
                        SelectionHandle.Scale(diff, transformType, keepZoomRatio, snapOnGrid);
                        selectionMatrix.Translate(-(float)SelectionHandle.ActualTransform.transformPoint.X, -(float)SelectionHandle.ActualTransform.transformPoint.Y);	// left-top point of selection-bounds
                        VisuGCode.pathMarkSelection.Transform(selectionMatrix);
                    }
                    selectionMatrix.Dispose();
                }
            }
        }

        private int PointDistance(Point a, Point b)
        {
            int x = a.X - b.X;
            int y = a.Y - b.Y;
            return (int)Math.Sqrt(x * x + y * y);
        }
        private void MoveView(double x, double y)       // MainForm_KeyDown
        {
            x = x * 10 * zoomFactor;
            y = y * 10 * zoomFactor;
            posIsMoving = false;        // move view
            moveTranslation = new XyPoint(x, y);  // calc delta move
            pBoxTransform.Translate((float)moveTranslation.X / zoomFactor, (float)moveTranslation.Y / zoomFactor);
            moveTranslationOld = new XyPoint(x, y);
        }
        //    private static MouseButtons _lastButtonUp = MouseButtons.None;
        private static int previousClick = SystemInformation.DoubleClickTime;

        /******************************************************************
		***** MouseUp, proceed move, scale, rotation **********************
		*******************************************************************/
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (logDetailed || logSelection) Logger.Trace("▲▲▲ pictureBox1_MouseUp   e.x:{0} y:{1}  absPos-x:{2:0.00} y:{3:0.00}   handleActive:{4}  handle:{5}    marker:{6}    lastmarker:{7}", e.X, e.Y, picAbsPos.X, picAbsPos.Y, SelectionHandle.IsActive, transformType, markerType, lastMarkerType);

            Matrix tmp = pBoxTransform.Clone();
            mouseUpPos = e.Location;
            bool selectionWasChanged = false;

            if (SelectionHandle.IsActive && (posMoveStart.DistanceTo(posMoveEnd) != 0))	// store applied selection move
            {
                double dx = (posMoveEnd.X - posMoveStart.X);
                double dy = (posMoveEnd.Y - posMoveStart.Y);
                if (Panel.ModifierKeys == Keys.Shift)       // snap on grid
                { dx = SelectionHandle.correctedDifference.X; dy = SelectionHandle.correctedDifference.Y; }

                SelectionHandle.SaveTransformData(transformType, dx, dy);
                ApplyTransform(SelectionHandle.ActualTransform);
                selectionWasChanged = true;
            }
            transformType = SelectionHandle.Handle.None;
            posIsMoving = false;    // mouse-up
            allowZoom = true;

            int now = System.Environment.TickCount;
            int diff = now - previousClick;

            /* select Figure if MouseDown and MouseUp position are close together */
            if (mouseDownLeftButton && ((PointDistance(mouseDownPos, mouseUpPos) < 10) || selectionWasChanged))
            {
                SetFigureSelectionOnClick(selectionWasChanged);
                if (!VisuGCode.CodeBlocksAvailable())
                {
                    fCTBCodeClickedLineNow = VisuGCode.SetPosMarkerNearBy(picAbsPos, false).lineNumber;
                    VisuGCode.SetPosMarkerLine(fCTBCodeClickedLineNow, false);
                    FctbSetBookmark();
                }
            }
            applyLastTransformToolStripMenuItem.Visible = SelectionHandle.IsActive;

            previousClick = now;
            pBoxTransform = tmp.Clone();    // restore previous view

            if ((diff <= SystemInformation.DoubleClickTime) && (diff > 100))
            { PictureBox1_DoubleClick(sender, e); }

            _projector_form?.Invalidate();
        }

        private void ApplyTransform(SelectionHandle.TransformData tmpTransform)
        {
            transformType = tmpTransform.action;
            if (transformType == SelectionHandle.Handle.None)
                return;
            TransformStart(transformType.ToString());
            Logger.Trace("●●● ApplyTransform  Handle:{0}   Marker:{1}", transformType, markerType);

            /* apply changes */
            if (transformType == SelectionHandle.Handle.Move)
            {
                if ((markerType == XmlMarkerType.Node) || (SelectionHandle.ActualTransform.markerType == XmlMarkerType.Node))
                {
                    Logger.Info("PictureBox1_MouseUp  line:{0}  code:{1}", fCTBCode.Selection.ToLine, fCTBCode.SelectedText);
                    string newLine = ModifyCode.ApplyXYOffsetSimple(fCTBCode.SelectedText, tmpTransform.dX, tmpTransform.dY);     // replace coordinate in text
                    Logger.Info("Insert  line:{0}  code:{1}", fCTBCode.Selection.ToLine, newLine);
                    fCTBCode.SelectedText = newLine;        // doesn't work, if folded
                    Logger.Info("fCTBCode.SelectedText  '{0}'  ", fCTBCode.SelectedText);
                }
                else
                    SetFctbCodeText(VisuGCode.TransformGCodeOffset(-tmpTransform.dX, -tmpTransform.dY, VisuGCode.Translate.None));

            }
            else if (transformType == SelectionHandle.Handle.Rotate)
                /* use center of actual selection */
                SetFctbCodeText(VisuGCode.TransformGCodeRotate(tmpTransform.angleDeg, 1, (XyPoint)SelectionHandle.ActualTransform.center, false));
            else
            {
                /* use point of actual selection */
                SetFctbCodeText(VisuGCode.TransformGCodeScale(tmpTransform.scalingX * 100, tmpTransform.scalingY * 100, (XyPoint)SelectionHandle.ActualTransform.transformPoint));
                if (VisuGCode.ContainsG2G3Command() && (tmpTransform.scalingX != tmpTransform.scalingY))
                {
                    StatusStripSet(2, "Code contains G2/G3 commands which can only be scaled with same x and y factor", Color.Yellow);
                }
            }

            fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
            TransformEnd();

            VisuGCode.SetPosMarkerLine(clickedLineNr, false);       // restore merker pos.
            fCTBCodeClickedLineLast = 0;
            SelectionHandle.ClearSelected();

            FoldBlocksByLevel(markerType, clickedLineNr);
            VisuGCode.MarkSelectedFigure(-1);
            Cursor = Cursors.Default;
            fCTBCode.Focus();
            VisuGCode.CreateRuler(VisuGCode.pathRuler, drawingSize, true);  // restore previous grid
        }

        private void ApplyLastTransformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Trace("applyLastTransformToolStripMenuItem_Click");
            if (SelectionHandle.IsActive)
                ApplyTransform(SelectionHandle.LastTransform);

            _projector_form?.Invalidate();
        }

        // find closest coordinate in GCode and mark
        private readonly bool expandGCode = true;
        private bool mouseDownLeftButton = false;
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownPos = e.Location;
            if (logDetailed || logSelection) Logger.Trace("▼▼▼ pictureBox1_MouseDown e.x:{0} y:{1}  absPos-x:{2:0.00} y:{3:0.00}   handleActive:{4}   marker:{5}   lasmarker:{6}", e.X, e.Y, picAbsPos.X, picAbsPos.Y, SelectionHandle.IsActive, markerType, lastMarkerType);
            moveTranslationOld = new XyPoint(e.X, e.Y);
            allowZoom = false;
            mouseWasMoved = false;
            mouseDownLeftButton = false;
            if (e.Button == MouseButtons.Left)
            {
                posMoveStart = picAbsPos;
                posMoveEnd = posMoveStart;

                mouseDownLeftButton = true;

                if (SelectionHandle.IsActive)
                {
                    if (transformType == SelectionHandle.Handle.Move) { Cursor = Cursors.SizeAll; }
                    else if (transformType == SelectionHandle.Handle.SizeX) { Cursor = Cursors.SizeWE; }
                    else if (transformType == SelectionHandle.Handle.SizeY) { Cursor = Cursors.SizeNS; }
                    else if (transformType == SelectionHandle.Handle.SizeXY) { Cursor = Cursors.SizeNESW; }
                    else if (transformType == SelectionHandle.Handle.Rotate) { Cursor = Cursors.SizeNESW; }
                    else
                        Cursor = Cursors.Default;
                }
                //        clickDownCount++;
                pictureBox1.Invalidate();
            }
        }

        /******************************************************************
		***** MouseUp, select node, figure or group  **********************
		*******************************************************************/
        private int clickedLineNr = 0;
        private DistanceByLine markerProperties;
        private void SetFigureSelectionOnClick(bool keepMarkerType)
        {
            if (logSelection) Logger.Trace("SetFigureSelectionOnClick   keepMarkerType:{0}  marker:{1}  lastmarker:{2}", keepMarkerType, markerType, lastMarkerType);
            if (fCTBCode.LinesCount > 2)
            {
                if (manualEdit) { NewCodeEnd(); }

                if (VisuGCode.CodeBlocksAvailable() && !isStreaming)
                {

                    /* 1. get selection whish: group, figure, node */
                    bool modKeyAlt = (Panel.ModifierKeys == Keys.Alt);          // Keys.Alt find line with coord nearby, mark / unmark Figure
                    bool modKeyCtrl = (Panel.ModifierKeys == Keys.Control);     // Keys.Control find line with coord nearby, mark / unmark Group
                    bool modKeyShift = (Panel.ModifierKeys == Keys.Shift);      // Keys.Shift find line with coord nearby, mark / unmark Tile

                    bool modKeyCtrlShift = (Panel.ModifierKeys.HasFlag(Keys.Control) && Panel.ModifierKeys.HasFlag(Keys.Shift));


                    //    if ((transformType == SelectionHandle.Handle.None) || keepMarkerType)
                    if (modKeyShift && (markerType == XmlMarkerType.Figure) && (PointDistance(mouseLastUpPos, mouseUpPos) > 10))
                    {
                        if (logSelection) Logger.Trace("SetFigureSelectionOnClick  add selection");
                        /*
                         * Not possible because whole selection must be in one block start-line to end-line
                         * */
                    }
                    else
                    {
                        /* Click on same object / same position: toggle selected objects (if possible):
                         * 1st select Figure, 2nd select Group, 3rd select Tile, finally select nothing
                         */
                        if (expandGCode)    //Properties.Settings.Default.FCTBBlockExpandOnSelect)
                        { foldLevel = foldLevelSelected; }

                        if (!keepMarkerType)
                        {
                            if (logSelection) Logger.Trace("set figure old:{0}  grp:{1}  tiles:{2}  collections:{3}", markerType, XmlMarker.GetGroupCount(), XmlMarker.GetTileCount(), XmlMarker.GetCollectionCount());
                            if (PointDistance(mouseLastUpPos, mouseUpPos) > 10)   //(clickedLineNr != markerProperties.lineNumber)
                            { markerType = XmlMarkerType.None; }
                            mouseLastUpPos = mouseUpPos;

                            if (markerType == XmlMarkerType.None)
                            {
                                markerType = lastMarkerType;    // XmlMarkerType.Figure;
                                                                //    markerType =  XmlMarkerType.Figure;
                                if (modKeyAlt) { SelectionHandle.ActualTransform.markerType = markerType = XmlMarkerType.Node; }
                                else if (modKeyCtrlShift) { markerType = XmlMarkerType.Collection; }
                                else if (modKeyCtrl) { markerType = XmlMarkerType.Group; }
                                else if (modKeyShift) { markerType = XmlMarkerType.Tile; }
                                if (logSelection) Logger.Trace("SetFigureSelectionOnClick   change markerType none to {0}   last:{1}", markerType, lastMarkerType);
                            }
                            else if (markerType == XmlMarkerType.Figure)
                            {
                                keepMarkerType = true; SelectionHandle.ClearSelected();
                                if (XmlMarker.GetGroupCount() > 0) markerType = XmlMarkerType.Group;
                                else if (XmlMarker.GetTileCount() > 0) markerType = XmlMarkerType.Tile;
                                else if (XmlMarker.GetCollectionCount() > 0) markerType = XmlMarkerType.Collection;
                                else
                                {
                                    markerType = XmlMarkerType.None;            // deselect
                                    lastMarkerType = XmlMarkerType.Figure;
                                }
                                if (logSelection) Logger.Trace("SetFigureSelectionOnClick   change markerType Figure to {0}   last:{1}", markerType, lastMarkerType);
                            }
                            else if (markerType == XmlMarkerType.Group)
                            {
                                keepMarkerType = true; SelectionHandle.ClearSelected();
                                if (XmlMarker.GetTileCount() > 0) markerType = XmlMarkerType.Tile;
                                else if (XmlMarker.GetCollectionCount() > 0) markerType = XmlMarkerType.Collection;
                                else
                                {
                                    markerType = XmlMarkerType.None;            // deselect
                                    lastMarkerType = XmlMarkerType.Figure;
                                }

                                if (logSelection) Logger.Trace("SetFigureSelectionOnClick   change markerType Group to {0}   last:{1}", markerType, lastMarkerType);
                            }
                            else if (markerType == XmlMarkerType.Tile)
                            {
                                keepMarkerType = true; SelectionHandle.ClearSelected();
                                if (XmlMarker.GetCollectionCount() > 0) markerType = XmlMarkerType.Collection;
                                else
                                {
                                    markerType = XmlMarkerType.None;            // deselect
                                    lastMarkerType = XmlMarkerType.Figure;
                                }
                                if (logSelection) Logger.Trace("SetFigureSelectionOnClick   change markerType Tile to {0}   last:{1}", markerType, lastMarkerType);
                            }
                            else
                            {
                                SelectionHandle.ActualTransform.markerType = markerType = XmlMarkerType.None;
                                lastMarkerType = XmlMarkerType.Figure;
                                VisuGCode.MarkSelectedFigure(-1);
                                StatusStripClear(2);
                                if (logSelection) Logger.Trace("SetFigureSelectionOnClick   ELSE markerType Tile to {0}   last:{1}", markerType, lastMarkerType);
                                return;
                            }

                        }
                        /* 2. find corresponding Gcode-line, by click coordinate picAbsPos */
                        if (markerType != XmlMarkerType.None)
                        { SelectionHandle.SelectedMarkerType = lastMarkerType = markerType; }

                        VisuGCode.MarkerSize = markerSize = (float)((double)Properties.Settings.Default.gui2DSizeTool / (picScaling * zoomFactor));

                        if (!keepMarkerType)
                            markerProperties = VisuGCode.SetPosMarkerNearBy(picAbsPos, ((markerType == XmlMarkerType.Node) || (SelectionHandle.ActualTransform.markerType == XmlMarkerType.Node)));  // find line with coord nearby, mark / unmark figure in GCodeVisuAndTransform.cs

                        clickedLineNr = markerProperties.lineNumber;

                        /* Switch selection if Text is selected */
                        if (!keepMarkerType && LineIsInRange(clickedLineNr) && XmlMarker.GetGroup(clickedLineNr))
                        {
                            if (XmlMarker.lastGroup.Type == "Text")             // reverse marking figure / group if text
                            {
                                if (!modKeyCtrl && SelectionHandle.IsActive)
                                { markerType = XmlMarkerType.Group; }
                                else
                                { markerType = XmlMarkerType.Figure; }
                            }
                        }

                        if (logDetailed || logSelection)
                            Logger.Trace("🗲🗲🗲 SetFigureSelectionOnClick pos:{0:0.0} ; {1:0.0} markerType:{2}  found line:{3}", picAbsPos.X, picAbsPos.Y, markerType, clickedLineNr);

                        fCTBCodeClickedLineNow = clickedLineNr;

                        /* check if line-nr is within tile, group or figure and highlight GCode */
                        /* highlight selected figure or group */
                        if (!FindFigureMarkSelection(markerType, clickedLineNr, markerProperties))
                        {
                            markerType = XmlMarkerType.None;
                            lastMarkerType = XmlMarkerType.Figure;
                        }

                        selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();

                        FoldBlocksByLevel(markerType, clickedLineNr);
                    }
                    //    else if (false)
                    //    { }
                }
                cmsPicBoxMoveToMarkedPosition.ToolTipText = "Work X: " + Grbl.PosMarker.X.ToString() + "   Y: " + Grbl.PosMarker.Y.ToString();
                if (VisuGCode.CodeBlocksAvailable())
                    StatusStripSet(1, Localization.GetString("statusStripeClickKeys2"), Color.LightGreen);
            }
        }

        private void SetSelection(int lineNr, XmlMarkerType mTpe)	// MainFormGetCodeTransform.cs-InsertCodeFromForm; MainFormLoadFile.cs-NewCodeEnd
        {
            clickedLineNr = lineNr;
            //    Logger.Trace("🗲🗲🗲 SetSelection pos:{0:0.0} ; {1:0.0} markerType:{2}  found line:{3}", picAbsPos.X, picAbsPos.Y, markerType, clickedLineNr);
            SelectionHandle.ClearSelected();
            FindFigureMarkSelection(mTpe, clickedLineNr, new DistanceByLine());   // markerProperties);//
            selectionPathOrig = (GraphicsPath)VisuGCode.pathMarkSelection.Clone();
            if (VisuGCode.CodeBlocksAvailable())
                StatusStripSet(1, Localization.GetString("statusStripeClickKeys2"), Color.LightGreen);
            pictureBox1.Invalidate();
        }

        private void PictureBox1_DoubleClick(object sender, MouseEventArgs e)
        {
            //if (logDetailed) 
            Logger.Trace("pictureBox1_DoubleClick {☺}", e.Button);
            if (e.Button == MouseButtons.Middle)
            {
                pBoxTransform.Reset(); zoomFactor = 1;
                pictureBox1.Invalidate();
            }
        }


        // Make array of Matrix to store and reload previous view, instead of back-calculation
        private Matrix pBoxTransform = new Matrix();
        private Matrix pBoxOrig = new Matrix();			// to restore e.Graphics.Transform
        private const float scrollZoomFactor = 1.2f; // zoom factor   
        private float zoomFactor = 1f;
        //private float zoomFactorMin = 1f;
        private float markerSize = 20;
        private bool allowZoom = true;

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!pictureBox1.Focused)
                pictureBox1.Focus();
            if ((pictureBox1.Focused == true) && (e.Delta != 0) && allowZoom)
            { ZoomScroll(e.Location, e.Delta); }
        }

        private XyPoint GetPicBoxOffset(Point mouseLocation)
        {   // backwards calculation to keep real coordinates on mouse-pos. on zoom-in -out
            double minx = VisuGCode.drawingSize.minX;
            double miny = VisuGCode.drawingSize.minY;
            double xRange = (VisuGCode.drawingSize.maxX - VisuGCode.drawingSize.minX);                                              // calculate new size
            double yRange = (VisuGCode.drawingSize.maxY - VisuGCode.drawingSize.minY);

            if (Properties.Settings.Default.machineLimitsFix)
            {
                double offset = (double)Properties.Settings.Default.machineLimitsRangeX / 40;       // view size
                minx = (double)Properties.Settings.Default.machineLimitsHomeX - Grbl.posWCO.X - offset;
                miny = (double)Properties.Settings.Default.machineLimitsHomeY - Grbl.posWCO.Y - offset;
                xRange = (double)Properties.Settings.Default.machineLimitsRangeX + 2 * offset;
                yRange = (double)Properties.Settings.Default.machineLimitsRangeY + 2 * offset;
            }
            double ratioVisu = xRange / yRange;
            double ratioPic = Convert.ToDouble(pictureBox1.Width) / pictureBox1.Height;
            double maxposY = yRange;
            if (ratioVisu > ratioPic)
                maxposY = xRange * pictureBox1.Height / pictureBox1.Width;

            double relposX = (picAbsPos.X - minx) / xRange;
            double relposY = (maxposY - picAbsPos.Y + miny) / yRange;

            if (ratioVisu > ratioPic)
                relposY = relposY * ratioPic / ratioVisu;
            else
                relposX = relposX * ratioVisu / ratioPic;

            XyPoint picOffset = new XyPoint
            {
                X = mouseLocation.X - (relposX * zoomFactor * pictureBox1.Width),
                Y = mouseLocation.Y - (relposY * zoomFactor * pictureBox1.Height)
            };
            return picOffset;
        }

        private void ZoomScroll(Point location, int zoomIn)
        {
            if (showPicBoxBgImage || posIsMoving)              // don't zoom if background image is shown
                return;

            if (zoomIn > 0)
            {
                if (zoomFactor < 10000)
                {
                    zoomFactor *= scrollZoomFactor;
                    XyPoint locationO = GetPicBoxOffset(location);
                    pBoxTransform.Reset();
                    pBoxTransform.Translate((float)locationO.X, (float)locationO.Y);
                    pBoxTransform.Scale(zoomFactor, zoomFactor);
                }
            }
            else if (zoomIn < 0)
            {
                if (zoomFactor > 0.1)
                {
                    zoomFactor *= 1 / scrollZoomFactor;
                    XyPoint locationO = GetPicBoxOffset(location);
                    pBoxTransform.Reset();
                    pBoxTransform.Translate((float)locationO.X, (float)locationO.Y);
                    pBoxTransform.Scale(zoomFactor, zoomFactor);
                }
            }

            markerSize = (float)((double)Properties.Settings.Default.gui2DSizeTool / (picScaling * zoomFactor));
            VisuGCode.CreateMarkerPath(markerSize);
            pictureBox1.Invalidate();
        }

        private void GraphicPropertiesSetup()	// called in MainFormUpdate()
        {
            try
            {
                pictureBox1.BackColor = Properties.Settings.Default.gui2DColorBackground;
                penUp.Color = Properties.Settings.Default.gui2DColorPenUp;
                penDown.Color = Properties.Settings.Default.gui2DColorPenDown;
                penRotary.Color = Properties.Settings.Default.gui2DColorRotaryInfo;
                penHeightMap.Color = Properties.Settings.Default.gui2DColorHeightMap;
                penRuler.Color = Properties.Settings.Default.gui2DColorRuler;
                //	penGrid1.Color = ControlPaint.LightLight(penRuler.Color);
                //	penGrid10.Color = ControlPaint.Light(penRuler.Color);

                penTool.Color = Properties.Settings.Default.gui2DColorTool;
                penMarker.Color = Properties.Settings.Default.gui2DColorMarker;
                penSimulation.Color = Properties.Settings.Default.gui2DColorSimulation;
                penBackgroundPath.Color = Properties.Settings.Default.gui2DColorBackgroundPath;
                brushBackgroundPath.Color = Properties.Settings.Default.gui2DColorBackgroundPath;
                penDimension.Color = Properties.Settings.Default.gui2DColorDimension;

                float factorWidth = 1;
                if (!Properties.Settings.Default.importUnitmm) factorWidth = 0.0393701f;
                if (Properties.Settings.Default.gui2DKeepPenWidth) factorWidth /= zoomFactor;
                penHeightMap.Width = (float)Properties.Settings.Default.gui2DWidthHeightMap * factorWidth;
                penHeightMap.LineJoin = LineJoin.Round;
                penRuler.Width = (float)Properties.Settings.Default.gui2DWidthRuler * factorWidth;
                penUp.Width = (float)Properties.Settings.Default.gui2DWidthPenUp * factorWidth;
                penUp.LineJoin = LineJoin.Round;
                penDown.LineJoin = LineJoin.Round;
                penDown.StartCap = LineCap.Round;
                penDown.EndCap = LineCap.Round;
                penDown.Width = (float)Properties.Settings.Default.gui2DWidthPenDown * factorWidth;

                penRotary.Width = (float)Properties.Settings.Default.gui2DWidthRotaryInfo * factorWidth;
                penRotary.LineJoin = LineJoin.Round;
                penTool.Width = (float)Properties.Settings.Default.gui2DWidthTool * factorWidth;
                penTool.LineJoin = LineJoin.Round;
                penMarker.Width = (float)Properties.Settings.Default.gui2DWidthMarker * factorWidth;
                penMarker.LineJoin = LineJoin.Round;
                penLandMark.LineJoin = LineJoin.Round;
                penLandMark.Width = 2 * (float)Properties.Settings.Default.gui2DWidthPenDown * factorWidth;
                penDimension.LineJoin = LineJoin.Round;
                penDimension.Width = 2 * (float)Properties.Settings.Default.gui2DWidthPenDown * factorWidth;
                penBackgroundPath.LineJoin = LineJoin.Round;
                penBackgroundPath.Width = 2 * (float)Properties.Settings.Default.gui2DWidthPenDown * factorWidth;
                penSimulation.Width = (float)Properties.Settings.Default.gui2DWidthSimulation * factorWidth;
                penSimulation.LineJoin = LineJoin.Round;

                brushMachineLimit = new HatchBrush(HatchStyle.DiagonalCross, Properties.Settings.Default.gui2DColorMachineLimit, Color.Transparent);
                picBoxBackround = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            }
            catch (Exception err)
            {
                Logger.Error(err, "GraphicPropertiesSetup");
                EventCollector.StoreException("GraphicPropertiesSetup " + err.Message);
            }
        }
        #endregion

        #region cmsPictureBox events
        private void CmsPicBoxEnable(bool enable = true)
        {
            Logger.Trace("==== cmsPicBoxEnable {0}", enable);
            cmsPicBoxMoveToMarkedPosition.Enabled = enable;
            cmsPicBoxZeroXYAtMarkedPosition.Enabled = enable;
            cmsPicBoxMoveGraphicsOrigin.Enabled = enable;
            cmsPicBoxResetZooming.Enabled = enable;
            cmsPicBoxSetGCodeAsBackground.Enabled = enable;
            cmsPicBoxClearBackground.Enabled = enable;
        }

        private void CmsPicBoxMoveToMarkedPosition_Click(object sender, EventArgs e)
        {
            SendCommand(String.Format("G0 X{0} Y{1}", Gcode.FrmtNum(Grbl.PosMarker.X), Gcode.FrmtNum(Grbl.PosMarker.Y)).Replace(',', '.'));
        }

        private void CmsPicBoxZeroXYAtMarkedPosition_Click(object sender, EventArgs e)
        {
            XyzPoint tmp = Grbl.posWork - Grbl.PosMarker;
            SendCommand(String.Format(zeroCmd + " X{0} Y{1}", Gcode.FrmtNum(tmp.X), Gcode.FrmtNum(tmp.Y)).Replace(',', '.'));
        }
        private void CmsPicBoxMoveGraphicsOrigin_Click(object sender, EventArgs e)
        {
            UnDo.SetCode(fCTBCode.Text, cmsPicBoxMoveGraphicsOrigin.Text, this);
            if (LineIsInRange(fCTBCodeClickedLineNow))
                ClearTextSelection(fCTBCodeClickedLineNow);
            VisuGCode.MarkSelectedFigure(-1);
            fCTBCode.Text = VisuGCode.TransformGCodeOffset(Grbl.PosMarker.X, Grbl.PosMarker.Y, VisuGCode.Translate.None);
            TransformEnd();
            Grbl.PosMarker = new XyzPoint(0, 0, 0);
        }

        private void CmsPicBoxResetZooming_Click(object sender, EventArgs e)
        { pBoxTransform.Reset(); zoomFactor = 1; }

        private void CmsPicBoxPasteFromClipboard_Click(object sender, EventArgs e)
        {
            UnDo.SetCode(fCTBCode.Text, cmsPicBoxPasteFromClipboard.Text, this);
            LoadFromClipboard();
            EnableCmsCodeBlocks(VisuGCode.CodeBlocksAvailable());
        }

        private void CmsPicBoxReloadFile_Click(object sender, EventArgs e)
        {
            Logger.Trace("CmsPicBoxReloadFile_Click  tooltip:{0}   file:{1}", cmsPicBoxReloadFile.ToolTipText, lastLoadFile);
            ToolStripMenuItem tmp = (ToolStripMenuItem)sender;
            ReStartConvertFile(sender, e, tmp.Name.Contains("2") ? 1 : 0);
        }

        private void CmsPicBoxClearWorkspace_Click(object sender, EventArgs e)
        { ClearWorkspace(); }
        private void ClearWorkspace()
        {
            Logger.Info("▀▀▀▀▀▀▀▀▀▀ Clear Workspace ▀▀▀▀▀▀▀▀▀▀");
            UnDo.SetCode(fCTBCode.Text, cmsPicBoxClearWorkspace.Text, this);
            GuiVariables.offsetOriginX = (double)Properties.Settings.Default.importGraphicOffsetOriginX;
            GuiVariables.offsetOriginY = (double)Properties.Settings.Default.importGraphicOffsetOriginY;
            Graphic2GCode.multiImport = false;
            Graphic2GCode.multiImportNr = 0;
            Graphic2GCode.multiImportName = "";
            Graphic.actualDimension = new Dimensions();
            NewCodeStart();         // ClearWorkspace
            fCTBCode.Clear();
            NewCodeEnd();
            globalCollectionCounter = 1;
            _heightmap_form?.SetBtnApply(true);
            isHeightMapApplied = false;
        }

        private void CmsPicBoxMoveToFirstPos_Click(object sender, EventArgs e)
        {
            FctbSetBookmark();
            fCTBCode.DoCaretVisible();
            return;
        }

        private void CmsPicBoxDuplicatePath_Click(object sender, EventArgs e)
        { DuplicateSelectedPath(); }

        private void DuplicateSelectedPath()
        {
            UnDo.SetCode(fCTBCode.Text, cmsPicBoxDuplicatePath.Text, this);
            Range range = fCTBCode.Selection.Clone();
            Logger.Trace("+++ DuplicateSelectedPath figureIsMarked:{0}  lastMarkerType:{1}   range:{2}", figureIsMarked, lastMarkerType, range);
            if (figureIsMarked && (lastMarkerType != XmlMarkerType.Node))
            {
                resetView = true;
                string tmpCode = fCTBCode.SelectedText;         // get selected code
                int line = fCTBCodeClickedLineNow;              // start line
                                                                //    if ((lastMarkerType != XmlMarkerType.Figure)|| (lastMarkerType != XmlMarkerType.Group))     // insert figure into same group, insert any other as new group
                                                                //        line = 0;

                /* calculate insertion offset */
                double offsetX = (double)Properties.Settings.Default.gui2DDuplicateOffsetX;
                double offsetY = (double)Properties.Settings.Default.gui2DDuplicateOffsetY;
                if (Properties.Settings.Default.gui2DDuplicateAddDimensionX)
                { offsetX += (double)SelectionHandle.Bounds.Width; }
                if (Properties.Settings.Default.gui2DDuplicateAddDimensionY)
                { offsetY += (double)SelectionHandle.Bounds.Height; }

                VisuGCode.MarkSelectedFigure(-1);
                int insertLine = InsertCodeToFctb(tmpCode, false, line, offsetX, offsetY);
                TransformEnd();     // reload GCode an analyze
                                    // Figure was duplicated and code inserted, next -> select and highlight new code snipped

                markerType = SelectionHandle.SelectedMarkerType;

                if (markerType == XmlMarkerType.Figure)
                    VisuGCode.MarkSelectedFigure(XmlMarker.lastFigure.FigureNr);
                else if (markerType == XmlMarkerType.Group)
                    VisuGCode.MarkSelectedGroup(XmlMarker.lastGroup.LineStart);

                SelectionHandle.SelectedMarkerLine = insertLine;
                markerSize = (float)((double)Properties.Settings.Default.gui2DSizeTool / (picScaling * zoomFactor));
                VisuGCode.CreateMarkerPath(markerSize);
                cmsPicBoxReverseSelectedPath.Enabled = false;
                cmsPicBoxRotateSelectedPath.Enabled = false;

                fCTBCode.Selection = range; // SetTextSelection
                if (LineIsInRange(insertLine))
                    FoldBlocksByLevel(markerType, insertLine);
                //	fCTBCode.DoSelectionVisible();      // will unfold blocks
            }
            return;
        }
        private void CmsPicBoxDeletePath_Click(object sender, EventArgs e)
        {
            UnDo.SetCode(fCTBCode.Text, cmsPicBoxDeletePath.Text, this);
            Logger.Trace("▌ cmsPicBoxDeletePath figureIsMarked:{0}  markedBlockType:{1}", figureIsMarked, markedBlockType);
            if (figureIsMarked)
            {
                int id = XmlMarker.lastGroup.Id;
                if (markedBlockType == XmlMarkerType.Figure)
                    id = XmlMarker.lastFigure.Id;
                if (markedBlockType == XmlMarkerType.Tile)
                    id = XmlMarker.lastTile.Id;
                resetView = true;
                fCTBCode.InsertText(string.Format("( Selection removed: {0} Id:{1})", markedBlockType, id));
                SelectionHandle.IsActive = false;
                NewCodeEnd();   // reload GCode to update XML-start / end information
                FoldBlocksByLevel(foldLevelSelected);
                //    TransformEnd();
            }
            return;
        }
        private void CmsPicBoxShowProperties_Click(object sender, EventArgs e)
        {
            UnDo.SetCode(fCTBCode.Text, cmsPicBoxShowProperties.Text, this);
            Logger.Trace("▌ cmsPicBoxShowProperties figureIsMarked:{0}", figureIsMarked);
            SelectionPropertiesDialog();
            return;
        }

        private void CmsPicBoxCropSelectedPath_Click(object sender, EventArgs e)
        {
            if (manualEdit)
                return;
            TransformStart(cmsPicBoxCropSelectedPath.Text);//, false);
            XmlMarker.BlockData tmp = new XmlMarker.BlockData();

            Logger.Trace("▌ cmsPicBoxCropSelectedPath groupCnt:{0} figureCnt:{1}  markedBlockType:{2}", XmlMarker.GetGroupCount(), XmlMarker.GetFigureCount(), markedBlockType);

            if ((XmlMarker.GetGroupCount() > 0) && (markedBlockType == XmlMarkerType.Group))
            {
                if (XmlMarker.GetGroupNext(ref tmp))    // get range group+1-start to group+n-end
                {
                    int startNextGroup = tmp.LineStart;
                    if (XmlMarker.GetGroupLast(ref tmp))
                    {
                        int endLastGroup = tmp.LineEnd;
                        SetTextSelection(startNextGroup, endLastGroup);
                        fCTBCode.SelectedText = string.Format("( Group removed line {0} to {1} )", startNextGroup, endLastGroup);
                        Logger.Trace("▌ cmsPicBoxCropSelectedPath after from: {0} to {1}", startNextGroup, endLastGroup);
                    }
                }
                if (XmlMarker.GetGroupPrev(ref tmp))    // get range group0-start to group-1-end
                {
                    int endPrevGroup = tmp.LineEnd;
                    if (XmlMarker.GetGroupFirst(ref tmp) && (tmp.LineStart != XmlMarker.lastGroup.LineStart))
                    {
                        int startFirstGroup = tmp.LineStart;
                        SetTextSelection(startFirstGroup, endPrevGroup);
                        fCTBCode.SelectedText = string.Format("( Group removed line {0} to {1} )", startFirstGroup, endPrevGroup);
                        Logger.Trace("▌ cmsPicBoxCropSelectedPath before from: {0} to {1}", startFirstGroup, endPrevGroup);
                    }
                }
            }
            else if ((XmlMarker.GetFigureCount() > 0) && (markedBlockType == XmlMarkerType.Figure))
            {
                bool isGroup = XmlMarker.GetGroup(XmlMarker.lastFigure.LineStart);

                if (XmlMarker.GetFigureNext(ref tmp) && (!isGroup || (tmp.LineEnd < XmlMarker.lastGroup.LineEnd)))    // next figure must be inside same group
                {
                    int startNextFigure = tmp.LineStart;
                    if (XmlMarker.GetFigureLast(ref tmp) && (tmp.LineStart != XmlMarker.lastFigure.LineStart))
                    {
                        int endLastFigure = tmp.LineEnd;
                        if (isGroup) { endLastFigure = XmlMarker.lastGroup.LineEnd - 1; }
                        SetTextSelection(startNextFigure, endLastFigure);
                        fCTBCode.SelectedText = string.Format("( Figure removed line {0} to {1} )", startNextFigure, endLastFigure);
                        Logger.Trace("▌ cmsPicBoxCropSelectedPath after from: {0} to {1}", startNextFigure, endLastFigure);
                    }
                }
                if (XmlMarker.GetFigurePrev(ref tmp) && (!isGroup || (tmp.LineEnd > XmlMarker.lastGroup.LineStart)))    // prev figure must be inside same group
                {
                    int endPrevFigure = tmp.LineEnd;
                    if (XmlMarker.GetFigureFirst(ref tmp) && (tmp.LineStart != XmlMarker.lastFigure.LineStart))
                    {
                        int startFirstFigure = tmp.LineStart;
                        if (isGroup) { startFirstFigure = XmlMarker.lastGroup.LineStart + 1; }
                        SetTextSelection(startFirstFigure, endPrevFigure);
                        fCTBCode.SelectedText = string.Format("( Figure removed line {0} to {1} )", startFirstFigure, endPrevFigure);
                        Logger.Trace("▌ cmsPicBoxCropSelectedPath before from: {0} to {1}", startFirstFigure, endPrevFigure);
                    }
                }



                /*        if (XmlMarker.GetFigure(fCTBCodeClickedLineNow, 10))    // get range group+1-start to group+n-end
                        {
                            SetTextSelection(XmlMarker.lastFigure.LineStart, XmlMarker.lastFigure.LineEnd);
                            fCTBCode.SelectedText = "( Figure removed )\r\n";
                        }
                        if (XmlMarker.GetFigure(fCTBCodeClickedLineNow, -10))    // get range group0-start to group-1-end
                        {
                            SetTextSelection(XmlMarker.lastFigure.LineStart, XmlMarker.lastFigure.LineEnd);
                            fCTBCode.SelectedText = "( Figure removed )\r\n";
                        }*/
            }
            else
                fCTBCode.Text = VisuGCode.CutOutFigure();

            NewCodeEnd();
            //    TransformEnd();
            FoldBlocksByLevel(foldLevelSelected);
            VisuGCode.pathBackground.Reset();
        }

        private void CmsPicBoxMoveSelectedPathInCode_Click(object sender, EventArgs e)
        {	//verwendung?
            TransformStart(cmsPicBoxMoveSelectedPathInCode.Text);	// set undo text
            Logger.Trace("▌ cmsPicBoxMoveSelectedPathInCode dX:{0:0.000} dY:{1:0.000}", (posMoveEnd.X - posMoveStart.X), (posMoveEnd.Y - posMoveStart.Y));
            zoomFactor = 1;
            fCTBCode.Text = VisuGCode.TransformGCodeOffset(-(posMoveEnd.X - posMoveStart.X), -(posMoveEnd.Y - posMoveStart.Y), VisuGCode.Translate.None);
            fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
            fCTBCodeClickedLineLast = 0;
            TransformEnd();						// anaylize GCode from FCTB...
            StatusStripClear(2);    			// CmsPicBoxMoveSelectedPathInCode_Click
            cmsPicBoxMoveSelectedPathInCode.Enabled = false;
            cmsPicBoxMoveSelectedPathInCode.BackColor = SystemColors.Control;
        }

        private void CmsPicBoxReverseSelectedPath_Click(object sender, EventArgs e)
        {   // picAbsPos doesn't work, because selecting entry in CMS changes coordinates
            // posMoveStart will be stored on left-click
            if (figureIsMarked)
            {
                if (Graphic.SizeOk())
                {
                    resetView = true;
                    SetEditMode(false);
                    VisuGCode.MarkSelectedFigure(-1);           // hide highlight
                    VisuGCode.pathBackground.Reset();

                    Logger.Info("▌ Reverse path figureNr:{0}  figureOrder:{1}  GroupIdOrder:{2}", XmlMarker.lastFigure.FigureNr, string.Join(",", XmlMarker.GetFigureIdOrder()), string.Join(",", XmlMarker.GetGroupIdOrder()));
                    if (Graphic.ReDoReversePath(XmlMarker.lastFigure.FigureNr, posMoveStart, XmlMarker.GetFigureIdOrder(), XmlMarker.GetGroupIdOrder(), true))
                    {
                        SetFctbCodeText(Graphic.GCode.ToString());      // cmsPicBoxReverseSelectedPath_Click
                        NewCodeEnd();
                    }
                    else
                    {
                        MessageBox.Show("Reverse path faild - graphic needs to be imported.\r\nRaw G-Code file doesn't work", "Attention");
                    }
                }
            }
            return;
        }
        private void CmsPicBoxRotateSelectedPath_Click(object sender, EventArgs e)
        {
            if (figureIsMarked)
            {
                if (Graphic.SizeOk())
                {
                    resetView = true;
                    SetEditMode(false);
                    VisuGCode.MarkSelectedFigure(-1);           // hide highlight
                    VisuGCode.pathBackground.Reset();

                    Logger.Info("▌ Rotate path figureNr:{0}  figureOrder:{1}  GroupIdOrder:{2}", XmlMarker.lastFigure.FigureNr, string.Join(",", XmlMarker.GetFigureIdOrder()), string.Join(",", XmlMarker.GetGroupIdOrder()));
                    if (Graphic.ReDoReversePath(XmlMarker.lastFigure.FigureNr, posMoveStart, XmlMarker.GetFigureIdOrder(), XmlMarker.GetGroupIdOrder(), false))
                    {
                        SetFctbCodeText(Graphic.GCode.ToString());      // cmsPicBoxReverseSelectedPath_Click
                        NewCodeEnd();
                    }
                    else
                    {
                        MessageBox.Show("Rotation faild - graphic needs to be imported.\r\nRaw G-Code file doesn't work", "Attention");
                    }
                }
            }
            return;
        }

        private void CmsPicBoxSetGCodeAsBackground_Click(object sender, EventArgs e)
        {
            VisuGCode.SetPathAsLandMark(false);//clear = false
            Properties.Settings.Default.guiBackgroundShow = toolStripViewBackground.Checked = true;
        }

        private void CmsPicBoxClearBackground_Click(object sender, EventArgs e)
        { VisuGCode.SetPathAsLandMark(true); }

        #endregion

        private void CmsPictureBox_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = mouseWasMoved;
        }
        private void PictureBox1_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }


        private void SelectionPropertiesDialog()
        {
            double penWidth = 0;
            string penColor = "";
            int line = 0;
            using (GCodeSelectionProperties f = new GCodeSelectionProperties())
            {
                f.CenterX = (double)SelectionHandle.ActualTransform.center.X;
                f.CenterY = (double)SelectionHandle.ActualTransform.center.Y;
                if (lastMarkerType == XmlMarkerType.Figure)
                {
                    f.AttributePenWidth = penWidth = XmlMarker.lastFigure.PenWidth;
                    f.AttributePenColor = penColor = XmlMarker.lastFigure.PenColor;
                    line = XmlMarker.lastFigure.LineStart;
                }
                else if (lastMarkerType == XmlMarkerType.Group)
                {
                    f.AttributePenWidth = penWidth = XmlMarker.lastGroup.PenWidth;
                    f.AttributePenColor = penColor = XmlMarker.lastGroup.PenColor;
                    line = XmlMarker.lastGroup.LineStart;
                }
                var result = f.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    if (f.AttributeWasChanged)
                    {
                        if (f.CenterWasChanged && (line > 0))
                        {
                            string tmpCode = VisuGCode.GetGcodeListLine(line);
                            if (penWidth > 0) { ReplaceXmlAttribute(ref tmpCode, XmlMarker.FigureStart, "PenWidth", string.Format("{0:0.00}", f.AttributePenWidth)); }
                            if (penColor.Length > 2) { ReplaceXmlAttribute(ref tmpCode, XmlMarker.FigureStart, "PenColor", f.AttributePenColor); }
                            VisuGCode.SetGcodeListLine(line, tmpCode);
                        }
                        else
                        {
                            string tmpCode = fCTBCode.SelectedText;
                            if (penWidth > 0) { ReplaceXmlAttribute(ref tmpCode, XmlMarker.FigureStart, "PenWidth", string.Format("{0:0.00}", f.AttributePenWidth)); }
                            if (penColor.Length > 2) { ReplaceXmlAttribute(ref tmpCode, XmlMarker.FigureStart, "PenColor", f.AttributePenColor); }
                            fCTBCode.SelectedText = tmpCode;
                            TransformEnd();
                        }
                    }
                    if (f.CenterWasChanged)
                    {
                        double dx = (double)SelectionHandle.ActualTransform.center.X - f.CenterX;
                        double dy = (double)SelectionHandle.ActualTransform.center.Y - f.CenterY;
                        SetFctbCodeText(VisuGCode.TransformGCodeOffset(dx, dy, VisuGCode.Translate.None));
                        fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
                        fCTBCodeClickedLineLast = 0;
                        TransformEnd();
                        VisuGCode.MarkSelectedFigure(-1);
                        Cursor = Cursors.Default;
                    }

                }
            }
        }

        private void ReplaceXmlAttribute(ref string code, string xmlTag, string xmlAtt, string newVal)
        {
            int posTagStart = code.IndexOf(xmlTag);
            if (posTagStart < 0) return;
            int posTagEnd = code.IndexOf(">", posTagStart + xmlTag.Length);
            if (posTagEnd < 0) return;
            int posAttStart = code.IndexOf(xmlAtt, posTagStart);
            int posAttEnd = code.IndexOf("\"", posAttStart + xmlAtt.Length + 3);
            string origAtt = code.Substring(posAttStart, posAttEnd - posAttStart + 1);
            string newAtt = string.Format("{0}=\"{1}\"", xmlAtt, newVal);
            //    MessageBox.Show("-"+origAtt+"-\r\n-"+newAtt+"-");
            code = code.Replace(origAtt, newAtt);
        }

        private void ResetPicBoxSelections()
        {
            SelectionHandle.ClearSelected();
            VisuGCode.MarkSelectedFigure(-1);
            lastMarkerType = XmlMarkerType.Figure;
            pictureBox1.Invalidate();
        }
    }
}
