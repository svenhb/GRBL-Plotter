/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2020 Sven Hasemann contact: svenhb@web.de

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
 * 
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using FastColoredTextBoxNS;
using System.Collections.Generic;

namespace GRBL_Plotter
{
    public partial class MainForm : Form
    {
        #region pictureBox
        // onPaint drawing
        private Pen penUp = new Pen(Color.Green, 0.1F);
        private Pen penDown = new Pen(Color.Red, 0.4F);
        private Pen penRotary = new Pen(Color.White, 0.4F);
        private Pen penHeightMap = new Pen(Color.Yellow, 1F);
        private Pen penRuler = new Pen(Color.Blue, 0.1F);
        private Pen penTool = new Pen(Color.Black, 0.5F);
        private Pen penMarker = new Pen(Color.DeepPink, 1F);
        private Pen penLandMark = new Pen(Color.DarkGray, 1F);
        //       SolidBrush machineLimit = new SolidBrush(Color.Red);
        private HatchBrush brushMachineLimit = new HatchBrush(HatchStyle.Horizontal, Color.Yellow);
        private SolidBrush brushBackground = new SolidBrush(Color.White);
        private xyPoint picAbsPos = new xyPoint(0, 0);
        private Bitmap picBoxBackround;
        private bool showPicBoxBgImage = false;
        private bool showPathPenUp = true;
        private bool showPaths = false;
        private xyPoint posMoveStart = new xyPoint(0, 0);
        private xyPoint posMoveTmp = new xyPoint(0, 0);
        private xyPoint posMoveEnd = new xyPoint(0, 0);
        private xyPoint moveTranslation = new xyPoint(0, 0);
        private xyPoint moveTranslationOld = new xyPoint(0, 0);
        private bool posIsMoving = false;

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            double minx = VisuGCode.drawingSize.minX;                  // extend dimensions
            double maxx = VisuGCode.drawingSize.maxX;
            double miny = VisuGCode.drawingSize.minY;
            double maxy = VisuGCode.drawingSize.maxY;
            double xRange = (maxx - minx);                                              // calculate new size
            double yRange = (maxy - miny);
            if (Properties.Settings.Default.machineLimitsFix)
            {   double offset = (double)Properties.Settings.Default.machineLimitsRangeX/40;       // view size
                minx = (double)Properties.Settings.Default.machineLimitsHomeX - grbl.posWCO.X - offset;
                miny = (double)Properties.Settings.Default.machineLimitsHomeY - grbl.posWCO.Y - offset; 
                xRange = (double)Properties.Settings.Default.machineLimitsRangeX + 2*offset;
                yRange = (double)Properties.Settings.Default.machineLimitsRangeY + 2*offset;
            }

