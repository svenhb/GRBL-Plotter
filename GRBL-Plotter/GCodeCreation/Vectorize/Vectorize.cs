/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2024-2024 Sven Hasemann contact: svenhb@web.de

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
 * 2024-12-13 Simple vectorization algorithm for geometric bitmaps
*/


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;

namespace GrblPlotter
{
    internal static partial class GeometricVectorize
    {
        //https://www.codeproject.com/Articles/560163/Csharp-Cubic-Spline-Interpolation
        //https://gist.github.com/dreikanter/3526685
        //https://swharden.com/blog/2022-01-22-spline-interpolation/
        //https://github.com/burningmime/curves
        //https://stackoverflow.com/questions/5525665/smoothing-a-hand-drawn-curve
        //https://hackaday.com/2024/09/14/create-custom-gridfinity-boxes-using-images-of-tools/

        public static List<List<Point>> outlinePaths = new List<List<Point>>();
        public static Color ObjectColor { get; set; }

        public static readonly int pixelScale = 10;

        internal class PxProp
        {
            internal Point p; internal int d; internal byte a; internal bool isEdge;
            public PxProp()
            { }
            public PxProp(Point pnew)	// Int32 max=2^31 = 2.147.483.647
            { p = pnew; }
        }
        private static List<PxProp> pixelPath = new List<PxProp>();
        private static List<Point> pixelOrig = new List<Point>();

        private static Dictionary<int, Point> edges = new Dictionary<int, Point>();

        private static List<int> iRemove = new List<int>();

        private static readonly byte objectMask = 0b11000000;
        private static readonly byte objectTrue = 0b10000000;
        private static readonly byte objectFalse = 0b01000000;
        private static Bitmap myBitmap;
        private static int BmpWidth, BmpHeight;
        private static byte[,] byteMap;
        private static int traceMinX, traceMinY, traceMaxX, traceMaxY, traceDimX, traceDimY, traceDim;
        private static int Smooth = 0;
        private static int contourCount = 0;
        private static bool skipOptimazion = false;

        private static bool log = true;
        private static bool logEdge = true;
        private static bool logBmp = true;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void DoTracing(Bitmap bmp, int treshold, int smooth, bool findTransparency, bool invert)
        {
            skipOptimazion = Properties.Settings.Default.importVectorizeOptimize1;
            uint logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            log = logEdge = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level1) > 0);
            //   logEdge = Properties.Settings.Default.importVectorizeOptimize3;
            //   log = Properties.Settings.Default.importVectorizeOptimize4;
            logBmp = logEdge || log;

            BmpWidth = bmp.Width;
            BmpHeight = bmp.Height;

            Smooth = smooth;
            myBitmap = new Bitmap(bmp, new Size(BmpWidth * pixelScale, BmpHeight * pixelScale));
            int psize = Image.GetPixelFormatSize(bmp.PixelFormat);    // color-deepth, including alpha?
            Logger.Info("●●●● DoTracing start PxSize:{0}  format:{1}   find transparency:{2}", psize, bmp.PixelFormat, findTransparency);

            if (findTransparency && ((psize / 8) > 3))
                byteMap = BitmapToByteMapAlpha(bmp, treshold, invert);
            else
                byteMap = BitmapToByteMapGrey(bmp, treshold, invert);

