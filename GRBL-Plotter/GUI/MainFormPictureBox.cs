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
*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private readonly Pen penRuler = new Pen(Color.Blue, 0.1F);
        private readonly Pen penTool = new Pen(Color.Black, 0.5F);
        private readonly Pen penMarker = new Pen(Color.DeepPink, 1F);
        private readonly Pen penLandMark = new Pen(Color.DarkGray, 1F);
        private readonly Pen penBackgroundPath = new Pen(Color.DarkGray, 1F);
        private readonly Pen penDimension = new Pen(Color.DarkGray, 1F);
        //        private Brushes brushLandMark = new Brushes();
        private readonly Pen penSimulation = new Pen(Color.Blue, 0.4F);
        //       SolidBrush machineLimit = new SolidBrush(Color.Red);
        private HatchBrush brushMachineLimit = new HatchBrush(HatchStyle.Horizontal, Color.Yellow);
        private readonly SolidBrush brushBackground = new SolidBrush(Color.White);
        private readonly SolidBrush brushBackgroundPath = new SolidBrush(Color.DarkGray);

        private XyPoint picAbsPos = new XyPoint(0, 0);
        private Bitmap picBoxBackround;
        private bool showPicBoxBgImage = false;
        private bool showPathPenUp = true;
        private bool showPaths = false;
        private XyPoint posMoveStart = new XyPoint(0, 0);
        private XyPoint posMoveTmp = new XyPoint(0, 0);
        private XyPoint posMoveEnd = new XyPoint(0, 0);
        private XyPoint moveTranslation = new XyPoint(0, 0);
        private XyPoint moveTranslationOld = new XyPoint(0, 0);
        private bool posIsMoving = false;
        private bool mouseWasMoved = false;
        private Point mouseDownPos = new Point();
        private Point mouseMovePos = new Point();
        private readonly Font myFont7 = new Font("Lucida Console", 7);
		private readonly Font myFont8 = new Font("Lucida Console", 8);

        private readonly object lockObject = new object();
        private bool shiftedDisplay = false;

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

            double picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit
            if ((picScaling > 0.001) && (picScaling < 10000))
            {
                double offsetX = 0;// pBoxTransform.OffsetX;
                double offsetY = 0;// pBoxTransform.OffsetY;
                try { offsetX = pBoxTransform.OffsetX; offsetY = pBoxTransform.OffsetY; } catch (Exception err){ Logger.Error(err, "PictureBox1_Paint - pBoxTransform1 nok"); } // pBoxTransform.Dispose  too early
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

                if (posIsMoving)
                    posMoveEnd = picAbsPos;

                /* Label position */
                int offX = +5, offY = -10;
                if (pictureBox1.PointToClient(MousePosition).X > (pictureBox1.Width / 2)) { offX = -75; }
                if (pictureBox1.PointToClient(MousePosition).Y > (pictureBox1.Height / 2)) { offY = -30; }

                Point stringpos = new Point(pictureBox1.PointToClient(MousePosition).X + offX, pictureBox1.PointToClient(MousePosition).Y + offY);

                pBoxOrig = e.Graphics.Transform;
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
                            {   e.Graphics.TranslateTransform(-(float)VisuGCode.ProcessedPath.offset2DView.X, -(float)VisuGCode.ProcessedPath.offset2DView.Y);       // apply offset
                                e.Graphics.DrawPath(penMarker, VisuGCode.pathMarker);
                                e.Graphics.TranslateTransform((float)VisuGCode.ProcessedPath.offset2DView.X, (float)VisuGCode.ProcessedPath.offset2DView.Y);       // remove offset
                            }
                            else
                                e.Graphics.DrawPath(penMarker, VisuGCode.pathMarker);
                        }
                        e.Graphics.Transform = pBoxOrig;
                        if (Properties.Settings.Default.machineLimitsShow)
                        {
                            e.Graphics.FillRectangle(brushBackground, new Rectangle(stringpos.X, stringpos.Y - 2, 75, 34));
                            e.Graphics.FillRectangle(brushBackground, new Rectangle(18, 3, 140, 60));
                            if (VisuGCode.selectedFigureInfo.Length > 0)
                                e.Graphics.FillRectangle(brushBackground, new Rectangle(148, 3, 160, 60));
                        }
                        if (Properties.Settings.Default.gui2DInfoShow)
                        {
                            string unit = (Properties.Settings.Default.importUnitmm) ? "mm" : "Inch";
                            e.Graphics.DrawString(String.Format("Work-Pos:\r\nX:{0,7:0.000}\r\nY:{1,7:0.000}", picAbsPos.X, picAbsPos.Y), myFont8, Brushes.Black, stringpos);
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

        private void OnPaint_scaling(Graphics e)
        {
            double minx = VisuGCode.drawingSize.minX;                  // extend dimensions
            double maxx = VisuGCode.drawingSize.maxX;
            double miny = VisuGCode.drawingSize.minY;
            double maxy = VisuGCode.drawingSize.maxY;
            double xRange = (maxx - minx);                                              // calculate new size
            double yRange = (maxy - miny);
            double picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit
            e.ScaleTransform((float)picScaling, (float)-picScaling);        // apply scaling (flip Y)
            e.TranslateTransform((float)-minx, (float)(-yRange - miny));       // apply offset
        }
        private void OnPaint_drawToolPath(Graphics e)
        {
            try
            {
                float factorWidth = 1;
                if (!Properties.Settings.Default.importUnitmm) factorWidth = 0.0393701f;
                if (Properties.Settings.Default.gui2DKeepPenWidth) factorWidth /= zoomFactor;
                penHeightMap.Width = (float)Properties.Settings.Default.gui2DWidthHeightMap * factorWidth;
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
                    e.DrawPath(penRuler, VisuGCode.pathRuler);

                if (Properties.Settings.Default.gui2DColorPenDownModeEnable && (!VisuGCode.largeDataAmount) && (VisuGCode.pathObject.Count > 0) && (VisuGCode.pathObject[0].path.PointCount > 0))    // Show PenDown path in colors from imported graphics
                {
                    //if (VisuGCode.pathObject.Count > 0)
                    {	if (VisuGCode.ShiftTilePaths)
                        {
                            // GCodeSimulate.cs VisuGCode.ProcessedPath.offset2DView updated via 
                            // MainFormStreaming.cs VisuGCode.ProcessedPath.ProcessedPathLine(e.CodeLineConfirmed);	
                            // VisuGCode.ProcessedPath.offset2DView depends on importGraphicClipShowOrigPositionShiftTileProcessed
                            shiftedDisplay = true;
                        }
                        if (shiftedDisplay)
                            e.TranslateTransform(-(float)VisuGCode.ProcessedPath.offset2DView.X, -(float)VisuGCode.ProcessedPath.offset2DView.Y);       // apply offset

						foreach (VisuGCode.PathData tmpPath in VisuGCode.pathObject)
						{ e.DrawPath(tmpPath.pen, tmpPath.path); }

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
                    e.DrawPath(penLandMark, VisuGCode.pathMarkSelection);
                else
                    e.DrawPath(penHeightMap, VisuGCode.pathMarkSelection);

                if (!(showPathPenUp ^ toolStripViewPenUp.Checked))  //Properties.Settings.Default.gui2DPenUpShow))
                    e.DrawPath(penUp, VisuGCode.pathPenUp);

                if (shiftedDisplay)
                    e.TranslateTransform((float)VisuGCode.ProcessedPath.offset2DView.X, (float)VisuGCode.ProcessedPath.offset2DView.Y);       // remove offset

                e.DrawPath(penSimulation, VisuGCode.Simulation.pathSimulation);
            }
            catch (Exception Ex){ 
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

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMovePos = e.Location;
            if ((e.Button == MouseButtons.Middle) || (e.Button == MouseButtons.Right))
            {
                XyPoint diff = posMoveEnd - posMoveTmp;
                posMoveTmp = posMoveEnd;
                if (VisuGCode.pathMarkSelection.PointCount > 1)     // move selected path
                {
                    Matrix tmp = new Matrix();
                    tmp.Translate((float)diff.X, (float)diff.Y);
                    VisuGCode.pathMarkSelection.Transform(tmp);
                    tmp.Dispose();
                    posIsMoving = true;
                }
                else
                {
                    if (PointDistance(mouseMovePos,mouseDownPos) > 20) mouseWasMoved = true;
                    posIsMoving = false;                            // move view
                    moveTranslation = new XyPoint(e.X, e.Y) - moveTranslationOld;  // calc delta move
                    pBoxTransform.Translate((float)moveTranslation.X / zoomFactor, (float)moveTranslation.Y / zoomFactor);
                    moveTranslationOld = new XyPoint(e.X, e.Y);
                }
                if ((diff.X != 0) || (diff.Y != 0))
                    previousClick = 0;  // no doubleclick

                StatusStripSet(2, string.Format("{0} X:{1:0.00}  Y:{2:0.00}", Localization.GetString("statusStripeSelectionMoved"), (posMoveEnd.X - posMoveStart.X), (posMoveEnd.Y - posMoveStart.Y)), Color.Yellow);
            }
            pictureBox1.Invalidate();
        }
        private int PointDistance(Point a, Point b)
        {
            int x = a.X - b.X;
            int y = a.Y - b.Y;
            return (int)Math.Sqrt(x*x + y*y);
        }
        private void MoveView(double x, double y)
        {
            x = x * 10 * zoomFactor;
            y = y * 10 * zoomFactor;
            posIsMoving = false;                            // move view
            moveTranslation = new XyPoint(x, y);  // calc delta move
            pBoxTransform.Translate((float)moveTranslation.X / zoomFactor, (float)moveTranslation.Y / zoomFactor);
            moveTranslationOld = new XyPoint(x, y);
        }
        private static MouseButtons _lastButtonUp = MouseButtons.None;
        private static int previousClick = SystemInformation.DoubleClickTime;
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (logDetailed) Logger.Trace("pictureBox1_MouseUp   e.x:{0} y:{1}  absPos-x:{2:0.00} y:{3:0.00}", e.X, e.Y, picAbsPos.X, picAbsPos.Y);
            if (posIsMoving)
            {
                cmsPicBoxMoveSelectedPathInCode.Enabled = true;
                cmsPicBoxMoveSelectedPathInCode.BackColor = Color.Yellow;
                StatusStripSet(2, string.Format("{0} X:{1:0.00}  Y:{2:0.00} - click [{3}] to apply", Localization.GetString("statusStripeSelectionMoved"), (posMoveEnd.X - posMoveStart.X), (posMoveEnd.Y - posMoveStart.Y), cmsPicBoxMoveSelectedPathInCode.Text), Color.Yellow);
            }
            posIsMoving = false;
            allowZoom = true;
            _lastButtonUp = e.Button;

            int now = System.Environment.TickCount;
            int diff = now - previousClick;
            //           lbInfo.Text = SystemInformation.DoubleClickTime.ToString() + "   " + diff.ToString();
            if ((diff <= SystemInformation.DoubleClickTime) && (diff > 100))
            { PictureBox1_DoubleClick(sender, e); }
            previousClick = now;
        }

        // find closest coordinate in GCode and mark
        private bool expandGCode = true;
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownPos = e.Location;
            if (logDetailed) Logger.Trace("pictureBox1_MouseDown e.x:{0} y:{1}  absPos-x:{2:0.00} y:{3:0.00}", e.X, e.Y, picAbsPos.X, picAbsPos.Y);
            pictureBox1.Focus();
            moveTranslationOld = new XyPoint(e.X, e.Y);
            allowZoom = false;
            mouseWasMoved = false;
            if (e.Button == MouseButtons.Left)
            {
                if ((fCTBCode.LinesCount > 2) && !posIsMoving)
                {
                    int line;
                    DistanceByLine marker;
                    posMoveStart = picAbsPos;
                    posMoveTmp = posMoveStart;
                    posMoveEnd = posMoveStart;

                    int fold = foldLevel;
                    expandGCode = Properties.Settings.Default.FCTBBlockExpandOnSelect;
                    if (!expandGCode)
                        fold = 0;

                    if (manualEdit)
                        NewCodeEnd();
                    if (Panel.ModifierKeys == Keys.Alt)
                    {
                        marker = VisuGCode.SetPosMarkerNearBy(picAbsPos, false);           // Keys.Alt find line with coord nearby, mark / unmark figure
                        line = marker.lineNumber;
                        fCTBCodeClickedLineNow = line;
                        FindFigureMarkSelection(XmlMarkerType.Line, line, (fold > 0));    // collapse=true
                    }
                    else if (Panel.ModifierKeys == Keys.Control)
                    {
                        marker = VisuGCode.SetPosMarkerNearBy(picAbsPos, false);           // Keys.Control find line with coord nearby, mark / unmark figure
                        line = marker.lineNumber;
                        fCTBCodeClickedLineNow = line;
                        FindFigureMarkSelection(XmlMarkerType.Group, line, (fold > 0));
                    }
                    else if (Panel.ModifierKeys == Keys.Shift)
                    {
                        marker = VisuGCode.SetPosMarkerNearBy(picAbsPos, false);           // Keys.Shift find line with coord nearby, mark / unmark figure
                        line = marker.lineNumber;
                        fCTBCodeClickedLineNow = line;
                        FindFigureMarkSelection(XmlMarkerType.Tile, line, (fold > 0));
                    }
                    else
                    {
                        marker = VisuGCode.SetPosMarkerNearBy(picAbsPos, true);                 // find line with coord nearby, mark / unmark figure
                        line = marker.lineNumber;
                        fCTBCodeClickedLineNow = line;
                        FindFigureMarkSelection(XmlMarkerType.Figure, line, (fold > 0));
                    }
                    Grbl.PosMarker = marker.actualPos;
                    VisuGCode.CreateMarkerPath();

                    cmsPicBoxMoveToMarkedPosition.ToolTipText = "Work X: " + Grbl.PosMarker.X.ToString() + "   Y: " + Grbl.PosMarker.Y.ToString();
                    EnableBlockCommands(VisuGCode.GetHighlightStatus() > 0);
                    if (VisuGCode.CodeBlocksAvailable())
                        StatusStripSet(1, Localization.GetString("statusStripeClickKeys2"), Color.LightGreen);
                    posIsMoving = false;
                }
            }
        }

        private void PictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (logDetailed) Logger.Trace("pictureBox1_DoubleClick");
            if (_lastButtonUp == MouseButtons.Middle)
            {
                pBoxTransform.Reset(); zoomFactor = 1;
                pictureBox1.Invalidate();
            }
        }


        // Make array of Matrix to store and reload previous view, instead of back-calculation
        private readonly Matrix pBoxTransform = new Matrix();
        private Matrix pBoxOrig = new Matrix();			// to restore e.Graphics.Transform
        private const float scrollZoomFactor = 1.2f; // zoom factor   
        private float zoomFactor = 1f;
        private bool allowZoom = true;

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
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
                if (zoomFactor < 800)
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
            if (Math.Round(zoomFactor, 2) == 1.00)
            {
                pBoxTransform.Reset(); zoomFactor = 1;
            }

            pictureBox1.Invalidate();
        }

        private void GraphicPropertiesSetup()
        {
            pictureBox1.BackColor = Properties.Settings.Default.gui2DColorBackground;
            penUp.Color = Properties.Settings.Default.gui2DColorPenUp;
            penDown.Color = Properties.Settings.Default.gui2DColorPenDown;
            penRotary.Color = Properties.Settings.Default.gui2DColorRotaryInfo;
            penHeightMap.Color = Properties.Settings.Default.gui2DColorHeightMap;
            penRuler.Color = Properties.Settings.Default.gui2DColorRuler;
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
        #endregion

        #region cmsPictureBox events
        private void CmsPicBoxEnable(bool enable = true)
        {
            Logger.Trace("cmsPicBoxEnable {0}", enable);
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
        { ReStartConvertFile(sender, e); }

        private void CmsPicBoxMoveToFirstPos_Click(object sender, EventArgs e)
        {
            FctbCodeMarkLine();
            fCTBCode.DoCaretVisible();
            return;
        }

        private void CmsPicBoxDeletePath_Click(object sender, EventArgs e)
        {
            UnDo.SetCode(fCTBCode.Text, cmsPicBoxDeletePath.Text, this);
            Logger.Trace("▌ cmsPicBoxDeletePath figureIsMarked:{0}", figureIsMarked);
            if (figureIsMarked)
            {
                resetView = true;
                fCTBCode.InsertText("( Figure removed )\r\n");
                TransformEnd();
            }
            return;
        }

        private void CmsPicBoxCropSelectedPath_Click(object sender, EventArgs e)
        {
            if (manualEdit)
                return;
            TransformStart(cmsPicBoxCropSelectedPath.Text);//, false);

            Logger.Trace("▌ cmsPicBoxCropSelectedPath groupCnt:{0} figureCnt:{1}", XmlMarker.GetGroupCount(), XmlMarker.GetFigureCount());

            if ((XmlMarker.GetGroupCount() > 0) && (markedBlockType == XmlMarkerType.Group))
            {
                if (XmlMarker.GetGroup(fCTBCodeClickedLineNow, 10))    // get range group+1-start to group+n-end
                {
                    SetTextSelection(XmlMarker.lastGroup.LineStart, XmlMarker.lastGroup.LineEnd);
                    fCTBCode.SelectedText = "( Figure removed )\r\n";
                }
                if (XmlMarker.GetGroup(fCTBCodeClickedLineNow, -10))    // get range group0-start to group-1-end
                {
                    SetTextSelection(XmlMarker.lastGroup.LineStart, XmlMarker.lastGroup.LineEnd);
                    fCTBCode.SelectedText = "( Figure removed )\r\n";
                }
            }
            else if ((XmlMarker.GetFigureCount() > 0) && (markedBlockType == XmlMarkerType.Figure))
            {
                if (XmlMarker.GetFigure(fCTBCodeClickedLineNow, 10))    // get range group+1-start to group+n-end
                {
                    SetTextSelection(XmlMarker.lastFigure.LineStart, XmlMarker.lastFigure.LineEnd);
                    fCTBCode.SelectedText = "( Figure removed )\r\n";
                }
                if (XmlMarker.GetFigure(fCTBCodeClickedLineNow, -10))    // get range group0-start to group-1-end
                {
                    SetTextSelection(XmlMarker.lastFigure.LineStart, XmlMarker.lastFigure.LineEnd);
                    fCTBCode.SelectedText = "( Figure removed )\r\n";
                }
            }
            else
                fCTBCode.Text = VisuGCode.CutOutFigure();
            TransformEnd();
        }

        private void CmsPicBoxMoveSelectedPathInCode_Click(object sender, EventArgs e)
        {
            TransformStart(cmsPicBoxMoveSelectedPathInCode.Text);
            Logger.Trace("▌ cmsPicBoxMoveSelectedPathInCode dX:{0:0.000} dY:{1:0.000}", (posMoveEnd.X - posMoveStart.X), (posMoveEnd.Y - posMoveStart.Y));
            zoomFactor = 1;
            fCTBCode.Text = VisuGCode.TransformGCodeOffset(-(posMoveEnd.X - posMoveStart.X), -(posMoveEnd.Y - posMoveStart.Y), VisuGCode.Translate.None);
            fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
            fCTBCodeClickedLineLast = 0;
            TransformEnd();
            StatusStripClear(2);
            cmsPicBoxMoveSelectedPathInCode.Enabled = false;
            cmsPicBoxMoveSelectedPathInCode.BackColor = SystemColors.Control;
        }

        private void CmsPicBoxReverseSelectedPath_Click(object sender, EventArgs e)
        {	// picAbsPos doesn't work, because selecting entry in CMS changes coordinates
			// posMoveStart will be stored on left-click
            if (figureIsMarked)
            {
                resetView = true;
                if (Graphic.SizeOk())
                {
                    SetEditMode(false);
					VisuGCode.MarkSelectedFigure(-1);           // hide highlight
                    VisuGCode.pathBackground.Reset();
					
					if (logEnable) Logger.Trace("▌ Reverse path figureNr:{0}  figureOrder:{1}  GroupIdOrder:{2}", XmlMarker.lastFigure.FigureNr, string.Join(",", XmlMarker.GetFigureIdOrder()), string.Join(",", XmlMarker.GetGroupIdOrder()));
                    Graphic.ReDoReversePath(XmlMarker.lastFigure.FigureNr, posMoveStart, XmlMarker.GetFigureIdOrder(), XmlMarker.GetGroupIdOrder(),true);
                    SetFctbCodeText(Graphic.GCode.ToString());      // cmsPicBoxReverseSelectedPath_Click
                    NewCodeEnd();
                }
            }
            return;
        }
        private void CmsPicBoxRotateSelectedPath_Click(object sender, EventArgs e)
        {
            if (figureIsMarked)
            {
                resetView = true;
                if (Graphic.SizeOk())
                {
					SetEditMode(false);
					VisuGCode.MarkSelectedFigure(-1);           // hide highlight
					VisuGCode.pathBackground.Reset();
					
					if (logEnable) Logger.Trace("▌ Rotate path figureNr:{0}  figureOrder:{1}  GroupIdOrder:{2}", XmlMarker.lastFigure.FigureNr, string.Join(",", XmlMarker.GetFigureIdOrder()), string.Join(",", XmlMarker.GetGroupIdOrder()));
                    Graphic.ReDoReversePath(XmlMarker.lastFigure.FigureNr, posMoveStart, XmlMarker.GetFigureIdOrder(), XmlMarker.GetGroupIdOrder(), false);
                    SetFctbCodeText(Graphic.GCode.ToString());      // cmsPicBoxReverseSelectedPath_Click
                    NewCodeEnd();
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
    }
}