            double picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit
            if ((picScaling > 0.001) && (picScaling < 10000))
            {
                double offsetX = 0;// pBoxTransform.OffsetX;
                double offsetY = 0;// pBoxTransform.OffsetY;
                try { offsetX = pBoxTransform.OffsetX; offsetY = pBoxTransform.OffsetY; } catch { } // pBoxTransform.Dispose  too early
                double relposX = (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).X) - offsetX) / Convert.ToDouble(pictureBox1.Width) / Convert.ToDouble(zoomFactor);
                double relposY = (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).Y) - offsetY) / Convert.ToDouble(pictureBox1.Height) / Convert.ToDouble(zoomFactor);
                double ratioVisu = xRange / yRange;
                double ratioPic = Convert.ToDouble(pictureBox1.Width) / pictureBox1.Height;
                if (ratioVisu > ratioPic)
                    relposY = relposY * ratioVisu / ratioPic;
                else
                    relposX = relposX * ratioPic / ratioVisu;

                picAbsPos.X = relposX * xRange + minx;
                picAbsPos.Y = yRange - relposY * yRange + miny;

                if (posIsMoving)
                    posMoveEnd = picAbsPos;

                int offX = +5;

                if (pictureBox1.PointToClient(MousePosition).X > (pictureBox1.Width - 100))
                { offX = -75; }

                Point stringpos = new Point(pictureBox1.PointToClient(MousePosition).X + offX, pictureBox1.PointToClient(MousePosition).Y - 10);

                pBoxOrig = e.Graphics.Transform;
                try { e.Graphics.Transform = pBoxTransform; } catch { }
                e.Graphics.ScaleTransform((float)picScaling, (float)-picScaling);           // apply scaling (flip Y)
                e.Graphics.TranslateTransform((float)-minx, (float)(-yRange - miny));       // apply offset

                try
                {
                    if (showPaths)          // only show graphics path if something is loaded
                    {
                        if (!showPicBoxBgImage)
                            onPaint_drawToolPath(e.Graphics);   // draw real path if background image is not shown
                        e.Graphics.DrawPath(penTool, VisuGCode.pathTool);
                        e.Graphics.DrawPath(penMarker, VisuGCode.pathMarker);

                        e.Graphics.Transform = pBoxOrig;
                        if (Properties.Settings.Default.machineLimitsShow)
                        {
                            e.Graphics.FillRectangle(brushBackground, new Rectangle(stringpos.X, stringpos.Y - 2, 75, 34));
                            e.Graphics.FillRectangle(brushBackground, new Rectangle(18, 3, 140, 50));
                        }
                        if (Properties.Settings.Default.gui2DInfoShow)
                        {
                            string unit = (Properties.Settings.Default.importUnitmm) ? "mm" : "Inch";
                            e.Graphics.DrawString(String.Format("Work-Pos:\r\nX:{0,7:0.000}\r\nY:{1,7:0.000}", picAbsPos.X, picAbsPos.Y), new Font("Lucida Console", 8), Brushes.Black, stringpos);
                            if (simuEnabled)
                                e.Graphics.DrawString(String.Format("Zooming   : {0,2:0.00}%\r\nRuler Unit: {1}\r\nMarker-Pos:\r\n X:{2,7:0.000}\r\n Y:{3,7:0.000}\r\n Z:{4,7:0.000}\r\n a:{5,7:0.000}°", 100 * zoomFactor, unit,
                                grbl.posMarker.X, grbl.posMarker.Y, VisuGCode.Simulation.getZ(), 180 * grbl.posMarkerAngle / Math.PI), new Font("Lucida Console", 7), Brushes.Black, new Point(20, 5));
                            else
                                e.Graphics.DrawString(String.Format("Zooming   : {0,2:0.00}%\r\nRuler Unit: {1}\r\nMarker-Pos:\r\n X:{2,7:0.000}\r\n Y:{3,7:0.000}", 100 * zoomFactor, unit,
                                grbl.posMarker.X, grbl.posMarker.Y), new Font("Lucida Console", 7), Brushes.Black, new Point(20, 5));

                            if (VisuGCode.selectedFigureInfo.Length > 0)
                                e.Graphics.DrawString(VisuGCode.selectedFigureInfo, new Font("Lucida Console", 7), Brushes.Black, new Point(150, 5));
                        }
                    }
                }
                catch { }
            }
        }

        private void onPaint_scaling(Graphics e)
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
        private void onPaint_drawToolPath(Graphics e)
        {
            try
            {
                if (Properties.Settings.Default.machineLimitsShow)
                    e.FillPath(brushMachineLimit, VisuGCode.pathMachineLimit);
                if (Properties.Settings.Default.gui2DToolTableShow)
                    e.DrawPath(penTool, VisuGCode.pathToolTable);
                if (Properties.Settings.Default.guiBackgroundShow)
                    e.DrawPath(penLandMark, VisuGCode.pathBackground);
                if (Properties.Settings.Default.guiDimensionShow)
                    e.DrawPath(penLandMark, VisuGCode.pathDimension);

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

                e.DrawPath(penHeightMap, VisuGCode.pathHeightMap);

                if (Properties.Settings.Default.gui2DRulerShow)
                    e.DrawPath(penRuler, VisuGCode.pathRuler);

                e.DrawPath(penDown, VisuGCode.pathPenDown);

                if (Properties.Settings.Default.ctrl4thUse)
                    e.DrawPath(penRotary, VisuGCode.pathRotaryInfo);

                if (posIsMoving)
                    e.DrawPath(penLandMark, VisuGCode.pathMarkSelection);
                else
                    e.DrawPath(penHeightMap, VisuGCode.pathMarkSelection);

                if (!(showPathPenUp ^ Properties.Settings.Default.gui2DPenUpShow))
                    e.DrawPath(penUp, VisuGCode.pathPenUp);
            }
            catch { }
        }

        // Generante a background-image for pictureBox to avoid frequent drawing of pen-up/down paths
        private void onPaint_setBackground()
        {
            if (Properties.Settings.Default.guiBackgroundImageEnable)
            {
                showPicBoxBgImage = true;
                pBoxTransform.Reset(); zoomFactor = 1; //zoomOffsetX = 0; zoomOffsetY = 0;
                pictureBox1.BackgroundImageLayout = ImageLayout.None;
                picBoxBackround = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                Graphics graphics = Graphics.FromImage(picBoxBackround);
                graphics.DrawString("Streaming, zooming disabled", new Font("Lucida Console", 8), Brushes.Black, 1, 1);
                onPaint_scaling(graphics);
                onPaint_drawToolPath(graphics);     // draw path
                pictureBox1.BackgroundImage = new Bitmap(picBoxBackround);//Properties.Resources.modell;
            }
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (showPicBoxBgImage)
                onPaint_setBackground();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                xyPoint diff = new xyPoint(0, 0);
                diff = posMoveEnd - posMoveTmp;
                posMoveTmp = posMoveEnd;
                if (VisuGCode.pathMarkSelection.PointCount > 1)     // move selected path
                {
                    Matrix tmp = new Matrix();
                    tmp.Translate((float)diff.X, (float)diff.Y);
                    VisuGCode.pathMarkSelection.Transform(tmp);
                    posIsMoving = true;
                }
                else
                {   posIsMoving = false;                            // move view
                    moveTranslation = new xyPoint(e.X, e.Y)  - moveTranslationOld;  // calc delta move
                    pBoxTransform.Translate((float)moveTranslation.X / zoomFactor, (float)moveTranslation.Y / zoomFactor);
                    moveTranslationOld = new xyPoint(e.X, e.Y);
                }
                if ((diff.X!=0) || (diff.Y!=0))
                    previousClick = 0;  // no doubleclick

                statusStripSet(2,string.Format("{0} X:{1:0.00}  Y:{2:0.00}", Localization.getString("statusStripeSelectionMoved"), (posMoveEnd.X - posMoveStart.X), (posMoveEnd.Y - posMoveStart.Y)),Color.Yellow);
            }
            pictureBox1.Invalidate();
        }

        private static MouseButtons _lastButtonUp = MouseButtons.None;
        private static int previousClick = SystemInformation.DoubleClickTime;
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (posIsMoving)
            {
                cmsPicBoxMoveSelectedPathInCode.Enabled = true;
                cmsPicBoxMoveSelectedPathInCode.BackColor = Color.Yellow;
                statusStripSet(2,string.Format("{0} X:{1:0.00}  Y:{2:0.00} - click [{3}] to apply", Localization.getString("statusStripeSelectionMoved"), (posMoveEnd.X - posMoveStart.X), (posMoveEnd.Y - posMoveStart.Y), cmsPicBoxMoveSelectedPathInCode.Text), Color.Yellow);
            }
            posIsMoving = false;
            allowZoom = true;
            _lastButtonUp = e.Button;

            int now = System.Environment.TickCount;
            int diff = now - previousClick;
 //           lbInfo.Text = SystemInformation.DoubleClickTime.ToString() + "   " + diff.ToString();
            if ((diff <= SystemInformation.DoubleClickTime) && (diff > 100))
            {   pictureBox1_DoubleClick(sender, e);   }
            previousClick = now;
        }

        // find closest coordinate in GCode and mark
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {   pictureBox1.Focus();
            moveTranslationOld = new xyPoint(e.X, e.Y);
            allowZoom = false;
            if (e.Button == MouseButtons.Left)
            {
                if ((fCTBCode.LinesCount > 2) && !posIsMoving)
                {
                    int line = 0;
                    posMoveStart = picAbsPos;
                    posMoveTmp = posMoveStart;
                    posMoveEnd = posMoveStart;

                    if (manualEdit)
                        newCodeEnd();
                    if (Panel.ModifierKeys == Keys.Alt)
                    {
                        line = VisuGCode.setPosMarkerNearBy(picAbsPos,false);         // find line with coord nearby, mark / unmark figure
                        fCTBCodeClickedLineNow = line;
                        findFigureMarkSelection(xmlMarkerType.Line, line);
                    }
                    else if (Panel.ModifierKeys == Keys.Control) //Keys.Alt)
                    {
                        line = VisuGCode.setPosMarkerNearBy(picAbsPos,false);         // find line with coord nearby, mark / unmark figure
                        fCTBCodeClickedLineNow = line;
                        findFigureMarkSelection(xmlMarkerType.Group, line);
                    }
                    else
                    {
                        line = VisuGCode.setPosMarkerNearBy(picAbsPos);         // find line with coord nearby, mark / unmark figure
                        fCTBCodeClickedLineNow = line;
                        findFigureMarkSelection(xmlMarkerType.Figure, line);
                    }

                    cmsPicBoxMoveToMarkedPosition.ToolTipText = "Work X: " + grbl.posMarker.X.ToString() + "   Y: " + grbl.posMarker.Y.ToString();
                    enableBlockCommands(VisuGCode.getHighlightStatus() > 0 );
                    if (VisuGCode.codeBlocksAvailable())
                        statusStripSet(1, Localization.getString("statusStripeClickKeys2"),Color.LightGreen);
                    posIsMoving = false;
                }
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (_lastButtonUp == MouseButtons.Middle)
            {   pBoxTransform.Reset(); zoomFactor = 1;
                pictureBox1.Invalidate();
            }
        }


        // Make array of Matrix to store and reload previous view, instead of back-calculation
        private Matrix pBoxTransform = new Matrix();
        private Matrix pBoxOrig = new Matrix();			// to restore e.Graphics.Transform
        private static float scrollZoomFactor = 1.2f; // zoom factor   
        private float zoomFactor = 1f;
        private bool allowZoom = true;

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {   pictureBox1.Focus();
            if ((pictureBox1.Focused == true) && (e.Delta != 0) && allowZoom)
            {   ZoomScroll(e.Location, e.Delta);    }
        }

        private xyPoint getPicBoxOffset(Point mouseLocation)
        {   // backwards calculation to keep real coordinates on mouse-pos. on zoom-in -out
            double minx = VisuGCode.drawingSize.minX;
            double miny = VisuGCode.drawingSize.minY;
            double xRange = (VisuGCode.drawingSize.maxX - VisuGCode.drawingSize.minX);                                              // calculate new size
            double yRange = (VisuGCode.drawingSize.maxY - VisuGCode.drawingSize.minY);

            if (Properties.Settings.Default.machineLimitsFix)
            {   double offset = (double)Properties.Settings.Default.machineLimitsRangeX / 40;       // view size
                minx = (double)Properties.Settings.Default.machineLimitsHomeX - grbl.posWCO.X - offset;
                miny = (double)Properties.Settings.Default.machineLimitsHomeY - grbl.posWCO.Y - offset;
                xRange = (double)Properties.Settings.Default.machineLimitsRangeX + 2 * offset;
                yRange = (double)Properties.Settings.Default.machineLimitsRangeY + 2 * offset;
            }

            double ratioVisu = xRange / yRange;
            double ratioPic = Convert.ToDouble(pictureBox1.Width) / pictureBox1.Height;
            double relposX = (         picAbsPos.X - minx) / xRange;
            double relposY = (yRange - picAbsPos.Y + miny) / yRange;

            if (ratioVisu > ratioPic)
                relposY = relposY * ratioPic / ratioVisu;
            else
                relposX = relposX * ratioVisu / ratioPic;
          
            xyPoint picOffset = new xyPoint();
            picOffset.X = mouseLocation.X - (relposX * zoomFactor * pictureBox1.Width);
            picOffset.Y = mouseLocation.Y - (relposY * zoomFactor * pictureBox1.Height);
            return picOffset;
        }

        private void ZoomScroll(Point location, int zoomIn)
        {
            if (showPicBoxBgImage || posIsMoving)              // don't zoom if background image is shown
                return;

            if (zoomIn > 0)
            {   if (zoomFactor < 200)
                {
                    zoomFactor *= scrollZoomFactor;
                    xyPoint locationO = getPicBoxOffset(location);
                    pBoxTransform.Reset();
                    pBoxTransform.Translate((float)locationO.X , (float)locationO.Y );
                    pBoxTransform.Scale(zoomFactor, zoomFactor);
                }
            }
            else if (zoomIn < 0)
            {   if (zoomFactor > 0.4)
                {
                    zoomFactor *= 1/scrollZoomFactor;
                    xyPoint locationO = getPicBoxOffset(location);
                    pBoxTransform.Reset();
                    pBoxTransform.Translate((float)locationO.X, (float)locationO.Y);
                    pBoxTransform.Scale(zoomFactor, zoomFactor);
                }
            }
            if (Math.Round(zoomFactor,2) == 1.00)
            {   pBoxTransform.Reset(); zoomFactor = 1;
            }

            pictureBox1.Invalidate();
        }

        private void setGraphicProperties()
        {
            pictureBox1.BackColor = Properties.Settings.Default.gui2DColorBackground;
            penUp.Color = Properties.Settings.Default.gui2DColorPenUp;
            penDown.Color = Properties.Settings.Default.gui2DColorPenDown;
            penRotary.Color = Properties.Settings.Default.gui2DColorRotaryInfo;
            penHeightMap.Color = Properties.Settings.Default.gui2DColorHeightMap;
            penRuler.Color = Properties.Settings.Default.gui2DColorRuler;
            penTool.Color = Properties.Settings.Default.gui2DColorTool;
            penMarker.Color = Properties.Settings.Default.gui2DColorMarker;

            float factorWidth = 1;
            if (!Properties.Settings.Default.importUnitmm) factorWidth = 0.0393701f;
            if (Properties.Settings.Default.gui2DKeepPenWidth) factorWidth /= zoomFactor;
            penHeightMap.Width = (float)Properties.Settings.Default.gui2DWidthHeightMap * factorWidth;
            penHeightMap.LineJoin = LineJoin.Round;
            penRuler.Width = (float)Properties.Settings.Default.gui2DWidthRuler * factorWidth;
            penUp.Width = (float)Properties.Settings.Default.gui2DWidthPenUp * factorWidth;
            penUp.LineJoin = LineJoin.Round;
            penDown.Width = (float)Properties.Settings.Default.gui2DWidthPenDown * factorWidth;
            penDown.LineJoin = LineJoin.Round;
            penRotary.Width = (float)Properties.Settings.Default.gui2DWidthRotaryInfo * factorWidth;
            penRotary.LineJoin = LineJoin.Round;
            penTool.Width = (float)Properties.Settings.Default.gui2DWidthTool * factorWidth;
            penTool.LineJoin = LineJoin.Round;
            penMarker.Width = (float)Properties.Settings.Default.gui2DWidthMarker * factorWidth;
            penMarker.LineJoin = LineJoin.Round;
            penLandMark.LineJoin = LineJoin.Round;
            penLandMark.Width = 2 * (float)Properties.Settings.Default.gui2DWidthPenDown * factorWidth;

            brushMachineLimit = new HatchBrush(HatchStyle.DiagonalCross, Properties.Settings.Default.gui2DColorMachineLimit, Color.Transparent);
            picBoxBackround = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }
        #endregion

        #region cmsPictureBox events
        private void cmsPicBoxEnable(bool enable = true)
        {   cmsPicBoxMoveToMarkedPosition.Enabled = enable;
            cmsPicBoxZeroXYAtMarkedPosition.Enabled = enable;
            cmsPicBoxMoveGraphicsOrigin.Enabled = enable;
            cmsPicBoxResetZooming.Enabled = enable;
            cmsPicBoxSetGCodeAsBackground.Enabled = enable;
            cmsPicBoxClearBackground.Enabled = enable;
        }

        private void cmsPicBoxMoveToMarkedPosition_Click(object sender, EventArgs e)
        {   sendCommand(String.Format("G0 X{0} Y{1}", gcode.frmtNum(grbl.posMarker.X), gcode.frmtNum(grbl.posMarker.Y)).Replace(',', '.'));
        }

        private void cmsPicBoxZeroXYAtMarkedPosition_Click(object sender, EventArgs e)
        {   xyPoint tmp = (xyPoint)(grbl.posWork) - grbl.posMarker;
            sendCommand(String.Format(zeroCmd + " X{0} Y{1}", gcode.frmtNum(tmp.X), gcode.frmtNum(tmp.Y)).Replace(',', '.'));
        //    grbl.posMarker = new xyPoint(0, 0);
        }
        private void cmsPicBoxMoveGraphicsOrigin_Click(object sender, EventArgs e)
        {   unDo.setCode(fCTBCode.Text, cmsPicBoxMoveGraphicsOrigin.Text, this);
            clearTextSelection(fCTBCodeClickedLineNow);
            VisuGCode.markSelectedFigure(-1);
            fCTBCode.Text = VisuGCode.transformGCodeOffset(grbl.posMarker.X, grbl.posMarker.Y, VisuGCode.translate.None);
            transformEnd();
            grbl.posMarker = new xyPoint(0, 0);
        }

        private void cmsPicBoxResetZooming_Click(object sender, EventArgs e)
        {   pBoxTransform.Reset(); zoomFactor = 1;    }

        private void cmsPicBoxPasteFromClipboard_Click(object sender, EventArgs e)
        {   unDo.setCode(fCTBCode.Text, cmsPicBoxPasteFromClipboard.Text, this);
            loadFromClipboard();
            enableCmsCodeBlocks(VisuGCode.codeBlocksAvailable());
        }

        private void cmsPicBoxReloadFile_Click(object sender, EventArgs e)
        {   reStartConvertFile(sender, e);   }

        private void cmsPicBoxMoveToFirstPos_Click(object sender, EventArgs e)
        {
            //      int start = findFigureMarkSelection(xmlMarkerType.Figure, 0);
            //     fCTBCodeClickedLineNow = start;
            fCTBCodeMarkLine();
            fCTBCode.DoCaretVisible();
            return;
        }

        private void cmsPicBoxDeletePath_Click(object sender, EventArgs e)
        {   unDo.setCode(fCTBCode.Text, cmsPicBoxDeletePath.Text, this);
            if (figureIsMarked)
            {   resetView = true;
                fCTBCode.InsertText("( Figure removed )\r\n");
                transformEnd();
            }
            return;
        }

        private void cmsPicBoxCropSelectedPath_Click(object sender, EventArgs e)
        {
            if (manualEdit)
                return;
            transformStart(cmsPicBoxCropSelectedPath.Text, false);

            if ((xmlMarker.GetGroupCount() > 0) && (markedBlockType == xmlMarkerType.Group))
            {   if (xmlMarker.GetGroup(fCTBCodeClickedLineNow, 10))    // get range group+1-start to group+n-end
                {   setTextSelection(xmlMarker.lastGroup.lineStart, xmlMarker.lastGroup.lineEnd);
                    fCTBCode.SelectedText = "";
                }
                if (xmlMarker.GetGroup(fCTBCodeClickedLineNow, -10))    // get range group0-start to group-1-end
                {   setTextSelection(xmlMarker.lastGroup.lineStart, xmlMarker.lastGroup.lineEnd);
                    fCTBCode.SelectedText = "";
                }
            }
            else if ((xmlMarker.GetFigureCount() > 0) && (markedBlockType == xmlMarkerType.Figure))
            {
                if (xmlMarker.GetFigure(fCTBCodeClickedLineNow, 10))    // get range group+1-start to group+n-end
                {   setTextSelection(xmlMarker.lastGroup.lineStart, xmlMarker.lastGroup.lineEnd);
                    fCTBCode.SelectedText = "";
                }
                if (xmlMarker.GetFigure(fCTBCodeClickedLineNow, -10))    // get range group0-start to group-1-end
                {   setTextSelection(xmlMarker.lastGroup.lineStart, xmlMarker.lastGroup.lineEnd);
                    fCTBCode.SelectedText = "";
                }
            }
            else
                fCTBCode.Text = VisuGCode.cutOutFigure();
            transformEnd();
        }

        private void cmsPicBoxMoveSelectedPathInCode_Click(object sender, EventArgs e)
        {
            transformStart(cmsPicBoxMoveSelectedPathInCode.Text);
            zoomFactor = 1;
            fCTBCode.Text = VisuGCode.transformGCodeOffset(-(posMoveEnd.X - posMoveStart.X), -(posMoveEnd.Y - posMoveStart.Y), VisuGCode.translate.None);
            fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
            fCTBCodeClickedLineLast = 0;
            transformEnd();
            statusStripClear(2);
            cmsPicBoxMoveSelectedPathInCode.Enabled = false;
            cmsPicBoxMoveSelectedPathInCode.BackColor = SystemColors.Control;
        }

        private void cmsPicBoxSetGCodeAsBackground_Click(object sender, EventArgs e)
        {   VisuGCode.setPathAsLandMark();
            Properties.Settings.Default.guiBackgroundShow = toolStripViewBackground.Checked = true;
        }

        private void cmsPicBoxClearBackground_Click(object sender, EventArgs e)
        { VisuGCode.setPathAsLandMark(true); }

        #endregion


    }
}
