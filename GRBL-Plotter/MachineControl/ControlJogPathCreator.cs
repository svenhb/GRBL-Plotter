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
/*  2021-01-13 First version
 *
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace GRBL_Plotter
{
    public partial class ControlJogPathCreator : Form
    {
        private GraphicsPath grid = new GraphicsPath();
        private GraphicsPath ruler = new GraphicsPath();
        private GraphicsPath actualLine = new GraphicsPath();
        private GraphicsPath rubberBand = new GraphicsPath();
        private GraphicsPath jogPath = new GraphicsPath();
        private GraphicsPath startIcon = new GraphicsPath();
        private Pen penGrid = new Pen(Color.LightGray, 0.1F);
        private Pen penRuler = new Pen(Color.Blue, 0.1F);
        private Pen penActualLine = new Pen(Color.Red, 0.2F);
        private Pen penRubberBand = new Pen(Color.Red, 0.1F);
        private Pen penjogPath = new Pen(Color.Black, 0.2F);
        private Pen penStartIcon = new Pen(Color.Blue, 0.2F);

        private PointF posMoveStart, posMoveTmp, posMoveEnd, lastSet;
        private PointF moveTranslation = new PointF(0, 0);
        private PointF moveTranslationOld = new PointF(0, 0);
        private PointF moveTranslationStart = new PointF(0, 0);
        private PointF picAbsPos = new PointF(0, 0);

        private drawingProperties drawingSize = new drawingProperties();
        private Matrix pBoxTransform = new Matrix();
        private Matrix pBoxOrig = new Matrix();			// to restore e.Graphics.Transform
        private float scrollZoomFactor = 1.2f; // zoom factor   
        private float zoomFactor = 1f;

        private bool showDimension = false;

        private string joggcode = "";
        public string jogGCode
        { get { return joggcode; } }
        private static StringBuilder gcodeString = new StringBuilder();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ControlJogPathCreator()
        {
            InitializeComponent();
        }

        private void ControlJogPathCreator_Load(object sender, EventArgs e)
        {
            //            Logger.Trace("++++++ ControlJogPathCreator START ++++++");
            this.Icon = Properties.Resources.Icon;
            penjogPath.StartCap = LineCap.Round;
            penjogPath.EndCap = LineCap.Triangle;
            drawingSize.setX(-10, 100);
            drawingSize.setY(-10, 100);
            jogPath.StartFigure();
            jogPath.AddLine(10,10,10,30);
            jogPath.AddLine(10,30,30,30);
            jogPath.AddLine(30,30,30,10);
            jogPath.AddLine(30, 10, 10, 10);
            lastSet = new PointF(10, 10);

            VisuGCode.createRuler(ruler, drawingSize);
            createGrid(grid, drawingSize, (float)nUDRaster.Value);            
        }
        private void logDrawingSize(drawingProperties dS)
        { Logger.Trace("logDrawingSize {0} {1} {2} {3}", dS.minX, dS.minY, dS.maxX, dS.maxY); }
        private void createGridView()
        {
            drawingProperties tmp = new drawingProperties();
            PointF ul = getGraphicCoordinateFromPictureBox(new Point(0,0));
            PointF lr = getGraphicCoordinateFromPictureBox(new Point(pictureBox1.Width,pictureBox1.Height));
            tmp.setX((float)Math.Round(ul.X), (float)Math.Round(lr.X));
            tmp.setY((float)Math.Round(ul.Y), (float)Math.Round(lr.Y));
            VisuGCode.createRuler(ruler, tmp);
            createGrid(grid, tmp, (float)nUDRaster.Value);
        }
        private void createGrid(GraphicsPath path, drawingProperties dS, float raster)
        {   path.Reset();
            float minX = (float)Math.Round(dS.minX / raster) * raster;
            float minY = (float)Math.Round(dS.minY / raster) * raster;
            for (float x = minX; x < (dS.maxX); x += raster)
            {   path.StartFigure();
                path.AddLine(x, dS.minY, x, dS.maxY);
            }
            for (float y = minY; y < (dS.maxY); y += raster)
            {   path.StartFigure();
                path.AddLine(dS.minX, y, dS.maxX, y);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int offX = +5, offY = -10;
            if (pictureBox1.PointToClient(MousePosition).X > (pictureBox1.Width / 2))  { offX = -75; }
            if (pictureBox1.PointToClient(MousePosition).Y > (pictureBox1.Height / 2)) { offY = -30; }
            Point stringpos = new Point(pictureBox1.PointToClient(MousePosition).X + offX, pictureBox1.PointToClient(MousePosition).Y + offY);

            picAbsPos = getGraphicCoordinateFromPictureBox(pictureBox1.PointToClient(MousePosition), true);

            pBoxOrig = e.Graphics.Transform;
            try { e.Graphics.Transform = pBoxTransform; } catch { }
            float picScaling = Math.Min(pictureBox1.Width / drawingSize.rangeX, pictureBox1.Height / drawingSize.rangeY);               // calculate scaling px/unit
            e.Graphics.ScaleTransform(picScaling, -picScaling);           // apply scaling (flip Y)
            e.Graphics.TranslateTransform(-drawingSize.minX, (-drawingSize.rangeY- drawingSize.minY));       // apply offset
            
            e.Graphics.DrawPath(penGrid, grid);
            e.Graphics.DrawPath(penRuler, ruler);
            e.Graphics.DrawPath(penActualLine, actualLine);
            e.Graphics.DrawPath(penRubberBand, rubberBand);
            e.Graphics.DrawPath(penStartIcon, startIcon);
            e.Graphics.DrawPath(penjogPath, jogPath);

/* Show labels */            
            e.Graphics.Transform = pBoxOrig;
            if (showDimension)
                e.Graphics.DrawString(String.Format("Dimension:\r\nX:{0,7:0.000}\r\nY:{1,7:0.000}", 
                                Math.Abs(picAbsPos.X - posMoveStart.X), Math.Abs(picAbsPos.Y - posMoveStart.Y)), new Font("Lucida Console", 8), Brushes.Black, stringpos);
            else
                e.Graphics.DrawString(String.Format("Pos:\r\nX:{0,7:0.000}\r\nY:{1,7:0.000}", picAbsPos.X, picAbsPos.Y), new Font("Lucida Console", 8), Brushes.Black, stringpos);

        }
        
        private PointF getGraphicCoordinateFromPictureBox(Point location, bool snapToGrid = false)
        {   float offsetX = 0;
            float offsetY = 0;
            try { offsetX = pBoxTransform.OffsetX; offsetY = pBoxTransform.OffsetY; } catch { } // pBoxTransform.Dispose  too early
            float relposX = (location.X - offsetX)  / pictureBox1.Width   / zoomFactor;    // normalization to 1
            float relposY = (location.Y - offsetY)  / pictureBox1.Height  / zoomFactor;   // normalization to 1
            PointF tmp = new PointF(0,0);
            tmp.X = relposX * drawingSize.rangeX + drawingSize.minX;              // calc. real graphics pos
            tmp.Y = drawingSize.rangeY - relposY * drawingSize.rangeY + drawingSize.minY;    // invert Y
            if (snapToGrid && cBSnap.Checked)
            { tmp.X = snap(tmp.X, (float)nUDRaster.Value); tmp.Y = snap(tmp.Y, (float)nUDRaster.Value); }
            return tmp;
        }
        
        private float snap(float x, float raster)
        { return (float)Math.Round((x / raster)) * raster; }

        private Point snap(Point xy, int raster)
        {
            int x = (int)Math.Round((double)(xy.X / raster)) * raster;
            int y = (int)Math.Round((double)(xy.Y / raster)) * raster;
            return new Point(x, y);
        }

        private Point toPoint(PointF tmp)
        { return new Point((int)tmp.X, (int)tmp.Y); }


