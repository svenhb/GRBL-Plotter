/* MainFormPictureBox
 * Methods related to pictureBox1
 * pictureBox1_Click
 * pictureBox1_MouseMove
 * pictureBox1_Paint
 * pictureBox1_SizeChanged
 * */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GRBL_Plotter
{
    public partial class MainForm
    {

        #region pictureBox
        // onPaint drawing
        private Pen penUp = new Pen(Color.Green, 0.1F);
        private Pen penDown = new Pen(Color.Red, 0.4F);
        private Pen penHeightMap = new Pen(Color.Yellow, 1F);
        private Pen penRuler = new Pen(Color.Blue, 0.1F);
        private Pen penTool = new Pen(Color.Black, 0.5F);
        private Pen penMarker = new Pen(Color.DeepPink, 1F);
 //       SolidBrush machineLimit = new SolidBrush(Color.Red);
        private HatchBrush brushMachineLimit = new HatchBrush(HatchStyle.Horizontal, Color.Yellow);
        private double picAbsPosX = 0;
        private double picAbsPosY = 0;
        private Bitmap picBoxBackround;
        private bool showPicBoxBgImage = false;
        private bool showPathPenUp = true;

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            double minx = GCodeVisuAndTransform.drawingSize.minX;                  // extend dimensions
            double maxx = GCodeVisuAndTransform.drawingSize.maxX;
            double miny = GCodeVisuAndTransform.drawingSize.minY;
            double maxy = GCodeVisuAndTransform.drawingSize.maxY;
            double xRange = (maxx - minx);                                              // calculate new size
            double yRange = (maxy - miny);
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

                picAbsPosX = relposX * xRange + minx;
                picAbsPosY = yRange - relposY * yRange + miny;
                int offX = +5;

                if (pictureBox1.PointToClient(MousePosition).X > (pictureBox1.Width - 100))
                { offX = -75; }

                Point stringpos = new Point(pictureBox1.PointToClient(MousePosition).X + offX, pictureBox1.PointToClient(MousePosition).Y - 10);
                e.Graphics.DrawString(String.Format("Worl-Pos:\r\nX:{0,7:0.00}\r\nY:{1,7:0.00}", picAbsPosX, picAbsPosY), new Font("Lucida Console", 8), Brushes.Black, stringpos);
                e.Graphics.DrawString(String.Format("Zooming   : {0,2:0.00}%\r\nRuler Unit: {1}", 100 / zoomRange, unit), new Font("Lucida Console", 8), Brushes.Black, new Point(5, 5));

                e.Graphics.Transform = pBoxTransform;
                e.Graphics.ScaleTransform((float)picScaling, (float)-picScaling);        // apply scaling (flip Y)
                e.Graphics.TranslateTransform((float)-minx, (float)(-yRange - miny));       // apply offset
                     
                if (!showPicBoxBgImage)
                    onPaint_drawToolPath(e.Graphics);   // draw real path if background image is not shown
                e.Graphics.DrawPath(penMarker, GCodeVisuAndTransform.pathMarker);
                e.Graphics.DrawPath(penTool, GCodeVisuAndTransform.pathTool);
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
            {   e.FillPath(brushMachineLimit, GCodeVisuAndTransform.pathMachineLimit);
                e.DrawPath(penTool, GCodeVisuAndTransform.pathToolTable);
            }

                //e.DrawPath(penHeightMap, GCodeVisuAndTransform.pathMachineLimit);
            if (!Properties.Settings.Default.importUnitmm)
            {   penDown.Width = 0.01F; penUp.Width = 0.01F; penRuler.Width = 0.01F; penHeightMap.Width = 0.01F;
                penMarker.Width = 0.01F; penTool.Width = 0.01F;
            }
            e.DrawPath(penHeightMap, GCodeVisuAndTransform.pathHeightMap);
            e.DrawPath(penRuler, GCodeVisuAndTransform.pathRuler);
            e.DrawPath(penDown, GCodeVisuAndTransform.pathPenDown);
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
            if (fCTBCode.LinesCount > 2)
            {
                int line;
                line = visuGCode.setPosMarkerNearBy(picAbsPosX, picAbsPosY);
                fCTBCode.Selection = fCTBCode.GetLine(line);
                fCTBCodeClickedLineNow = line;
                fCTBCodeMarkLine();
                fCTBCode.DoCaretVisible();
            }
        }

        private Matrix pBoxTransform = new Matrix();
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
