/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2016 Sven Hasemann contact: svenhb@web.de

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
/*  GCodeVisuAndTransform.cs
    Scaling, Rotation, Remove OffsetXY, Mirror X or Y
    During transformation the drawing path will be generated, because cooridantes are already parsed.
    Return transformed GCode 
*/
/* 2016-09-18 use gcode.frmtNum to control amount of decimal places
 * 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace GRBL_Plotter
{
    public class GCodeVisuAndTransform
    {   public enum translate{None, Offset0,MirrorX, MirrorY };
        public Dimensions xyzSize = new Dimensions();
        public static drawingProperties drawingSize = new drawingProperties();
        public static GraphicsPath pathPenUp = new GraphicsPath();
        public static GraphicsPath pathPenDown = new GraphicsPath();
        public static GraphicsPath pathRuler = new GraphicsPath();
        public static GraphicsPath pathTool = new GraphicsPath();
        public static GraphicsPath pathMarker = new GraphicsPath();
        public static GraphicsPath path= pathPenUp;

        private double toolPosX = 0;
        private double toolPosY = 0;
        private double markerPosX = 0;
        private double markerPosY = 0;
        private double zeroOffsetX = 0;
        private double zeroOffsetY = 0;
        private int picWidth = 640;
        private int picHeight = 480;
        private double picScaling = 1;
        private double zLimit = 0.0;
        public void setPosTool(double x, double y)
        { toolPosX = x; toolPosY = y; }
        public void setPosMarker(double x, double y)
        { markerPosX = x; markerPosY = y; }
        public void setPosMarkerX(double x)
        { markerPosX = x; }
        public double GetPosMarkerX()
        { return markerPosX; }
        public void setPosMarkerY(double y)
        { markerPosY = y; }
        public double GetPosMarkerY()
        { return markerPosY; }

        public GCodeVisuAndTransform(double x, double y)
        {   xyzSize.Reset();
            xyzSize.SetXYZ(0, 0, 0);
            xyzSize.SetXYZ(x, y, 1);
        }
        public void clear()
        {   xyzSize.Reset();
            pathPenUp.Reset();
            pathPenDown.Reset();
            pathRuler.Reset();
            pathTool.Reset();
            pathMarker.Reset();
            old_x = 0; old_y = 0; old_z = 0;
        }

        private double? old_x = null;
        private double? old_y = null;
        private double? old_z = null;
        private int cmdnr = 0;
        public void addGCode(string cmdstr, double? nx, double? ny, double? nz, double i, double j)
        {
            bool validx = false, validy = false, validz = false;
            int indexG = cmdstr.IndexOf('G');
            cmdnr = gcode.getIntGCode('G',cmdstr);
            if (cmdnr < 0) cmdnr = 1;

            xyzSize.SetXYZ(nx, ny, nz);            // calculate max dimensions

            double x=0, y=0, z=0;
            if (nx != null)
            {   x = (double)nx; validx = true;
                if (old_x == null) { old_x = x; }
            }
            else
            { x = (double)old_x; }

            if (ny != null)
            {   y = (double)ny; validy = true;
                if (old_y == null) { old_y = y; }
            }
            else
            { y = (double)old_y; }

            if (nz != null)
            {   z = (double)nz; validz = true;
                if (old_z == null) { old_z = z; }
            }

            if (validz)
            {   if (z <= zLimit)                    // select path to draw depending on Z-Value
                { path = pathPenDown; }
                else
                { path = pathPenUp; }
            }
            path.StartFigure();                 // Start Figure but never close to avoid connecting first and last point
            if (cmdnr == 0 || cmdnr == 1)
            {
                path.AddLine((float)old_x, (float)old_y, (float)x, (float)y);
            }
            if (cmdnr == 2 || cmdnr == 3)
            {
                float radius = (float)Math.Sqrt(i * i + j * j);
                float x1 = (float)(old_x + i - radius);
                float y1 = (float)(old_y + j - radius);

                float cos1 = (float)i / radius;
                if (cos1 > 1) cos1 = 1;
                if (cos1 < -1) cos1 = -1;
                float a1 = 180-180*(float)(Math.Acos(cos1)/Math.PI);

                if (j > 0) { a1 = -a1; }
                float cos2 = (float)(old_x + i - x) / radius;
                if (cos2 > 1) cos2 = 1;
                if (cos2 < -1) cos2 = -1;
                float a2 = 180-180*(float)(Math.Acos(cos2)/Math.PI);

    //            debugString += String.Format("Arc a1 {0}={1}  a2 {2}={3} \r\n", (cos1),a1,(cos2),a2);
                if ((old_y+j-y) > 0) { a2 = -a2; }
                float da = -(360 + a1 - a2);
                if (cmdnr == 3) { da=-(360 + a2 - a1); }
                if (da > 360) { da -= 360; }
                if (da < -360) { da += 360; }
                if (cmdnr == 2)
                {   path.AddArc(x1, y1, 2 * radius, 2 * radius, a1, da);
                    xyzSize.SetCircle(x1 + radius, y1 + radius, radius, a1, da);        // calculate new dimensions
                }
                else
                {   path.AddArc(x1, y1, 2 * radius, 2 * radius, a1, -da);
                    xyzSize.SetCircle(x1 + radius, y1 + radius, radius, a1, -da);       // calculate new dimensions
                }
    //            debugString += String.Format("Arc {0} {1} {2} {3} ({4} {5} {6})\r\n", x, y, i, j,a1,a2,da);
            }
            if (validx) { old_x = x; }
            if (validy) { old_y = y; }
            if (validz) { old_z = z; }
        }
 
        public void createImagePath()
        {   double extend = 1.01;                                                       // extend dimension a little bit
            double roundTo = 5;                                                         // round-up dimensions
            drawingSize.minX = Math.Floor(xyzSize.minx* extend / roundTo)* roundTo;                  // extend dimensions
            if (drawingSize.minX >= 0) { drawingSize.minX = -roundTo; }                                          // be sure to show 0;0 position
            drawingSize.maxX = Math.Ceiling(xyzSize.maxx* extend / roundTo) * roundTo;
            drawingSize.minY = Math.Floor(xyzSize.miny* extend / roundTo) * roundTo;
            if (drawingSize.minY >= 0) { drawingSize.minY = -roundTo; }
            drawingSize.maxY = Math.Ceiling(xyzSize.maxy* extend / roundTo) * roundTo;
            double xRange = (drawingSize.maxX - drawingSize.minX);                                              // calculate new size
            double yRange = (drawingSize.maxY - drawingSize.minY);
            zeroOffsetX = drawingSize.minX;
            zeroOffsetY = drawingSize.minY;
            picScaling = Math.Min(picWidth/(xRange), picHeight/(yRange));               // calculate scaling px/unit
            CreateRuler(drawingSize.maxX, drawingSize.maxY);
            createMarkerPath();
        }

        public void createMarkerPath()
        {
            float msize = (float) Math.Max(xyzSize.dimx, xyzSize.dimy) / 50; 
            CreateMarker(pathTool, (float)toolPosX, (float)toolPosY, msize, 2);
            CreateMarker(pathMarker, (float)markerPosX, (float)markerPosY, msize, 3);
        }
        private void CreateRuler(double maxX, double maxY)
        {
            for (int i = 0; i < maxX; i++)          // horizontal ruler
            {   pathRuler.StartFigure();
                if (i % 5 == 0)
                {   if (i % 100 == 0)
                    { pathRuler.AddLine((float)i, 0, (float)i, -5F); }  // 100                    
                    else if ((i % 10 == 0) && (maxX < 1000))
                    { pathRuler.AddLine((float)i, 0, (float)i, -3F); }  // 10                  
                    else if (maxX < 500)
                    { pathRuler.AddLine((float)i, 0, (float)i, -2F); }  // 5
                    }
                    else if (maxX < 200)
                    { pathRuler.AddLine((float)i, 0, (float)i, -1F); }  // 1
            }
            for (int i = 0; i < maxY; i++)          // vertical ruler
            {   pathRuler.StartFigure();
                if (i % 5 == 0)
                {   if (i % 100 == 0)
                    { pathRuler.AddLine(0, (float)i, -5F, (float)i); } // 100                   
                    else if ((i % 10 == 0) && (maxY < 1000))
                    { pathRuler.AddLine(0, (float)i, -3F, (float)i); } // 10           
                    else if (maxY < 500)
                    { pathRuler.AddLine(0, (float)i, -2F, (float)i); } // 5
                }
                else if (maxY < 200)
                { pathRuler.AddLine(0, (float)i, -1F, (float)i); }     // 1
            }
        }

        private void CreateMarker(GraphicsPath path, float centerX,float centerY, float dimension,int style)
        {   if (dimension == 0) { return; }
            path.Reset();
            if (style == 0)   // horizontal cross
            {
                path.AddLine(centerX , centerY + dimension, centerX , centerY - dimension);
                path.AddLine(centerX + dimension, centerY , centerX - dimension, centerY );
            }
            if (style == 1)   // diagonal cross
            {
                path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY - dimension);
                path.AddLine(centerX + dimension, centerY - dimension, centerX - dimension, centerY + dimension);
            }
            if (style == 2)            // box
            {
                path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY + dimension);
                path.AddLine(centerX + dimension, centerY + dimension, centerX + dimension, centerY - dimension);
                path.AddLine(centerX + dimension, centerY - dimension, centerX - dimension, centerY - dimension);
                path.AddLine(centerX - dimension, centerY - dimension, centerX - dimension, centerY + dimension);
                path.CloseFigure();
            }
            else
            {   path.AddArc(centerX - dimension, centerY - dimension, 2 * dimension, 2 * dimension, 0, 360);
                path.CloseFigure();
            }
        }

        public string transformGCode(IList<string> oldCode, double scale, double angle, translate shiftToZero = translate.None)
        {
            string[] GCode = oldCode.ToArray<string>();
            StringBuilder newCode = new StringBuilder();
            StringBuilder newtext = new StringBuilder();
            double offsetX = 0;
            double offsetY = 0;
            double oldmaxx = xyzSize.maxx;
            double oldmaxy = xyzSize.maxy;
            if (shiftToZero == translate.Offset0)
            {   offsetX = xyzSize.minx; offsetY = xyzSize.miny;            }

            clear();                      // clear old drawing-paths
//            showChangedMessage = true;
            bool receivedX = false;                // remember if X,Y word was given
            bool receivedY = false;                // remember if X,Y word was given
            bool receivedZ = false;
            bool receivedI = false;                // remember if I,J word was given
            bool receivedJ = false;                // remember if I,J word was given
            bool applyscale = false;                // only change code if scale is given
            bool applyrotation = false;             // only change code if scale is given
            double factor = scale / 100;
            if (scale != 100)
                applyscale = true;                  // recalculate coordinates and replace code-line
            if (angle != 0.0)
                applyrotation = true;               // recalculate coordinates and replace code-line
                                                    //         string newCode="";
            bool codeWasChanged = false;
            string matchText = "";
            double value, valx, valy, valz, vali, valj, newvali, newvalj;
            double? newvalx, newvaly, newvalz;
            //            string tokens = "(X-?\\d+(.\\d+)?)|(Y-?\\d+(.\\d+)?)|(Z-?\\d+(.\\d+)?)|(I-?\\d+(.\\d+)?)|(J-?\\d+(.\\d+)?)";
            string tokens = "(X[-\\d]*(.\\d+)?)|(Y[-\\d]*(.\\d+)?)|(Z[-\\d]*(.\\d+)?)|(I[-\\d]*(.\\d+)?)|(J[-\\d]*(.\\d+)?)";
            // Index-assignment for m.Groups[index]        1     2        3       4         5         6       
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(tokens);
            System.Text.RegularExpressions.MatchCollection mc;
            string singleLine, origLine, tmpLine;
            valx = 0; valy = 0; valz = 0; vali = 0; valj = 0;
            newvalx = null; newvaly = null; newvalz = null; newvali = 0; newvalj = 0;
            //           MessageBox.Show(fCTBCode.LinesCount.ToString());
            for (int index = 0; index < GCode.Length; index++)
            {
                singleLine = GCode[index];
                origLine = singleLine;
                if (singleLine.IndexOf(';') >= 0)
                    singleLine = singleLine.Substring(0, singleLine.IndexOf(';'));  // remove comments
                int c_start, c_end;
                if (((c_start = singleLine.IndexOf('(')) >= 0) && ((c_end = singleLine.IndexOf(')')) >= 0))
                {
                    string part1 = singleLine.Substring(0, c_start);
                    string part2 = singleLine.Substring(c_end + 1);
                    singleLine = part1 + part2;
                }
                receivedX = false; receivedY = false; receivedZ = false; receivedI = false; receivedJ = false;
                newvalx = null; newvaly = null; newvalz = null; newvali = 0; newvalj = 0; vali = 0; valj = 0;
                mc = rex.Matches(singleLine.ToUpper());
                if ((singleLine.Length > 1) && (mc.Count > 0))
                {
                    int firstIndex = mc[0].Index;
                    int lastIndex = mc[mc.Count - 1].Index;// singleLine.Length;
                    if (firstIndex > 1)
                        tmpLine = singleLine.Substring(0, firstIndex - 1);
                    else
                        tmpLine = singleLine;
                    foreach (System.Text.RegularExpressions.Match m in mc)
                    {
                        int startIndex = m.Index;
                        int StopIndex = m.Length;
                        matchText = singleLine.Substring(startIndex, StopIndex);
                        Double.TryParse(matchText.Substring(1), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value);
                        if (applyscale && !(m.Groups[5].Success || m.Groups[6].Success))    // don't scale Z
                            value = value * factor;
                        if (m.Groups[1].Success || m.Groups[2].Success)             // X
                        { valx = value - offsetX; newvalx = valx; receivedX = true; }
                        if (m.Groups[3].Success || m.Groups[4].Success)             // Y
                        { valy = value - offsetY; newvaly = valy; receivedY = true; }
                        if (m.Groups[5].Success || m.Groups[6].Success)             // Z
                        { valz = value; newvalz = value; receivedZ = true; }
                        if (m.Groups[7].Success || m.Groups[8].Success)             // I
                        { vali = value; newvali = value; receivedI = true; }
                        if (m.Groups[9].Success || m.Groups[10].Success)            // J
                        { valj = value; newvalj = value; receivedJ = true; }
                        lastIndex = startIndex + StopIndex;
                    }
                    if ((applyrotation && (receivedX || receivedY)))
                    {
                        newvalx = valx * Math.Cos(angle * Math.PI / 180) - valy * Math.Sin(angle * Math.PI / 180);
                        newvaly = valx * Math.Sin(angle * Math.PI / 180) + valy * Math.Cos(angle * Math.PI / 180);
                        receivedX = true; receivedY = true;
                    }
                    if ((applyrotation && (receivedI || receivedJ)))
                    {
                        newvali = vali * Math.Cos(angle * Math.PI / 180) - valj * Math.Sin(angle * Math.PI / 180);
                        newvalj = vali * Math.Sin(angle * Math.PI / 180) + valj * Math.Cos(angle * Math.PI / 180);
                        receivedI = true; receivedJ = true;
                    }
                    if ((shiftToZero == translate.MirrorX)|| (shiftToZero == translate.MirrorY))           // mirror xy 
                    {
                        string tmp = tmpLine;
                        string Gnr = gcode.getStrGCode('G',tmpLine);
                        if (Gnr.Length > 1)
                        {
                            int cmdnr = Convert.ToInt16(Gnr.Substring(1));
                            if (cmdnr == 2) tmp = tmpLine.Replace(Gnr, "G03");
                            if (cmdnr == 3) tmp = tmpLine.Replace(Gnr, "G02");
                            tmpLine = tmp;
                        }
                    }
                    if (shiftToZero == translate.MirrorX)           // mirror x
                    {
                        if (newvalx != null)
                            newvalx = oldmaxx - newvalx;
                        newvali = -newvali;
                    }
                    if (shiftToZero == translate.MirrorY)           // mirror y
                    {
                        if (newvaly != null)
                            newvaly = oldmaxy - newvaly;
                        newvalj = -newvalj;
                    }
                    if (applyscale || applyrotation || (shiftToZero > 0))
                    {
                        newtext.Clear();
                        newtext.Append(tmpLine);// + string.Format(" X{0:0.0000} Y{1:0.0000}", newvalx, newvaly).Replace(',', '.');
                        if (receivedX)
                        { newtext.AppendFormat(" X{0}", gcode.frmtNum((double)newvalx)); }
                        if (receivedY)
                        { newtext.AppendFormat(" Y{0}", gcode.frmtNum((double)newvaly)); }
                        if (receivedZ)
                        { newtext.AppendFormat(" Z{0}", gcode.frmtNum((double)newvalz)); }
                        if (receivedI)
                        { newtext.AppendFormat(" I{0}", gcode.frmtNum(newvali)); }
                        if (receivedJ)
                        { newtext.AppendFormat(" J{0}", gcode.frmtNum(newvalj)); }
                        newtext.Append(origLine.Substring(lastIndex));
                        newtext.Replace(',', '.');
                        codeWasChanged = true;
                    }

                    if (receivedX || receivedY || receivedI || receivedJ || receivedZ)
                        addGCode(tmpLine, newvalx, newvaly, newvalz, newvali, newvalj);
                }       // end  if (mc.Count > 0)
//                singleLine = origLine;      //restore comments
                if (receivedX || receivedY || receivedI || receivedJ || receivedZ)
                    newCode.AppendLine(newtext.ToString());
                else
                    newCode.AppendLine(origLine);// singleLine);
            }
            if (codeWasChanged)
                return newCode.ToString();
            //           newCode.Append(oldCode);
            return string.Join("\r",GCode);//.ToString();
        }

        public void setMarkerOnDrawing(string singleLine)
        {
            string tokens = "(X-?\\d+(.\\d+)?)|(Y-?\\d+(.\\d+)?)|(Z-?\\d+(.\\d+)?)|(I-?\\d+(.\\d+)?)|(J-?\\d+(.\\d+)?)";
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(tokens);
            System.Text.RegularExpressions.MatchCollection mc;
            bool receivedX = false;
            bool receivedY = false;
            double valx = 0, valy = 0, value = 0;
            mc = rex.Matches(singleLine.ToUpper());
            if ((singleLine.Length > 1) && (mc.Count > 0))
            {
                foreach (System.Text.RegularExpressions.Match m in mc)
                {
                    int startIndex = m.Index;
                    int StopIndex = m.Length;
                    string matchText = singleLine.Substring(startIndex, StopIndex);
                    Double.TryParse(matchText.Substring(1), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value);
                    if (m.Groups[1].Success || m.Groups[2].Success)             // X
                    { valx = value; receivedX = true; }
                    if (m.Groups[3].Success || m.Groups[4].Success)             // Y
                    { valy = value; receivedY = true; }
                }
                if (receivedX && !receivedY)
                { setPosMarkerX(valx); }
                if (!receivedX && receivedY)
                { setPosMarkerY(valy); }
                if (receivedX && receivedY)
                { setPosMarker(valx, valy); }
                createMarkerPath();
            }
        }
    }


    public struct drawingProperties
    {
        public double minX,minY,maxX,maxY;
        public void drawingProperty()
         { minX = 0;minY = 0;maxX = 0;maxY=0; }
    };

    // calculate overall dimensions of drawing
    public class Dimensions
    {
        public double minx, maxx, miny, maxy, minz, maxz;
        public double dimx, dimy, dimz;

        public Dimensions()
        { Reset(); }
        public void SetXYZ(double? x, double? y, double? z)
        {   if (x != null) { SetX((double)x); }
            if (y != null) { SetY((double)y); }
            if (z != null) { SetZ((double)z); }
        }
        public void SetX(double value)
        {
            minx = Math.Min(minx, value);
            maxx = Math.Max(maxx, value);
            dimx = maxx - minx;
        }
        public void SetY(double value)
        {
            miny = Math.Min(miny, value);
            maxy = Math.Max(maxy, value);
            dimy = maxy - miny;
        }
        public void SetZ(double value)
        {
            minz = Math.Min(minz, value);
            maxz = Math.Max(maxz, value);
            dimz = maxz - minz;
        }
        // calculate min/max dimensions of a circle
        public void SetCircle(double x, double y, double radius, double start, double delta)
        {
            double end = start + delta;
            if (delta > 0)
            {
                for (double i = start; i < end; i += 10)
                {   SetX(x+radius*Math.Cos(i/180*Math.PI));
                    SetY(y + radius * Math.Sin(i / 180 * Math.PI));
                }
            }
            else
            {
                for (double i = start; i > end; i -= 10)
                {
                    SetX(x + radius * Math.Cos(i / 180 * Math.PI));
                    SetY(y + radius * Math.Sin(i / 180 * Math.PI));
                }
            }

        }
        public void Reset()
        {
            minx = Double.MaxValue;
            miny = Double.MaxValue;
            minz = Double.MaxValue;
            maxx = Double.MinValue;
            maxy = Double.MinValue;
            maxz = Double.MinValue;
            dimx = 0;
            dimy = 0;
            dimz = 0;
        }
        // return string with dimensions
        public String GetString()
        {
            return String.Format("X:[ {0:0.00} | {1:0.00} ];  Y:[ {2:0.00} | {3:0.00} ];  Z:[ {4:0.00} | {5:0.00} ]", minx, maxx, miny, maxy, minz,maxz);
        }
    }
}