#region mouse action
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                //                posMoveEnd = picAbsPos;//getPicBoxOffset(e.Location, drawingSize);  //snap(e.Location, 10); ;
                posMoveEnd = getGraphicCoordinateFromPictureBox(e.Location, true);
            if (jogPath.PointCount == 0)
            {
                startIcon.AddEllipse(posMoveStart.X - 0.5F, posMoveStart.Y - 0.5F, 1, 1);
            }
            jogPath.AddLine(posMoveStart, posMoveEnd);
            lastSet = posMoveEnd;
            actualLine.Reset();
            pictureBox1.Invalidate();
            showDimension = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            posMoveTmp = getGraphicCoordinateFromPictureBox(e.Location, true);
            /* draw line */
            if (e.Button == MouseButtons.Left)
            {   posMoveEnd = posMoveTmp;
                if (jogPath.PointCount > 0)
                {
                    rubberBand.Reset();
                    rubberBand.AddLine(lastSet, posMoveStart);
                }
            }
            /* move 2D view */
            else if (e.Button == MouseButtons.Middle)
            {
                moveTranslation = new PointF(e.X, e.Y);
                moveTranslation.X = moveTranslation.X - moveTranslationOld.X ;  // calc delta move
                moveTranslation.Y = moveTranslation.Y - moveTranslationOld.Y ;  // calc delta move
                pBoxTransform.Translate(moveTranslation.X / zoomFactor, moveTranslation.Y / zoomFactor);
                moveTranslationOld = new PointF(e.X, e.Y);
            }
            /* show rubber band */
            else
            {
                if (jogPath.PointCount > 0)
                {
                    rubberBand.Reset();
                    rubberBand.AddLine(lastSet, posMoveTmp);//getPicBoxOffset(e.Location, drawingSize));
                }
            }

            actualLine.Reset();
            actualLine.AddLine(posMoveStart, posMoveEnd);

            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                posMoveStart = getGraphicCoordinateFromPictureBox(e.Location, true);    // picAbsPos;//getPicBoxOffset(e.Location, drawingSize); //snap(e.Location, 10);
                posMoveTmp = posMoveStart;
                posMoveEnd = posMoveStart;
            }
            if (e.Button == MouseButtons.Middle)
            { moveTranslationStart = e.Location; }
            showDimension = true;
        }
        
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        { rubberBand.Reset(); pictureBox1.Invalidate(); }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {   //if ((pictureBox1.Focused == true) && (e.Delta != 0))
            {   ZoomScroll(e.Location, e.Delta);    }
        }
        private void ZoomScroll(Point location, int zoomIn)
        {
            if (zoomIn != 0)
            {   if (zoomIn > 0)
                {   if (zoomFactor < 100)
                    {   zoomFactor *= scrollZoomFactor;     }
                }
                else if (zoomIn < 0)
                {   if (zoomFactor > 0.1)
                    {   zoomFactor *= 1/scrollZoomFactor;   }
                }
                PointF locationO =  getPicBoxOffset(location, drawingSize);
                pBoxTransform.Reset();
                pBoxTransform.Translate(locationO.X , locationO.Y );
                pBoxTransform.Scale(zoomFactor, zoomFactor);
                createGridView();
            }
            
            if (Math.Round(zoomFactor,2) == 1.00)
            {   pBoxTransform.Reset(); zoomFactor = 1;   }

            pictureBox1.Invalidate();
        }
        private PointF getPicBoxOffset(Point mouseLocation, drawingProperties dS)
        {   // backwards calculation to keep real coordinates on mouse-pos. on zoom-in -out
            float ratioVisu = dS.rangeX / dS.rangeY;
            float ratioPic = pictureBox1.Width / pictureBox1.Height;
            float maxposY = dS.rangeY;
            if (ratioVisu > ratioPic)
                maxposY = dS.rangeX * pictureBox1.Height / pictureBox1.Width;

            float relposX = (          picAbsPos.X - dS.minX) / dS.rangeX;
            float relposY = (maxposY - picAbsPos.Y + dS.minY) / dS.rangeY;

            if (ratioVisu > ratioPic)
                relposY = relposY * ratioPic / ratioVisu;
            else
                relposX = relposX * ratioVisu / ratioPic;
          
            PointF picOffset = new PointF();
            picOffset.X = mouseLocation.X - (relposX * zoomFactor * pictureBox1.Width);
            picOffset.Y = mouseLocation.Y - (relposY * zoomFactor * pictureBox1.Height);
            return picOffset;
        }

