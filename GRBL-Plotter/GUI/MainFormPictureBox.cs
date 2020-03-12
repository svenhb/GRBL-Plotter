/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2019 Sven Hasemann contact: svenhb@web.de

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
 * */

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
        private xyPoint zoomTranslation = new xyPoint(0, 0);
        private xyPoint zoomTranslationOld = new xyPoint(0, 0);
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
            {   RectangleF tmp;
                float offset = (float)Properties.Settings.Default.machineLimitsRangeX/40;       // view size
                tmp = VisuGCode.pathMachineLimit.GetBounds();
                minx = (float)Properties.Settings.Default.machineLimitsHomeX - (float)grbl.posWCO.X - offset;
                miny = (float)Properties.Settings.Default.machineLimitsHomeY - (float)grbl.posWCO.Y - offset; 
                xRange = (float)Properties.Settings.Default.machineLimitsRangeX + 2*offset;
                yRange = (float)Properties.Settings.Default.machineLimitsRangeY + 2*offset;
            }

            double picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit
            string unit = (Properties.Settings.Default.importUnitmm) ? "mm" : "Inch";
            if ((picScaling > 0.001) && (picScaling < 10000))
            {
    //            double relposX = zoomOffsetX +  (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).X - zoomTranslationOld.X ) / pictureBox1.Width) / zoomFactor;
    //            double relposY = zoomOffsetY +  (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).Y - zoomTranslationOld.Y ) / pictureBox1.Height) / zoomFactor;
                double relposX =  (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).X - pBoxTransform.OffsetX) / pictureBox1.Width) / zoomFactor;
                double relposY =  (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).Y - pBoxTransform.OffsetY) / pictureBox1.Height) / zoomFactor;
                double ratioVisu = xRange / yRange;
                double ratioPic = Convert.ToDouble(pictureBox1.Width) / pictureBox1.Height;
                if (ratioVisu > ratioPic)
                    relposY = relposY * ratioVisu / ratioPic;
                else
                    relposX = relposX * ratioPic / ratioVisu;

                picAbsPos.X = relposX * xRange + minx;
                picAbsPos.Y = yRange - relposY * yRange + miny;
                posMoveEnd = picAbsPos;
                int offX = +5;

                if (pictureBox1.PointToClient(MousePosition).X > (pictureBox1.Width - 100))
                { offX = -75; }

                Point stringpos = new Point(pictureBox1.PointToClient(MousePosition).X + offX, pictureBox1.PointToClient(MousePosition).Y - 10);

                pBoxOrig = e.Graphics.Transform;
                e.Graphics.Transform = pBoxTransform;
                e.Graphics.ScaleTransform((float)picScaling, (float)-picScaling);           // apply scaling (flip Y)
                e.Graphics.TranslateTransform((float)-minx, (float)(-yRange - miny));       // apply offset

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
                        e.Graphics.DrawString(String.Format("Work-Pos:\r\nX:{0,7:0.000}\r\nY:{1,7:0.000}", picAbsPos.X, picAbsPos.Y), new Font("Lucida Console", 8), Brushes.Black, stringpos);
                        if (simuEnabled)
                            e.Graphics.DrawString(String.Format("Zooming   : {0,2:0.00}%\r\nRuler Unit: {1}\r\nMarker-Pos:\r\n X:{2,7:0.000}\r\n Y:{3,7:0.000}\r\n Z:{4,7:0.000}\r\n a:{5,7:0.000}°", 100 * zoomFactor, unit,
                            grbl.posMarker.X, grbl.posMarker.Y, VisuGCode.Simulation.getZ(), 180*grbl.posMarkerAngle/Math.PI), new Font("Lucida Console", 7), Brushes.Black, new Point(20, 5));
                        else
                            e.Graphics.DrawString(String.Format("Zooming   : {0,2:0.00}%\r\nRuler Unit: {1}\r\nMarker-Pos:\r\n X:{2,7:0.000}\r\n Y:{3,7:0.000}", 100 * zoomFactor, unit,
                            grbl.posMarker.X, grbl.posMarker.Y), new Font("Lucida Console", 7), Brushes.Black, new Point(20, 5));

                        if (VisuGCode.selectedFigureInfo.Length > 0)
                            e.Graphics.DrawString(VisuGCode.selectedFigureInfo, new Font("Lucida Console", 7), Brushes.Black, new Point(150, 5));
                    }
                }
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
                    zoomTranslationOld -= (moveTranslation* zoomFactor);
                }
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            posIsMoving = false;
            allowZoom = true;
        }

        // find closest coordinate in GCode and mark
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {   // MessageBox.Show(picAbsPosX + "  " + picAbsPosY);
            pictureBox1.Focus();
            moveTranslationOld = new xyPoint(e.X, e.Y); 

            if (e.Button == MouseButtons.Left)
            {
                if ((fCTBCode.LinesCount > 2) && !posIsMoving)
                {
                    int line = VisuGCode.setPosMarkerNearBy(picAbsPos);
                    posMoveStart = picAbsPos;
                    posMoveTmp = posMoveStart;
                    posMoveEnd = posMoveStart;

                    moveToMarkedPositionToolStripMenuItem.ToolTipText = "Work X: " + grbl.posMarker.X.ToString() + "   Y: " + grbl.posMarker.Y.ToString();

                    fCTBCodeClickedLineNow = line;
                    fCTBCodeMarkLine();
                    fCTBBookmark.DoVisible();
                    findFigureMarkSelection(Color.OrangeRed);
                    posIsMoving = false;
                }
            }
            if (e.Button == MouseButtons.Middle)
            { allowZoom = false; }
        }


        // Make array of Matrix to store and reload previous view, instead of back-calculation
        private Matrix pBoxTransform = new Matrix();
        private Matrix pBoxOrig = new Matrix();			// to restore e.Graphics.Transform
        private static float scrollZoomFactor = 2f; // zoom factor   
        private float zoomFactor = 1f;
        private bool allowZoom = true;

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();
            if ((pictureBox1.Focused == true) && (e.Delta != 0) && allowZoom)
            { ZoomScroll(e.Location, e.Delta); }
        }
        private void ZoomScroll(Point location, int zoomIn)
        {
            if (showPicBoxBgImage || posIsMoving)              // don't zoom if background image is shown
                return;

            if (zoomIn > 0)
            {   if (zoomFactor < 1025)
                {
                    pBoxTransform.Reset();
                    zoomTranslation = new xyPoint(location.X, location.Y) * zoomFactor + zoomTranslationOld;// + (moveTranslation * zoomFactor);
                    pBoxTransform.Translate((float)-zoomTranslation.X, (float)-zoomTranslation.Y);
                    zoomFactor *= scrollZoomFactor;
                    pBoxTransform.Scale(zoomFactor, zoomFactor);
                }
            }
            else if (zoomIn < 0)
            {   if (zoomFactor > 0.1)
                {   pBoxTransform.Reset();
                    zoomFactor *= 1 / scrollZoomFactor;
                    zoomTranslation = new xyPoint(location.X, location.Y) * -zoomFactor + zoomTranslationOld;
                    pBoxTransform.Translate((float)-zoomTranslation.X, (float)-zoomTranslation.Y);
                    pBoxTransform.Scale(zoomFactor, zoomFactor);
                }
            }
            zoomTranslationOld = zoomTranslation;

            if (zoomFactor == 1)
            {   pBoxTransform.Reset(); zoomFactor = 1;
                zoomTranslation = new xyPoint(0, 0);
                zoomTranslationOld = new xyPoint(0, 0);
            }

            pictureBox1.Invalidate();
        }
        private void resetZoomingToolStripMenuItem_Click(object sender, EventArgs e)
        { pBoxTransform.Reset(); zoomFactor = 1; 
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

    }
}
