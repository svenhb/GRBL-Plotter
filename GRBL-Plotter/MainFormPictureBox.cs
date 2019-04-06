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
 * */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

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

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            double minx = GCodeVisuAndTransform.drawingSize.minX;                  // extend dimensions
            double maxx = GCodeVisuAndTransform.drawingSize.maxX;
            double miny = GCodeVisuAndTransform.drawingSize.minY;
            double maxy = GCodeVisuAndTransform.drawingSize.maxY;
            double xRange = (maxx - minx);                                              // calculate new size
            double yRange = (maxy - miny);
            if (Properties.Settings.Default.machineLimitsFix)
            {   RectangleF tmp;
                float offset = (float)Properties.Settings.Default.machineLimitsRangeX/40;       // view size
                tmp = GCodeVisuAndTransform.pathMachineLimit.GetBounds();
                minx = (float)Properties.Settings.Default.machineLimitsHomeX - (float)grbl.posWCO.X - offset;
                miny = (float)Properties.Settings.Default.machineLimitsHomeY - (float)grbl.posWCO.Y - offset; 
                xRange = (float)Properties.Settings.Default.machineLimitsRangeX + 2*offset;
                yRange = (float)Properties.Settings.Default.machineLimitsRangeY + 2*offset;
            }

            double picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit
            string unit = (Properties.Settings.Default.importUnitmm) ? "mm" : "Inch";
            if ((picScaling > 0.001) && (picScaling < 10000))
            {
                double relposX = zoomOffsetX + zoomRange * (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).X) / pictureBox1.Width);
                double relposY = zoomOffsetY + zoomRange * (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).Y) / pictureBox1.Height);
                double ratioVisu = xRange / yRange;
                double ratioPic = Convert.ToDouble(pictureBox1.Width) / pictureBox1.Height;
                if (ratioVisu > ratioPic)
                    relposY = relposY * ratioVisu / ratioPic;
                else
                    relposX = relposX * ratioPic / ratioVisu;

                picAbsPos.X = relposX * xRange + minx;
                picAbsPos.Y = yRange - relposY * yRange + miny;
                int offX = +5;

                if (pictureBox1.PointToClient(MousePosition).X > (pictureBox1.Width - 100))
                { offX = -75; }

                Point stringpos = new Point(pictureBox1.PointToClient(MousePosition).X + offX, pictureBox1.PointToClient(MousePosition).Y - 10);

                pBoxOrig = e.Graphics.Transform;
                e.Graphics.Transform = pBoxTransform;
                e.Graphics.ScaleTransform((float)picScaling, (float)-picScaling);           // apply scaling (flip Y)
                e.Graphics.TranslateTransform((float)-minx, (float)(-yRange - miny));       // apply offset

                if (showPaths)  // only show graphics path if something is loaded
                {
                    if (!showPicBoxBgImage)
                        onPaint_drawToolPath(e.Graphics);   // draw real path if background image is not shown
                    e.Graphics.DrawPath(penMarker, GCodeVisuAndTransform.pathMarker);
                    e.Graphics.DrawPath(penTool, GCodeVisuAndTransform.pathTool);

                    e.Graphics.Transform = pBoxOrig;
                    if (Properties.Settings.Default.machineLimitsShow)
                    {
                        e.Graphics.FillRectangle(brushBackground, new Rectangle(stringpos.X, stringpos.Y - 2, 75, 34));
                        e.Graphics.FillRectangle(brushBackground, new Rectangle(18, 3, 140, 50));
                    }
                    e.Graphics.DrawString(String.Format("Work-Pos:\r\nX:{0,7:0.000}\r\nY:{1,7:0.000}", picAbsPos.X, picAbsPos.Y), new Font("Lucida Console", 8), Brushes.Black, stringpos);
                    e.Graphics.DrawString(String.Format("Zooming   : {0,2:0.00}%\r\nRuler Unit: {1}\r\nMarker-Pos:\r\n X:{2,7:0.000}\r\n Y:{3,7:0.000}", 100 / zoomRange, unit,
                        grbl.posMarker.X, grbl.posMarker.Y), new Font("Lucida Console", 7), Brushes.Black, new Point(20, 5));
                }
            }
        }

        private void onPaint_scaling(Graphics e)
        {
            double minx = GCodeVisuAndTransform.drawingSize.minX;                  // extend dimensions
            double maxx = GCodeVisuAndTransform.drawingSize.maxX;
            double miny = GCodeVisuAndTransform.drawingSize.minY;
            double maxy = GCodeVisuAndTransform.drawingSize.maxY;
            double xRange = (maxx - minx);                                              // calculate new size
            double yRange = (maxy - miny);
            double picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit
            e.ScaleTransform((float)picScaling, (float)-picScaling);        // apply scaling (flip Y)
            e.TranslateTransform((float)-minx, (float)(-yRange - miny));       // apply offset
        }
        private void onPaint_drawToolPath(Graphics e)
        {
            if (Properties.Settings.Default.machineLimitsShow)
                e.FillPath(brushMachineLimit, GCodeVisuAndTransform.pathMachineLimit); 
            if (Properties.Settings.Default.toolTableShow)
                e.DrawPath(penTool, GCodeVisuAndTransform.pathToolTable);           
            if (Properties.Settings.Default.backgroundShow)
                e.DrawPath(penLandMark, GCodeVisuAndTransform.pathBackground);

            if (!Properties.Settings.Default.importUnitmm)
            {   penDown.Width = 0.01F; penRotary.Width = 0.01F; penUp.Width = 0.01F; penRuler.Width = 0.01F; penHeightMap.Width = 0.01F;
                penMarker.Width = 0.01F; penTool.Width = 0.01F;
            }
            e.DrawPath(penHeightMap, GCodeVisuAndTransform.pathHeightMap);
            e.DrawPath(penRuler, GCodeVisuAndTransform.pathRuler);
            e.DrawPath(penDown, GCodeVisuAndTransform.pathPenDown);

            if (Properties.Settings.Default.ctrl4thUse)
                e.DrawPath(penRotary, GCodeVisuAndTransform.pathRotaryInfo);

            e.DrawPath(penHeightMap, GCodeVisuAndTransform.pathMarkSelection);

            if (showPathPenUp)
                e.DrawPath(penUp, GCodeVisuAndTransform.pathPenUp);
        }

        // Generante a background-image for pictureBox to avoid frequent drawing of pen-up/down paths
        private void onPaint_setBackground()
        {
            if (Properties.Settings.Default.guiBackgroundImageEnable)
            {
                showPicBoxBgImage = true;
                pBoxTransform.Reset(); zoomRange = 1; zoomOffsetX = 0; zoomOffsetY = 0;
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
            pictureBox1.Invalidate();
        }

        // find closest coordinate in GCode and mark
        private void pictureBox1_Click(object sender, EventArgs e)
        {   // MessageBox.Show(picAbsPosX + "  " + picAbsPosY);
            pictureBox1.Focus();
#if (debuginfo)
            log.Add("MainFormPictureBox event pictureBox1_Click start");
#endif
            if (fCTBCode.LinesCount > 2)
            {
                int line;
                line = visuGCode.setPosMarkerNearBy(picAbsPos);
//                blockFCTB_Events = true;
                fCTBCode.Selection = fCTBCode.GetLine(line);
                fCTBCodeClickedLineNow = line;
                fCTBCodeMarkLine();
                fCTBCode.DoCaretVisible();
//                fCTBCode.Focus();
//                fCTBCode.ShowFoldingLines = true;
#if (debuginfo)
                log.Add("MainFormPictureBox event pictureBox1_Click end, line: " + line.ToString());
#endif
                moveToMarkedPositionToolStripMenuItem.ToolTipText = "Work X: "+grbl.posMarker.X.ToString() + "   Y: "+ grbl.posMarker.Y.ToString();
            }
        }

        private Matrix pBoxTransform = new Matrix();
        private Matrix pBoxOrig = new Matrix();
        private static float s_dScrollValue = 2f; // zoom factor   
        private float zoomRange = 1f;
        private float zoomOffsetX = 0f;
        private float zoomOffsetY = 0f;

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();
            if (pictureBox1.Focused == true && e.Delta != 0)
            { ZoomScroll(e.Location, e.Delta > 0); }
        }
        private void ZoomScroll(Point location, bool zoomIn)
        {
            if (showPicBoxBgImage)              // don't zoom if background image is shown
                return;
            float locX = -location.X;
            float locY = -location.Y;
            float posX = (float)location.X / (float)pictureBox1.Width;      // range 0..1
            float posY = (float)location.Y / (float)pictureBox1.Height;     // range 0..1
            float valX = zoomOffsetX + posX * zoomRange;                    // range offset...(offset+range)
            float valY = zoomOffsetY + posY * zoomRange;

            pBoxTransform.Translate(-locX, -locY);
            if (zoomIn)
            {
                pBoxTransform.Scale((float)s_dScrollValue, (float)s_dScrollValue);
                zoomRange *= 1 / s_dScrollValue;
            }
            else
            {
                pBoxTransform.Scale(1 / (float)s_dScrollValue, 1 / (float)s_dScrollValue);
                zoomRange *= s_dScrollValue;
            }
            zoomOffsetX = valX - posX * zoomRange;
            zoomOffsetY = valY - posY * zoomRange;
            pBoxTransform.Translate(locX, locY);
            if (zoomRange == 1)
            { pBoxTransform.Reset(); zoomRange = 1; zoomOffsetX = 0; zoomOffsetY = 0; }

            pictureBox1.Invalidate();
        }
        private void resetZoomingToolStripMenuItem_Click(object sender, EventArgs e)
        { pBoxTransform.Reset(); zoomRange = 1; zoomOffsetX = 0; zoomOffsetY = 0; }
        private float transformMousePos(float old, float offset)
        { return old * zoomRange + offset; }
#endregion

    }
}