#endregion


#region controls
        private void btnUndo_Click(object sender, EventArgs e)
        {
            List<PointF> list = jogPath.PathData.Points.ToList<PointF>();
            list.RemoveAt(list.Count - 1);
            rubberBand.Reset();
            actualLine.Reset();
            jogPath.Reset();
            jogPath.AddLines(list.ToArray());
            lastSet = toPoint(list[list.Count - 1]);
            if (jogPath.PointCount > 0)
            { rubberBand.AddLine(lastSet, posMoveTmp); }

            posMoveStart = posMoveTmp;
            posMoveEnd = posMoveStart;
            pictureBox1.Invalidate();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            rubberBand.Reset();
            actualLine.Reset();
            jogPath.Reset();
            startIcon.Reset();
            posMoveEnd = posMoveStart = posMoveTmp = new PointF(0, 0);
            pictureBox1.Invalidate();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            createGrid(grid, drawingSize, (float)nUDRaster.Value);
            pictureBox1.Invalidate();
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            float centerX = snap(jogPath.GetBounds().Left + jogPath.GetBounds().Width / 2, 10);
            float centerY = snap(jogPath.GetBounds().Top + jogPath.GetBounds().Height / 2, 10);
            Matrix translateMatrix = new Matrix();
            translateMatrix.RotateAt(90, new PointF(centerX, centerY));
            jogPath.Transform(translateMatrix);
            actualLine.Transform(translateMatrix);
            rubberBand.Transform(translateMatrix);
            startIcon.Transform(translateMatrix);
            pictureBox1.Invalidate();
        }

        private void btnJogStart_Click(object sender, EventArgs e)
        {
            float x, y, factor = (float)nUDRaster.Value / 10;
            float lastX = 0, lastY = 0;
            int i = 0;
            joggcode = "";

            PointF[] list = jogPath.PathPoints;
            foreach (PointF pnt in list)
            {
                if (i++ == 0)
                { lastX = pnt.X * factor; lastY = pnt.Y * factor; }
                else
                {
                    x = (pnt.X * factor) - lastX;
                    y = (pnt.Y * factor) - lastY;
                    lastX = (pnt.X * factor); lastY = (pnt.Y * factor);
                    if ((x != 0) || (y != 0))
                        joggcode += String.Format("G91 X{0:0.00} Y{1:0.00} F{2};", x, -y, nUDFeedrate.Value).Replace(',', '.');
                }
            }
        }

        private void ControlJogPathCreator_SizeChanged(object sender, EventArgs e)
        {
            int hor = this.Width - 180;
            int ver = this.Height - 45;
            pictureBox1.Width = pictureBox1.Height = Math.Min(hor,ver);
            gBJogParameter.Left = this.Width - 172;
            gBPathCreator.Left = this.Width - 172;
            pictureBox1.Invalidate();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Graphic.Init(Graphic.SourceTypes.CSV, "", null, null);
            Graphic.SetHeaderInfo("From Jog path creator");
            Graphic.SetGeometry("Line");

            GraphicsPathIterator pathIterator = new GraphicsPathIterator(jogPath);
            pathIterator.Rewind();
            int pointcnt = 0;
            for (int i = 0; i < pathIterator.SubpathCount; i++)
            {
                int strIdx, endIdx;
                bool bClosedCurve;
                pathIterator.NextSubpath(out strIdx, out endIdx, out bClosedCurve);

                lastSet = jogPath.PathPoints[endIdx];

                Graphic.SetPathId(i.ToString());
                pointcnt = 0;
                for (int k = strIdx; k <= endIdx; k++)
                {
                    if (pointcnt++ == 0)
                    { Graphic.StartPath(new System.Windows.Point(jogPath.PathPoints[k].X, jogPath.PathPoints[k].Y)); }
                    else
                    { Graphic.AddLine(new System.Windows.Point(jogPath.PathPoints[k].X, jogPath.PathPoints[k].Y));      }
                }
                Graphic.StopPath();
            }
            Graphic.CreateGCode();
            joggcode = Graphic.GCode.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Application.StartupPath + datapath.jogpath;
            sfd.Filter = "Jog paths (*.xml)|*.xml";
            sfd.FileName = "JogPath.xml";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(sfd.FileName))
                    File.Delete(sfd.FileName);

                XmlWriterSettings set = new XmlWriterSettings();
                set.Indent = true;
                XmlWriter w = XmlWriter.Create(sfd.FileName, set);
                w.WriteStartDocument();
                w.WriteStartElement("view");
                w.WriteAttributeString("raster", nUDRaster.Value.ToString());
                
                GraphicsPathIterator pathIterator =  new GraphicsPathIterator(jogPath);  
                pathIterator.Rewind();                 
                // https://www.c-sharpcorner.com/UploadFile/mahesh/understanding-and-using-graphics-paths-in-gdi/
                for (int i = 0; i < pathIterator.SubpathCount; i++)  
                {  
                    int strIdx, endIdx;  
                    bool bClosedCurve;  
                    pathIterator.NextSubpath(out strIdx, out endIdx, out bClosedCurve);  
            
                    w.WriteStartElement("path");
                    w.WriteAttributeString("id", i.ToString());
                    for (int k = strIdx; k <= endIdx; k++)
                    {
                        w.WriteStartElement("pos");
                        w.WriteAttributeString("X", jogPath.PathPoints[k].X.ToString().Replace(',', '.'));
                        w.WriteAttributeString("Y", jogPath.PathPoints[k].Y.ToString().Replace(',', '.'));
                        w.WriteEndElement();
                    }
                    w.WriteEndElement();
                }  
                w.WriteEndElement();
                w.Close();
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.InitialDirectory = Application.StartupPath + datapath.jogpath;
            sfd.Filter = "Jog paths (*.xml)|*.xml";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                XmlReader reader = XmlReader.Create(sfd.FileName);
                float x1 = -1, y1 = -1, x2 = -1, y2 = -1;

                jogPath.Reset();
                rubberBand.Reset();
                actualLine.Reset();
                startIcon.Reset();
                
                while (reader.Read())
                {
                    if (!reader.IsStartElement())
                        continue;

                    switch (reader.Name)
                    {
                        case "view":
                            nUDRaster.Value = decimal.Parse(reader["raster"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                            break;
                        case "path":
                            jogPath.StartFigure();
                            break;
                        case "pos":
                            x1 = float.Parse(reader["X"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                            y1 = float.Parse(reader["Y"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                            if ((x2 >= 0) || (y2 >= 0))
                            { jogPath.AddLine(x2, y2, x1, y1); }
                            else
                            {
                                startIcon.AddEllipse(x1 - 0.5F, y1 - 0.5F, 1, 1);
                            }
                            x2 = x1; y2 = y1;
                            break;
                    }
                }
                reader.Close();
                lastSet = new PointF(x1, y1);
                
                RectangleF dim = jogPath.GetBounds();
                float maxX = dim.Left + dim.Width + 10;
                float maxY = dim.Top + dim.Height + 10;
                float a = Math.Max(maxX,maxY);
                drawingSize.setX(-10, a);
                drawingSize.setY(-10, a);
                VisuGCode.createRuler(ruler, drawingSize);
                createGrid(grid, drawingSize, (float)nUDRaster.Value);            

                pBoxTransform.Reset(); 
                zoomFactor = 1;  

                pictureBox1.Invalidate();
            }
        }
        
  #endregion      
        
    }
}