            StartTracing();
            if (log) Logger.Trace("●●●● DoTracing end outlinePaths:{0}", outlinePaths.Count);
            myBitmap.Dispose();
        }

        public static byte[,] BitmapToByteMapGrey(Bitmap bmp, int threshold, bool invert)
        {
            Logger.Info("●●●● BitmapToByteMapGrey {0} x {1} Threshold:{2}  invert:{3}", bmp.Width, bmp.Height, threshold, invert);
            ObjectColor = Color.Black;

            BitmapData SourceData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            byte[,] Result = new byte[bmp.Width, bmp.Height];
            int SourceStride = SourceData.Stride;
            int H = bmp.Height;
            int W = bmp.Width;
            int index, average;
            byte r, g, b;
            byte isBackground = 0;
            byte isForeground = 1;
            if (invert)
            {
                isBackground = 1;
                isForeground = 0;
            }
            unsafe
            {
                byte* SourcePtr = (byte*)(void*)SourceData.Scan0;
                int Ydisp = 0;
                for (int y = 0; y < H; y++)
                {
                    for (int x = 0; x < W; x++)
                    {
                        index = x * 3 + 0 + Ydisp;
                        b = SourcePtr[index];
                        g = SourcePtr[index + 1];
                        r = SourcePtr[index + 2];
                        //    average = (int)(b * 0.07 + g * 0.72 + r * 0.21);  // real brighntess needed?
                        average = (b + g + r) / 3;
                        if (average > threshold)        // check if it is background
                        {
                            Result[x, y] = isBackground;
                        }
                        else
                        {
                            Result[x, y] = isForeground;
                        }
                    }
                    Ydisp += SourceStride;
                }
            }
            bmp.UnlockBits(SourceData);
            return Result;
        }

        public static byte[,] BitmapToByteMapAlpha(Bitmap bmp, int threshold, bool invert)
        {
            Logger.Info("●●●● BitmapToByteMapAlpha {0} x {1} Threshold:{2}  invert:{3}", bmp.Width, bmp.Height, threshold, invert);
            ObjectColor = Color.Transparent;
            BitmapData dataAdjusted;
            byte[,] Result = new byte[bmp.Width, bmp.Height];
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                int psize = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;  // 4;

                dataAdjusted = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

                IntPtr ptrAdjusted = dataAdjusted.Scan0;
                long bsize = dataAdjusted.Stride * bmp.Height;
                byte[] pixelData = new byte[bsize];
                Marshal.Copy(ptrAdjusted, pixelData, 0, pixelData.Length);

                byte r = 0, g = 0, b = 0, a = 255;  // default, if no pixel found
                int bx = 0, by = 0;
                byte isBackground = 0;
                byte isForeground = 1;
                if (invert)
                {
                    isBackground = 1;
                    isForeground = 0;
                }

                long transCnt = 0;
                for (long index = 0; index < pixelData.Length; index += psize)
                {
                    a = pixelData[index + 3];

                    if (a < threshold)        // transparent?          
                    {
                        Result[bx++, by] = isBackground;
                        transCnt++;
                    }
                    else
                    {
                        Result[bx++, by] = isForeground;
                        if (ObjectColor == Color.Transparent)
                        {
                            b = pixelData[index];
                            g = pixelData[index + 1];
                            r = pixelData[index + 2];
                            ObjectColor = Color.FromArgb(a, r, g, b);
                        }
                    }
                    if (bx >= bmp.Width)
                    { bx = 0; by++; }
                }
                if (transCnt == 0)  // no background found!
                    ObjectColor = Color.Transparent;

                bmp?.UnlockBits(dataAdjusted);
            }
            catch (Exception err)
            {
                string errString = string.Format("BitmapToByteMapAlpha: size:{0} x {1}  bits:{2}", bmp.Width, bmp.Height, Image.GetPixelFormatSize(bmp.PixelFormat));
                Logger.Error(err, " {0}  ", errString);
                EventCollector.StoreException(errString + "  " + err.Message);
                //    bmp?.UnlockBits(dataAdjusted);
            }
            return Result;
        }

        private static void StartTracing()
        {
            bool isObjectLast, isObjectCur, wasScannedLast = false, wasScannedCur = false;
            byte pixelValue;
            outlinePaths.Clear();
            contourCount = 0;

            for (int y = 0; y < BmpHeight; y++)
            {
                isObjectLast = false;
                for (int x = 0; x < BmpWidth; x++)
                {
                    pixelValue = byteMap[x, y];
                    if ((pixelValue & objectTrue) > 0) { wasScannedCur = isObjectCur = true; }		// already scanned?
                    else if ((pixelValue & objectFalse) > 0) { isObjectCur = false; wasScannedCur = true; }	// already scanned?
                    else if (pixelValue > 0)
                    {
                        byteMap[x, y] |= objectTrue;
                        isObjectCur = true;
                        if (!isObjectLast)              // found 0/1 edge
                        {
                            if (log) Logger.Trace("►►► StartTracing 0/1 at x:{0} y:{1}  value:{2}", x, y, pixelValue.ToString("X4"));
                            TraceContour(x, y, true);	// now on object isClockwise=true = 6
                        }
                    }
                    else
                    {
                        byteMap[x, y] |= objectFalse;
                        isObjectCur = false;
                        if (isObjectLast && !wasScannedCur && !wasScannedLast)               // found 1/0 edge
                        {
                            if (log) Logger.Trace("►►► StartTracing 1/0 at x:{0} y:{1}  value:{2}", x - 1, y, pixelValue.ToString("X4"));
                            TraceContour(x - 1, y - 1, false);	// x-1 is on object isClockwise=false = 3
                        }
                    }
                    isObjectLast = isObjectCur;
                    wasScannedLast = wasScannedCur;
                }
            }
        }

        private static void AddToPath(List<PxProp> pixelPath, int pxX, int pxY, int length, byte dir)
        {
            PxProp pix = new PxProp();
            int x = pxX * pixelScale;
            int y = (BmpHeight - pxY) * pixelScale;
            pix.p = new Point(x, y);
            pix.d = length * pixelScale;
            pix.a = dir;
            if ((dir % 2) > 0)
            {
                if (log) Logger.Trace(" Add diagonal x:{0}  y:{1}   d:{2}  dir:{3}  change len:{4}", pxX, pxY, pix.d, dir, length * pixelScale * 1.4);
                pix.d = (int)(length * pixelScale * 1.4);
            }
            pixelPath.Add(pix);
            traceMinX = Math.Min(traceMinX, x); traceMinY = Math.Min(traceMinY, y);
            traceMaxX = Math.Max(traceMaxX, x); traceMaxY = Math.Max(traceMaxY, y);
        }

        private static void TraceContour(int x, int y, bool isClockwise)
        {
            Point start = new Point(x, y);         // 1st point in list = initial coord.
            Point next = new Point(x, y);           // current found pixel coord
            Point last = next;
            pixelPath.Clear();
            edges.Clear();
            bool startFound;
            byte searchDir;        // Initial direction, with the first hit the object is to the right/bottom (p,0,1,2 set)
            byte searchDirOld = 3;
            if (isClockwise)
                searchDirOld = 6;

            contourCount++;
            int abortCount = BmpWidth * BmpHeight;
            int cnt = 0;
            int lenBefore = 0;

            traceMaxX = traceMaxY = 0;
            traceMinX = BmpWidth * pixelScale;
            traceMinY = BmpHeight * pixelScale;
            traceDimX = traceDimY = traceDim = 0;

            /*********************************************************************
             *********** find contour pixels - pixel-center coordinates **********
             * collect path-direction-change-coordinates in List<PxProp> pixelPath
             *********************************************************************/
            do
            {
                searchDir = TracePath(ref next, searchDirOld);
                if (searchDir > 8)  // no neighbor pixels
                {
                    Logger.Warn("!!!!! TraceContour no neighbor pixel !!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    return;
                }

                startFound = (next == start);       // stop when 1st point repeats

                lenBefore++;
                if (searchDir != searchDirOld)		// skip intermediate pixels
                {
                    AddToPath(pixelPath, last.X, last.Y, lenBefore, searchDirOld);
                    if (log && skipOptimazion) Logger.Trace("     TracePath  AddToPath  dir:{0} pos:{1}   last-dir:{2}  last-pos;{3}", searchDir, next, searchDirOld, last);
                    lenBefore = 0;
                }

                searchDirOld = searchDir;
                last = next;
                if (cnt++ > abortCount)
                {
                    Logger.Error("#################### TraceContour abort loop (more vectors than pixel area) #####################");
                    return; break;
                }    // safety stop
            } while (!startFound);

            pixelPath[0].a = searchDir;
            if (!isClockwise)
                pixelPath[0].d = lenBefore * pixelScale;	// correct start direction 

            pixelPath.Add(pixelPath[0]);

            if (log) Logger.Trace("►►► TraceContour Nr.:{0}  isClockwise:{1} pixelPath count:{2}  x:{3} - {4}  y:{5} - {6}", contourCount, isClockwise, pixelPath.Count, traceMinX, traceMaxX, traceMinY, traceMaxY);
            List<Point> vectorPath = new List<Point>();

            traceDimX = traceMaxX - traceMinX;
            traceDimY = traceMaxY - traceMinY;
            traceDim = (traceDimX + traceDimY) / 2;
            if (log) Logger.Trace("    TraceContour dimension x:{0}  y:{1}  minx:{2}  miny:{3}  maxx:{4}  maxy:{5}", traceDimX, traceDimY, traceMinX, traceMinY, traceMaxX, traceMaxY);

            if (skipOptimazion || (pixelPath.Count <= 10) || CheckForSquare())
            {
                foreach (PxProp point in pixelPath)
                    vectorPath.Add(new Point(point.p.X, point.p.Y));

                outlinePaths.Add(new List<Point>(vectorPath));

                if (log) Logger.Trace("    TraceContour skipOptimazion pixelPath count:{0}", pixelPath.Count);
                return;
            }

            if (false)
            {
                List<Point> pixelPathPartial = new List<Point>();
                for (int l = 0; l < pixelPath.Count; l++)
                {
                    pixelPathPartial.Add(pixelPath[l].p);
                }
                List<Point> secPoint = DouglasPeuckerReduction(pixelPathPartial, 20);
                Logger.Trace("    DouglasPeuckerReduction  {0}  {1}", pixelPathPartial.Count, secPoint.Count);
                /* show DouglasPeucker result */
                int ii = 0;
                for (int k = 0; k < pixelPath.Count; k++)
                {
                    if (ii < secPoint.Count)
                        pixelPath[k].p = secPoint[ii++];
                    else
                        pixelPath[k].p = secPoint[secPoint.Count - 1];
                    pixelPath[k].isEdge = true;
                }
            }
            /*********************************************************************
             *********** rework contour pixels ***********************************
             ***** pixelParh.coordinates will be moified
             *********************************************************************/
            // https://www.chai3d.org/download/doc/html/fig-color-palette.jpg

            int LengthLimit = traceDim / 5;

            /* find and mark object edges */
            MarkEdges(LengthLimit);
            MarkDirectionChanges(LengthLimit);


            /* analyze sections between edges */
            iRemove.Clear();
            AnalyzeSections(true);  // 1st mark diagonal

            /* remove pixels to smoothen contour */
            iRemove.Sort((a, b) => b.CompareTo(a));
            foreach (int a in iRemove)
            {
                BitmapSetPixel(pixelPath[Geti(a)].p, Color.Yellow); // log removed pixel
                pixelPath.RemoveAt(Geti(a));
            }

            edges.Clear();
            pixelOrig.Clear();
            for (int i = 0; i < pixelPath.Count; i++)
            {
                pixelOrig.Add(pixelPath[i].p);      // save original coordinates
                if (pixelPath[i].isEdge)
                { edges.Add(i, pixelPath[i].p); }   // save edge-indexes
            }

            /* apply arcs to remaining sections */
            iRemove.Clear();
            AnalyzeSections(false);                 // 2nd apply arcs
            iRemove.Sort((a, b) => b.CompareTo(a));
            foreach (int a in iRemove)
            {
                BitmapSetPixel(pixelPath[Geti(a)].p, Color.YellowGreen);// log removed pixel
                pixelPath.RemoveAt(Geti(a));
            }
            iRemove.Clear();

            /* clean up single 1px steps */
            Point pTmp;
            for (int i = pixelPath.Count - 2; i >= 0; i--)
            {
                /*    if ((pixelPath[i].d == pixelScale) && !pixelPath[i].isEdge)
                    {
                        pTmp = pixelPath[Geti(i - 1)].p = pixelPath[i].p = GetPointBetween(pixelPath[Geti(i - 1)].p, pixelPath[i].p);
                        Logger.Trace("clean up single 1px steps {0}", pTmp);
                    }*/
                if ((pixelPath[Geti(i - 1)].d > 3 * pixelScale) && (pixelPath[i].d == pixelScale) && (pixelPath[i + 1].d == pixelScale) && (pixelPath[Geti(i + 2)].d > 3 * pixelScale))
                {
                    Logger.Trace("clean up single 1px steps {0}", pixelPath[Geti(i)].p);
                    pixelPath.RemoveAt(i);
                }
            }

            /* copy final contour to outlinePaths */
            foreach (PxProp point in pixelPath)
                vectorPath.Add(new Point(point.p.X, point.p.Y));

            outlinePaths.Add(new List<Point>(vectorPath));

            if (logBmp)
            {
                Logger.Trace("◯◯◯ Save Bitmap to {0}\\bitmap1.png", Datapath.LogFiles);
                myBitmap.Save(Datapath.LogFiles + "\\bitmap1.png");
            }
        }

        // like a radar, scan for 0/1 transient in front of current position
        // The object is always to the right of the current direction
        private static readonly short[][] searchDirection1 = { new short[] { 1, 0 }, new short[] { 1, 1 }, new short[] { 0, 1 }, new short[] { -1, 1 }, new short[] { -1, 0 }, new short[] { -1, -1 }, new short[] { 0, -1 }, new short[] { 1, -1 } }; // xy-dir by index
        private static byte TracePath(ref Point p, byte startDirection)
        {
            int scanFrom = startDirection - 2;          // 5 6 7
            int scanTo = startDirection + 4;            // 4 p 0	// +4allow 180° turn
            short index, itst;                          // 3 2 1
            bool isObjectNow;
            bool isObjectLast = false;
            for (int i = scanFrom; i <= scanTo; i++)
            {
                index = (short)i;
                if (index < 0) { index += 8; }
                if (index > 7) { index -= 8; }
                isObjectNow = CheckPixelValue(p.X + searchDirection1[index][0], p.Y + searchDirection1[index][1]);
                if (isObjectNow && !isObjectLast)
                {
                    if ((index % 2) > 0)    //uneven = diagonal - try to avoid
                    {
                        itst = (short)(index + 1);
                        if (itst > 7)
                            itst -= 8;
                        if (CheckPixelValue(p.X + searchDirection1[itst][0], p.Y + searchDirection1[itst][1]))
                        {
                            index = itst;
                        }
                    }
                    p.X += searchDirection1[index][0];
                    p.Y += searchDirection1[index][1];
                    return (byte)index;
                    break;
                }
                isObjectLast = isObjectNow;
            }
            // single pixel? No neighbor found
            return byte.MaxValue;
        }

        private static bool CheckPixelValue(int cx, int cy, bool outerLimit = false)
        {
            /* check if it is object, mark position as 'used' */
            if ((cx < 0) || (cx >= BmpWidth) || (cy < 0) || (cy >= BmpHeight))
            { return outerLimit; }
            else
            {
                if (((byteMap[cx, cy] & 0x01)) > 0)
                {
                    byteMap[cx, cy] |= objectTrue;      // mark as object
                    return true;
                }
                else
                {
                    byteMap[cx, cy] |= objectFalse;     // mark as background
                    return false;
                }
            }
        }

        private static int Geti(int i)
        {
            if (i < 0) { i += (pixelPath.Count - 1); }
            if (i > pixelPath.Count - 1) { i = 1 + i - pixelPath.Count; }
            return i;
        }

        private static void MarkToRemovePointsBetween(int start, int end)
        {
            pixelPath[Geti(start)].d = pixelPath[Geti(end)].d = 2 * pixelScale;
            pixelPath[Geti(start)].isEdge = pixelPath[Geti(end)].isEdge = true;
            for (int j = start + 1; j < end; j++)
            {
                iRemove.Add(Geti(j));
            }
        }

        private static void BitmapSetPixel(Point p, Color col)
        {
            if (!logBmp)
                return;
            int x = p.X;// / pixelScale;
                        //    int y = BmpHeight - p.Y / pixelScale;
            int y = BmpHeight * pixelScale - p.Y;

            Color newColor = System.Drawing.Color.FromArgb(128, col);
            if ((x >= 0) && (x < BmpWidth * pixelScale) && (y >= 0) && (y < BmpHeight * pixelScale))
                myBitmap.SetPixel(x, y, newColor);
        }

        private static void BitmapMarkPixel(Point p, Color col)
        {
            if (!logBmp)
                return;
            int x, y;
            Color newColor = System.Drawing.Color.FromArgb(128, col);
            for (int i = 0; i < 8; i++)
            {
                //    x = p.X / pixelScale + searchDirection1[i][0];
                //    y = BmpHeight - p.Y / pixelScale + searchDirection1[i][1];
                x = p.X + searchDirection1[i][0];
                y = BmpHeight * pixelScale - p.Y + searchDirection1[i][1];

                if ((x >= 0) && (x < BmpWidth * pixelScale) && (y >= 0) && (y < BmpHeight * pixelScale))
                    myBitmap.SetPixel(x, y, newColor);
            }
        }

        private static void BitmapMarkArc(Point pCenter, Point pStart, Point pEnd, Color c1, Color c2)
        {
            BitmapMarkPixel(pCenter, Color.Yellow);
            using (var graphics = Graphics.FromImage(myBitmap))
            {
                //     graphics.DrawLine(new System.Drawing.Pen(c1), pStart.X / pixelScale, BmpHeight - pStart.Y / pixelScale, pCenter.X / pixelScale, BmpHeight - pCenter.Y / pixelScale);
                //     graphics.DrawLine(new System.Drawing.Pen(c2), pEnd.X / pixelScale, BmpHeight - pEnd.Y / pixelScale, pCenter.X / pixelScale, BmpHeight - pCenter.Y / pixelScale);
                graphics.DrawLine(new System.Drawing.Pen(c1), pStart.X, BmpHeight * pixelScale - pStart.Y, pCenter.X, BmpHeight * pixelScale - pCenter.Y);
                graphics.DrawLine(new System.Drawing.Pen(c2), pEnd.X, BmpHeight * pixelScale - pEnd.Y, pCenter.X, BmpHeight * pixelScale - pCenter.Y);
            }
        }
        private static void BitmapDrawLine(Point pStart, Point pEnd, Color c1)
        {
            using (var graphics = Graphics.FromImage(myBitmap))
            {
                graphics.DrawLine(new System.Drawing.Pen(c1, 2), pStart.X, BmpHeight * pixelScale - pStart.Y, pEnd.X, BmpHeight * pixelScale - pEnd.Y);
            }
        }

    }
}
