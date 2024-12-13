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
using Color = System.Drawing.Color;

namespace GrblPlotter
{
    internal static partial class GeometricVectorize
    {

        private static bool CheckForSquare()
        {
            return false;
            int min = 10 * pixelScale, max = 100 * pixelScale;
            if ((pixelPath.Count < 50) && (min <= traceDimX && traceDimX <= max) && (min <= traceDimY && traceDimY <= max))
            {
                //alternativ
                if (false)
                {
                    List<Point> pixelPathPartial = new List<Point>();
                    for (int l = 0; l < pixelPath.Count; l++)
                    {
                        pixelPathPartial.Add(pixelPath[l].p);
                    }
                    List<Point> sectionPoints = DouglasPeuckerReduction(pixelPathPartial, 15);
                    Logger.Trace("    CheckForSquare DouglasPeuckerReduction  {0}  {1}", pixelPathPartial.Count, sectionPoints.Count);
                    /* show DouglasPeucker result */
                    pixelPath.Clear();
                    for (int k = 0; k < sectionPoints.Count; k++)
                    {
                        PxProp pix = new PxProp();
                        pix.p = sectionPoints[k];
                        pixelPath.Add(pix);
                    }
                    return true;
                }



                iRemove.Clear();
                int len = Math.Max(traceDimX, traceDimY) * 2 / 5;
                int edgeCount = 0;
                List<int> edges = new List<int>();
                //           Logger.Trace("    CheckForSquare   len {0}  {1}", len, sectionPoints.Count);
                for (int i = 0; i < pixelPath.Count; i++)
                {
                    Logger.Trace("Found {0}  len:{1}   min:{2}", i, pixelPath[Geti(i)].d, len);
                    if (pixelPath[Geti(i)].d > len)
                    {
                        pixelPath[Geti(i)].isEdge = true;
                        pixelPath[Geti(i - 1)].isEdge = true;
                        edges.Add(Geti(i - 1));
                        edges.Add(Geti(i));
                        edgeCount++;
                        BitmapMarkPixel(pixelPath[Geti(i)].p, Color.PaleGreen);
                        BitmapMarkPixel(pixelPath[Geti(i - 1)].p, Color.PaleGreen);
                        //         Logger.Trace("Found {0}  {1}", i, Geti(i));
                    }
                }
                if (edgeCount == 0)
                    return false;
                for (int i = 0; i < edges.Count - 1; i++)
                {
                    Logger.Trace("check {0}  {1}  {2}", i, edges[i + 1], edges[i]);
                    if (edges[i + 1] > (edges[i] + 1))
                    {
                        MarkToRemovePointsBetween(edges[i], edges[i + 1]);
                    }
                }
                //  if (edges[0] > (edges[edges.Count - 1] + 1))
                {
                    MarkToRemovePointsBetween(edges[edges.Count - 1], pixelPath.Count - 1);
                    MarkToRemovePointsBetween(0, edges[0]);
                    iRemove.Add(0);
                    iRemove.Add(pixelPath.Count - 1);
                    pixelPath.Add(pixelPath[edges[0]]);
                }

                iRemove.Sort((a, b) => b.CompareTo(a));
                foreach (int a in iRemove)
                {
                    BitmapSetPixel(pixelPath[Geti(a)].p, Color.YellowGreen);// log removed pixel
                    pixelPath.RemoveAt(Geti(a));
                }
                iRemove.Clear();
                myBitmap.Save(Datapath.LogFiles + "\\bitmap1.png");

                return true;
            }
            return false;
        }

        private static void MarkEdges(int limit)
        {
            List<PxProp> pP = pixelPath;
            if (limit < 50) limit = 50;
            if (logEdge) Logger.Trace("◆◆◆ MarkEdge pP.Count:{0}   limit:{1}", pP.Count, limit);

            int pxWidth = 1 * pixelScale;

            for (int i = 0; i < pP.Count; i++)
            {
                if (pixelPath[i].d == 0) Logger.Trace("MarkEdge NULL {0}", i);

                /* max dimension */
                if ((pP[i].p.X == traceMinX) || (pP[i].p.X == traceMaxX) || (pP[i].p.Y == traceMinY) || (pP[i].p.Y == traceMaxY))
                {
                    pP[i].isEdge = true;
                    BitmapMarkPixel(pP[i].p, Color.PaleGreen);
                    if (logEdge) Logger.Trace("     MarkEdge max dim {0,3}  {1}", i, pP[i].p);
                }

                /* long distance */
                /*   if (pP[i].d >= limit)
                   {
                       pP[i].isEdge = true;
                       BitmapMarkPixel(pP[i].p, Color.LawnGreen);
                       if (logEdge) Logger.Trace("    MarkEdge long dist pos:{0}  dist:{1}   limit:{2}", i, pP[i].d, limit);
                       if (i > 0)
                       {
                           pP[i - 1].isEdge = true;
                           BitmapMarkPixel(pP[i - 1].p, Color.LawnGreen);
                           if (logEdge) Logger.Trace("    MarkEdge long dist {0}  limit:{1}", pP[i - 1].p, limit);
                       }
                   }*/

                /* any U-turn */
                if ((i > 0) && (i < pP.Count - 1) &&
                                (pP[i].a != pP[i - 1].a) && (pP[i].a != pP[i + 1].a) &&     //   ___i
                                (Math.Abs(pP[i - 1].a - pP[Geti(i + 1)].a) == 4) &&         // __|  |__

                                ((pP[i].d >= pxWidth) &&
                                (((pP[i - 1].d == pxWidth) && (pP[Geti(i + 1)].d >= pxWidth)) ||
                                 ((pP[i - 1].d >= pxWidth) && (pP[Geti(i + 1)].d == pxWidth)) ||
                                 //        ((pP[i - 1].d == pxWidth) && (pP[Geti(i)].d == pxWidth) && (pP[Geti(i + 1)].d == pxWidth)) ||
                                 ((pP[i - 1].d > pxWidth) && (pP[Geti(i + 1)].d > pxWidth)))))
                {
                    pP[i].isEdge = pP[Geti(i - 1)].isEdge = true;
                    BitmapMarkPixel(pP[i].p, Color.Lime);
                    BitmapMarkPixel(pP[Geti(i - 1)].p, Color.GreenYellow);
                    if (logEdge) Logger.Trace("     MarkEdge U-turn  {0,3}  {1}  and {2,3}   {3}   limit:{4}", i, pP[i].p, (i - 1), pP[i - 1].p, limit);
                }
            }
        }

        private static void MarkDirectionChanges(int LengthLimit)
        {
            if (logEdge) Logger.Trace("◆◆◆ MarkDirectionChanges count:{0} limit:{1}     color:Green/Olive", pixelPath.Count, LengthLimit);
            int k, range = Math.Min(20, LengthLimit);
            for (int i = 0; i < pixelPath.Count; i++)
            {
                if (pixelPath[i].d == 0) Logger.Trace("MarkDirectionChanges NULL {0}", i);

                if (pixelPath[i].isEdge)
                    continue;

                k = FindEdgeUpwards(i, i + range);
                if (k != i)
                {
                    if (k == i + range)
                    { i += range - 2; continue; }
                    pixelPath[Geti(k)].isEdge = true;
                    BitmapMarkPixel(pixelPath[Geti(k)].p, Color.LightBlue);
                    if (logEdge) Logger.Trace("     MarkDirectionChanges {0,3}  {1}", k, pixelPath[Geti(k)].p);
                    i = k;
                }
            }
        }

        private static int FindEdgeUpwards(int start, int end)
        {
            int d0, d1, d2, d3;
            int max = 3 * pixelScale;
            double rise0, rise1;

            if (true)
            {
                if ((pixelPath[Geti(start)].d > max) && (pixelPath[Geti(start + 2)].d == pixelScale) && (pixelPath[Geti(start + 4)].d == pixelScale))
                {
                    if (logEdge) Logger.Trace("    FindEdgeUpwards 1st is edge (3,x,1,x,1):{0}   {1}   GreenYellow", start, pixelPath[Geti(start)].p);
                    BitmapMarkPixel(pixelPath[Geti(start)].p, Color.GreenYellow);
                    pixelPath[Geti(start)].isEdge = true;
                    return start;
                }
            }

            for (int k = start; k <= end; k += 2)
            {
                if (pixelPath[Geti(start)].a != pixelPath[Geti(k + 2)].a)		// change in direction?
                    return k + 1;
                if (pixelPath[Geti(start + 1)].a != pixelPath[Geti(k + 3)].a)	// change in direction?
                    return k + 2;
            }
            return end;
        }

        private static void AnalyzeSections(bool mark)
        {
            List<PxProp> pP = pixelPath;
            if (log) Logger.Trace("◆◆◆ AnalyzeSections pP.Count:{0}    {1}", pP.Count, mark ? "mark linear sections diagonale arc split" : "apply arcs");
            Point pCenter;
            PxProp pStart, pEnd;
            int segmentCnt = 0;
            for (int i = 0; i <= pP.Count; i++)
            {

                pStart = pP[Geti(i)];
                pEnd = pP[Geti(i + 1)];
                if (pStart.isEdge && !pEnd.isEdge)	// start of arc
                {
                    for (int k = i + 2; k <= pP.Count; k++)
                    {
                        pEnd = pP[Geti(k)];
                        if (pEnd.isEdge)  // end of arc
                        {
                            segmentCnt++;
                            if (Contains1pxLengths(i, k, segmentCnt))
                            {


                                int range = k - i - 1;
                                if (range <= 2)
                                {
                                    if (GetDistance(pP[Geti(i)].p, pP[Geti(k)].p) < (5 * pixelScale))
                                    {
                                        pP[Geti(i + 1)].p = GetPointBetween(pP[Geti(i)].p, pP[Geti(k)].p);
                                        if (range == 2) { iRemove.Add(i + 2); }
                                        pP[Geti(i)].d += (int)(pixelScale * 0.5);       // adapt length to avoid clean-up
                                        pP[Geti(i + 1)].d += (int)(pixelScale * 0.5);   // adapt length to avoid clean-up
                                    }
                                }
                                else //if (range >= 5)
                                {
                                    if (mark)
                                    {
                                        //    if (Contains1pxLengths(i, k))
                                        //    {
                                        CheckForMissingEdge(ref i, ref k);
                                        List<Point> ranges = new List<Point>();
                                        if (range >= 3)
                                        {
                                            ranges = CheckDiagonalSegemnts(i, k, segmentCnt);
                                        }

                                        if (ranges.Count > 0)
                                        {
                                            for (int c = 0; c < ranges.Count; c++)
                                            {
                                                if ((ranges[c].X == 0) && (ranges[c].Y == 0))
                                                    continue;

                                                if ((ranges[c].X == i) && (ranges[c].Y == k))
                                                    continue;

                                                range = ranges[c].Y - ranges[c].X - 1;
                                                if (range <= 2)
                                                {
                                                    if (GetDistance(pP[Geti(ranges[c].X)].p, pP[Geti(ranges[c].Y)].p) < (5 * pixelScale))
                                                    {
                                                        pP[Geti(ranges[c].X + 1)].p = GetPointBetween(pP[Geti(ranges[c].X)].p, pP[Geti(ranges[c].Y)].p);
                                                        if (range == 2) { iRemove.Add(ranges[c].X + 2); }
                                                        pP[Geti(ranges[c].X)].d += (int)(pixelScale * 0.5);   	// adapt length to avoid clean-up
                                                        pP[Geti(ranges[c].X + 1)].d += (int)(pixelScale * 0.5); // adapt length to avoid clean-up
                                                    }
                                                }
                                                else
                                                { CheckDiagonalSegemnts(ranges[c].X, ranges[c].Y, segmentCnt); }
                                            }
                                        }
                                        //}
                                    }
                                    else
                                    {
                                        ApplyArcs(i, k, segmentCnt);
                                    }
                                }
                            }
                            i = k - 1;
                            break;
                        }
                    }
                }
            }
        }

        private static bool Contains1pxLengths(int start, int end, int nr)
        {
            /* check if section contains 1px lengths (if not: no arc or diagonale) */
            int cnt1px = 0;
            int length = 0;
            int range = end - start;
            for (int k = start; k < end; k++)
            {
                length += pixelPath[Geti(k)].d;
                if (pixelPath[Geti(k)].d == pixelScale)
                {
                    cnt1px++;
                }
            }
            int dist = (int)GetDistance(pixelPath[Geti(start)].p, pixelPath[Geti(end)].p);

            if ((cnt1px == 0) || (dist / range > 25))
            {
                for (int k = start; k < end; k++)
                {
                    pixelPath[k].isEdge = true;
                }
                if (log) Logger.Trace("     Contains1pxLengths    nr:{0,3}   start:{1}  end:{2} range:{3}  cnt1px:{4}  length:{5}  distance:{6}  distance/range:{7}", nr, start, end, range, cnt1px, length, dist, dist / range);
                return false;
            }
            return true;
        }

        private static void CheckForMissingEdge(ref int start, ref int end)
        {
            /* check for missing edge = high diatance at 1st or last point */
            Point pStart = pixelPath[Geti(start)].p;
            Point pEnd = pixelPath[Geti(end)].p;
            int h0 = PerpendicularDistanceInt(pStart, pEnd, pixelPath[Geti(start + 1)].p);
            int h1 = PerpendicularDistanceInt(pStart, pEnd, pixelPath[Geti(start + 2)].p);
            int h2 = PerpendicularDistanceInt(pStart, pEnd, pixelPath[Geti(start + 3)].p);
            if ((Math.Abs(h0 + h1) > 10 * pixelScale * pixelScale))// || (Math.Abs(h1 + h2) > 10 * pixelScale * pixelScale))
            {
                start++;
                pStart = pixelPath[Geti(start)].p;
                h0 = PerpendicularDistanceInt(pStart, pEnd, pixelPath[Geti(start + 1)].p);
                pixelPath[Geti(start)].isEdge = true;
                if (log) Logger.Trace("---- CheckForMissingEdge Reduce start to {0}  high step {1}", start, (h0 + h1) / 2);
            }
            h0 = PerpendicularDistanceInt(pStart, pEnd, pixelPath[Geti(end - 1)].p);
            h1 = PerpendicularDistanceInt(pStart, pEnd, pixelPath[Geti(end - 2)].p);
            h2 = PerpendicularDistanceInt(pStart, pEnd, pixelPath[Geti(end - 3)].p);
            if ((Math.Abs(h0 + h1) > 10 * pixelScale * pixelScale))// || (Math.Abs(h1 + h2) > 10 * pixelScale * pixelScale))
            {
                end--;
                pEnd = pixelPath[Geti(end)].p;
                h0 = PerpendicularDistanceInt(pStart, pEnd, pixelPath[Geti(end - 1)].p);
                pixelPath[Geti(end)].isEdge = true;
                if (log) Logger.Trace("---- CheckForMissingEdge Reduce end to {0}  high step {1}", end, (h0 + h1) / 2);
            }
        }

        private static List<Point> CheckDiagonalSegemnts(int start, int end, int nr)
        {
            /* collect all distance and gradient data */
            pdDistance.Clear();
            pdGradient.Clear();
            List<Point> removed = new List<Point>();
            int diff0, diff1, diff2, havg = 0, h0, h1 = 0;
            int pdMax = 0, pdMaxIndex = start;
            int pdMin = 1000, pdMinIndex = end;
            int diff01Max = 0;
            int diff01Avg = 0;
            int diff01Sum = 0;
            int avgPd = 0;
            string distance = "";
            string gradient = "";
            string arcerror = "";
            Point pStart = pixelPath[Geti(start)].p;
            Point pEnd = pixelPath[Geti(end)].p;
            int range = end - start * 1;
            int scan = range / 4;
            int pdRangeMin = 1000, pdRangeMinIndex = end;

            /* Calculate gradient */
            for (int ih = start + 1; ih < end; ih++)
            {
                h0 = PerpendicularDistanceInt(pStart, pEnd, pixelPath[Geti(ih)].p);           // 1px = pixelScale * pixelScale
                avgPd += h0;
                pdDistance.Add(h0);
                if (log) distance += string.Format("{0:0}\t", h0);
                diff0 = h0 - h1;
                diff01Sum += diff0;
                pdGradient.Add(diff0);
                if (log) gradient += string.Format("{0:0}\t", diff0);
                h1 = h0;

                if (h0 > pdMax) { pdMaxIndex = ih; pdMax = h0; }
                if (h0 < pdMin) { pdMinIndex = ih; pdMin = h0; }
                if (Math.Abs(diff0) > diff01Max) { diff01Max = Math.Abs(diff0); }
                if ((ih > (start + scan)) && (ih < (end - scan)))
                {
                    if (Math.Abs(h0) < pdRangeMin) { pdRangeMinIndex = ih; pdRangeMin = Math.Abs(h0); }
                }
                diff01Avg = (diff01Avg + (h0 + h1) / 2) / 2;
            }
            avgPd = Math.Abs(avgPd / range);
            int hlimit1 = (int)(1.2 * pixelScale * pixelScale);
            int hlimit2 = (int)(1.5 * pixelScale * pixelScale);
            bool isDiagonal = false;
            int segmentLength = (int)GetDistance(pStart, pEnd);
            int absPdMax = Math.Max(Math.Abs(pdMax), Math.Abs(pdMin));
            string logtxt = "";
            /* if change in PerpendicularDistance is too small -> diagonale */
            //   Logger.Trace("#### range:{0}  avgPd:{1} < hlimit1:{2} (pdMax - pdMin):{3}  absPdMax:{4}  diff01Sum:{5}", range, avgPd, hlimit1, (pdMax - pdMin), absPdMax, diff01Sum);
            if (range > 20)
            {
                if (avgPd < hlimit1)
                {
                    if (log) logtxt = string.Format("# avg:{0} < hlimit1:{1}", avgPd, hlimit1);
                    isDiagonal = true;
                }
                else if ((Math.Abs(pdMax - pdMin) < hlimit1) ||
                               (absPdMax < hlimit2))
                {
                    if (log) logtxt = string.Format("# Math.Abs(pdMax - pdMin):{0} < hlimit1:{1} || absPdMax:{2} < hlimit2:{3}", Math.Abs(pdMax - pdMin), hlimit1, absPdMax, hlimit2);
                    isDiagonal = true;
                }
                else
                    if (log) logtxt = string.Format("# range > 20 avg:{0} <? hlimit1:{1}", avgPd, hlimit1);
            }
            else
            {
                if ((absPdMax < 1.1 * segmentLength) && (segmentLength < 1.1 * hlimit1))
                {
                    if (log) logtxt = string.Format("# absPdMax:{0} < segmentLength:{1}", absPdMax, 1.1 * segmentLength);
                    isDiagonal = true;
                }
                else
                    if (log) logtxt = string.Format("# range < 20 absPdMax:{0} <? segmentLength:{1} <? hlimit1:{2}", absPdMax, 1.1 * segmentLength, hlimit1);
            }

            if (isDiagonal)
            {
                removed.Add(new Point(0, 0));
                if (log) Logger.Trace("     CheckDiagonalSegemnts nr:{0,3} is diagonale  {1,4}  {2,4}  {3,15}  {4,15}  range:{5}  absPdMax:{6}  segmentLength:{7}  {8}", nr, start, end, pixelPath[start].p, pixelPath[end].p, range, absPdMax, segmentLength, logtxt);
                MarkToRemovePointsBetween(start, end);
                BitmapDrawLine(pStart, pEnd, Color.LightPink);
                return removed;
            }

            pdMax = Math.Max(Math.Abs(pdMin), Math.Abs(pdMax));
            /* calculate arc shape error */
            /* perhaps we have a circular segment https://en.wikipedia.org/wiki/Circular_segment */
            double chordLength = pixelScale * GetDistance(pixelPath[start].p, pixelPath[end].p);    // scale to pdMax ratio
            double r = ((4 * pdMax * pdMax + chordLength * chordLength) / (8 * pdMax));

            double x2, x1 = chordLength / ((pdDistance.Count - 1));   // r²=x²+y²  y=sqrt(r²-x²)
            double r2 = r * r;
            int xd = (int)(r - chordLength / 2);
            int hd = (int)r - pdMax;
            double e, y, error = 1;
            x2 = (r - xd);

            /* compare actual PerpendicularDistances with theoretical arc distances */
            for (int id = 0; id < pdDistance.Count; id++)
            {
                x2 = (r - xd) - x1 * id;
                y = Math.Sqrt(r2 - x2 * x2) - hd;
                if ((y < pixelScale) || (Math.Abs(pdDistance[id]) == y))
                    e = 0;
                else
                    e = (y - Math.Abs(pdDistance[id])) / y;
                error += Math.Abs(e);
                if (log) arcerror += string.Format("{0:0.00}\t", e);
            }

            double shapeErr = error / pdDistance.Count;

            /* could be an arc */
            if ((shapeErr < 0.4) || ((range < 20) && (shapeErr < 0.7)))
            {
                removed.Add(new Point(start, end));
                if (log) Logger.Trace("     CheckDiagonalSegemnts nr:{0,3} is arc        {1,4}  {2,4}  {3,15}  {4,15}  shapeErr:{5:0.00}  {6}", nr, start, end, pixelPath[start].p, pixelPath[end].p, shapeErr, logtxt);
                return removed;
            }

            if (false)
            {
                List<Point> pixelPathPartial = new List<Point>();
                for (int l = start; l < end; l++)
                {
                    pixelPathPartial.Add(pixelPath[l].p);
                }
                List<Point> secPoint = DouglasPeuckerReduction(pixelPathPartial, 15);
                Logger.Trace("    DouglasPeuckerReduction  {0}  {1}", pixelPathPartial.Count, secPoint.Count);

                if (!Properties.Settings.Default.importVectorizeOptimize2)
                { /* show DouglasPeucker result */
                    int ii = 0;
                    for (int k = start; k < end; k++)
                    {
                        if (ii < secPoint.Count)
                            pixelPath[k].p = secPoint[ii++];
                        else
                            pixelPath[k].p = secPoint[secPoint.Count - 1];
                        pixelPath[k].isEdge = true;
                    }
                }
                else
                    ApplyBezierFitCurveSection(secPoint, start, end, 10);/* adjust original pixelPath coordinates to match resultPath*/
                removed.Add(new Point(start, end));
                return removed;
            }

            /* drop in height -> split into two segments */
            if ((double)pdRangeMin / absPdMax < 0.1)
            {
                removed.Add(new Point(start, pdRangeMinIndex));
                removed.Add(new Point(pdRangeMinIndex, end));
                pixelPath[pdRangeMinIndex].isEdge = true;
                //      if (log) Logger.Trace("       pdRangeMin / Math.Abs(pdMax):{0}  {1}   {2}", (double)pdRangeMin / absPdMax, pdRangeMin, absPdMax);
                if (log) Logger.Trace("     CheckDiagonalSegemnts nr:{0,3} is split 1     {1,4}  {2,4}  {3,15}  {4,15}  split at:{5,4}   pdRangeMin / Math.Abs(pdMax):{6:0.00}  {7}   {8}", nr, start, end, pixelPath[start].p, pixelPath[end].p, pdRangeMinIndex, (double)pdRangeMin / absPdMax, pdRangeMin, absPdMax);
                if (log) Logger.Trace("       Error:{0:0.00}   pdMin:{1}  pdMax:{2}  min/max:{3:0.00}", shapeErr, pdMin, pdMax, (double)pdMin / pdMax);
                if (log) Logger.Trace("       raw distance  p:{0}  {1}", pixelPath[start].p, distance);
                if (log) Logger.Trace("       raw gradient  p:{0}  {1}", pixelPath[end].p, gradient);
                if (log) Logger.Trace("       pdMin:{0}  pdmax:{1}  pdrange:{2}  diff01:{3}", pdMin, pdMax, Math.Abs(pdMax - pdMin), diff01Max);
                return removed;
            }

            int ip;
            int diffLimit = 5;
            int inc = 2, match = 1;
            int matchCnt = 0;
            int ic;
            int rStart = start, rEnd = start;

            bool logSplit = true;
            for (int ig = 0; ig < pdGradient.Count - 8; ig++)
            {
                for (ip = ig + inc; ip < Math.Min(pdGradient.Count, ig + 8); ip++)
                {
                    if (ig >= pdGradient.Count)
                        break;
                    //        if (logSplit) Logger.Trace("Find pattern at:{0}   inc:{1}   ip {2}  {3}  {4}", ig, inc, ip, pdGradient[ig], pdGradient[ip]);
                    if (Math.Abs(pdGradient[ig] - pdGradient[ip]) < diffLimit)
                    {
                        for (ic = 1; ic < inc; ic++)
                        {
                            if (ig + ic >= pdGradient.Count - 1)
                                break;
                            if (ip + ic < pdGradient.Count)
                                //                    if (logSplit) Logger.Trace("   test ig {0}  ip {1}   {2}  {3}", ig + ic, ip + ic, pdGradient[ig + ic], pdGradient[ip + ic]);
                                if ((ip + ic < pdGradient.Count) && (Math.Abs(pdGradient[ig + ic] - pdGradient[ip + ic]) < diffLimit))
                                { match++; }
                        }
                        //            if (logSplit) Logger.Trace("   result inc {0}  match {1}   ", inc, match);

                        if (match == inc)
                        {
                            bool matchFail = false;
                            for (int im = ig + inc; im < pdGradient.Count; im += inc)
                            {
                                for (ic = 0; ic < inc; ic++)
                                {
                                    //        if (im + ic < pdGradient.Count)
                                    //            Logger.Trace("   compare ig {0}  im {1}   {2}  {3}", ig + ic, im + ic, pdGradient[ig + ic], pdGradient[im + ic]);
                                    if ((im + ic < pdGradient.Count) && (Math.Abs(pdGradient[ig + ic] - pdGradient[im + ic]) > diffLimit))
                                    {
                                        matchFail = true;
                                        break;
                                    }
                                    matchCnt++;
                                }
                                if (matchFail || (im == pdGradient.Count - 1))
                                {
                                    if (matchCnt > 8)
                                    {
                                        if (rStart == start)
                                            removed.Add(new Point(rStart, start + ig));
                                        rStart = start + ig;
                                        rEnd = start + im + ic - 1;
                                        removed.Add(new Point(rStart, rEnd));
                                        pixelPath[rStart].isEdge = true;
                                        pixelPath[rEnd].isEdge = true;
                                        if (logSplit) Logger.Trace("      Find pattern match  count:{0}  {1}  {2}", matchCnt, rStart, rEnd);
                                    }
                                    ig = im + ic;
                                    matchCnt = 0;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            inc++;
                            continue;
                        }
                    }
                    inc++;
                    if (inc > 8)
                        break;
                    match = 1;
                }
                inc = 2;
                match = 1;
            }
            if (rEnd != end)
            {
                removed.Add(new Point(rEnd, end));
            }

            //    if (log) Logger.Trace("       (double)pdRangeMin / Math.Abs(pdMax):{0:0.00}  {1}   {2}", (double)pdRangeMin / Math.Abs(pdMax), pdRangeMin, pdMax);
            if (log) Logger.Trace("     CheckDiagonalSegemnts nr:{0,3} is split 2      {1,4}  {2,4}  {3,15}  {4,15}  shapeErr:{5:0.00}", nr, start, end, pixelPath[start].p, pixelPath[end].p, shapeErr);
            //     if (log) Logger.Trace("       Error:{0:0.00}   pdMin:{1}  pdMax:{2}  min/max:{3:0.00}", shapeErr, pdMin, pdMax, (double)pdMin / pdMax);
            if (log) Logger.Trace("       raw distance  p:{0}  {1}", pixelPath[start].p, distance);
            if (log) Logger.Trace("       raw gradient  p:{0}  {1}", pixelPath[end].p, gradient);
            //      if (log) Logger.Trace("       pdMin:{0}  pdmax:{1}  pdrange:{2}  diff01:{3}", pdMin, pdMax, Math.Abs(pdMax - pdMin), diff01Max);
            return removed;
        }





        private static List<int> pdDistance = new List<int>();
        private static List<int> pdGradient = new List<int>();


        private static int ApplyArcs(int start, int end, int nr)
        {
            int pdLimitArc = (int)(2 * pixelScale * pixelScale);     //PerpendicularDistanceInt
            int radiusVariation = (int)(2 * pixelScale);
            int scanRange = 50;// pixelOrig.Count/2;

            int pdMaxIndex = GetIndexOfMaxPerpendicularDistanceInt(start, end, out int pdMax);
            Point pStart = pixelOrig[Geti(start)];      // pixelPath[Geti(start)].p;
            Point pEnd = pixelOrig[Geti(end)];          // pixelPath[Geti(end)].p;
            Point pCenter = FindCircleCenter(pStart, pEnd, pixelPath[Geti(pdMaxIndex)].p, out double radius);
            //    Logger.Trace("  arc 1 start:{0}  end:{1}  center:{2}  radius:{3:0}  chord:{4:0}", start, end, pCenter, radius, GetDistance(pStart, pEnd));
            Point nEnd, nCenter;
            int newEndIndex = end;
            string logtxt = "";
            //    int nEnd = end;
            if (!pixelPath[Geti(end + 1)].isEdge)   // extenc arc
            {
                for (int i = end + 1; i < pixelOrig.Count; i++)
                {
                    if (pixelPath[Geti(i)].isEdge)
                    {
                        nEnd = pixelOrig[Geti(i)];
                        nCenter = FindCircleCenter(pStart, nEnd, pixelPath[Geti(pdMaxIndex)].p, out double nradius);
                        //       Logger.Trace("  arc 2 s:{0}  e:{1}  center:{2}  radius:{3:0}", start, i, nCenter, nradius);
                        if ((GetDistance(pCenter, nCenter) < radiusVariation) && (Math.Abs(radius - nradius) < radiusVariation))
                        {
                            if (log) logtxt = string.Format(" extend end from {0} to {1} ", end, i);
                            newEndIndex = end = i;
                            pEnd = nEnd;
                        }
                        break;
                    }
                }
            }
            //       if (log) Logger.Trace("     ApplyArcs nr:{0}  start:{1}  end:{2}    pdmax:{3:0.0}  pdmaxi:{4} center:{5}  radius:{6:0}  chord:{7:0}  {8}", nr, start, end, pdMax, pdMaxIndex, pCenter, radius, GetDistance(pStart, pEnd), logtxt);

            pdMaxIndex = GetIndexOfMaxPerpendicularDistanceInt(start, end, out pdMax);

            {
                if (pdMax <= pdLimitArc)
                {
                    if (log) logtxt += string.Format(" pdmax:{0,4:0} <= pdLimitArc:{1,4}  pdmaxi:{2,4}  radius:{3,4:0}", pdMax, pdLimitArc, pdMaxIndex, radius);
                    pdMaxIndex = GetIndexOfMaxPerpendicularDistanceInt(start - 1, end + 1, out pdMax);
                    if (log) logtxt += string.Format("  redo  pdmax:{0,4:0}  pdmaxi:{1,4}", pdMax, pdMaxIndex);
                    int irange = end - start;
                    //    if (log) Logger.Trace("     adapt i d1:{0}  d2:{1}  range;{2}", (pdMaxIndex - start), (end - pdMaxIndex), (irange / 4));
                    if (((pdMaxIndex - start) < (irange / 4)) || ((end - pdMaxIndex) < (irange / 4)))
                    { pdMaxIndex = start + irange / 2; }
                }

                //    Point pCenter = FindCircleCenter(pStart, pEnd, pixelPath[Geti(pdMaxIndex)].p, out double radius);
                //    if (log) Logger.Trace("     Analyze arc p1:{0}  p2:{1}  p3:{2}   radius:{3:0}   center:{4}", pStart, pEnd, pixelPath[Geti(pdMaxIndex)].p, radius, pCenter);

                if (double.IsNaN(radius) || double.IsInfinity(radius))
                {
                    MarkToRemovePointsBetween(start, end);          // convert to diagonale by removing all points in range                                                             //    break;
                    if (log) Logger.Trace("     ApplyArcs nr:{0,3}  start:{1,4}  end:{2,4}    pdmax:{3:0.0}  pdmaxi:{4} center:{5,15}  radius:{6,4:0}  !!! radius is NaN or infinite", nr, start, end, pdMax, pdMaxIndex, pCenter, radius);
                    return newEndIndex;
                }
                if (radius > 20000)
                {
                    MarkToRemovePointsBetween(start, end);          // convert to diagonale by removing all points in range                                                             //    break;
                    if (log) Logger.Trace("     ApplyArcs nr:{0,3}  start:{1,4}  end:{2,4}    pdmax:{3:0.0}  pdmaxi:{4} center:{5,15}  radius:{6,4:0}  !!! radius too big", nr, start, end, pdMax, pdMaxIndex, pCenter, radius);
                    return newEndIndex;
                }
                else
                {
                    if (log) BitmapMarkArc(pCenter, pStart, pEnd, Color.Green, Color.GreenYellow);

                    double ang1 = GetAngle2(pCenter, pixelPath[Geti(start)].p);
                    double ang2 = GetAngle2(pCenter, pixelPath[Geti(start + 2)].p);
                    double ang3 = GetAngle2(pCenter, pixelPath[Geti(end)].p);
                    int dcent, dmax;
                    double angLimit = Math.PI / 9;
                    //     if (log) Logger.Trace("     Analyze Adapt  {0}  {1}   ang1:{2:0.0}   ang3:{3:0.0}   Limit:{4:0.0}   pdmax:{5:0.0}  pdmaxi:{6}", start, end, 180 * ang1 / Math.PI, 180 * ang3 / Math.PI, 180 * angLimit / Math.PI, pdMax, pdMaxIndex);


                    /* if start or end-angle close to 0°,90°,180°,270° adapt p1/p2 coordinate to get round angle */
                    bool angleAdapted = false;
                    if ((Math.Abs(ang1 - Math.PI / 2) < angLimit) ||
                        (Math.Abs(ang1 - 3 * Math.PI / 2) < angLimit))
                    {
                        if (edges.ContainsKey(Geti(start - 1)) && (pixelPath[Geti(start - 1)].p.Y == pixelPath[Geti(start)].p.Y))
                        {
                            dmax = Math.Abs(edges[Geti(start)].X - pixelPath[Geti(start - 1)].p.X);
                            dcent = Math.Abs(edges[Geti(start)].X - pCenter.X);
                            if (dcent <= dmax / 2)
                            {
                                pixelPath[Geti(start)].p.X = pCenter.X;
                            }
                            else
                            {
                                pixelPath[Geti(start)].p.X = (edges[Geti(start)].X + edges[Geti(start - 1)].X) / 2;
                            }
                            if (log) logtxt += string.Format(" # Adapt start-x   {0}  {1}", start, end);
                            angleAdapted = true;
                        }
                    }

                    if ((Math.Abs(ang3 - Math.PI / 2) < angLimit) ||
                        (Math.Abs(ang3 - 3 * Math.PI / 2) < angLimit))
                    {
                        if (edges.ContainsKey(Geti(end + 1)) && (pixelPath[Geti(end + 1)].p.Y == pixelPath[Geti(end)].p.Y))
                        {
                            dmax = Math.Abs(edges[Geti(end)].X - pixelPath[Geti(end + 1)].p.X);
                            dcent = Math.Abs(edges[Geti(end)].X - pCenter.X);
                            if (dcent <= dmax / 2)
                            {
                                pixelPath[Geti(end)].p.X = pCenter.X;
                            }
                            else
                            {
                                pixelPath[Geti(end)].p.X = (edges[Geti(end)].X + edges[Geti(end + 1)].X) / 2;
                            }
                            if (log) logtxt += string.Format(" # Adapt end-x  {0}  {1}", start, end);
                            angleAdapted = true;
                        }
                    }

                    if ((Math.Abs(ang1) < angLimit) ||
                        (Math.Abs(ang1 - Math.PI) < angLimit) ||
                        (Math.Abs(ang1 - 2 * Math.PI) < angLimit))
                    {
                        if (edges.ContainsKey(Geti(start - 1)) && (pixelPath[Geti(start - 1)].p.X == pixelPath[Geti(start)].p.X))
                        {
                            dmax = Math.Abs(edges[Geti(start)].Y - pixelPath[Geti(start - 1)].p.Y);
                            dcent = Math.Abs(edges[Geti(start)].Y - pCenter.Y);
                            if (dcent <= dmax / 2)
                            {
                                pixelPath[Geti(start)].p.Y = pCenter.Y;
                            }
                            else
                            {
                                pixelPath[Geti(start)].p.Y = (edges[Geti(start)].Y + edges[Geti(start - 1)].Y) / 2;
                            }
                            if (log) logtxt += string.Format(" # Adapt start-y {0}  {1}", start, end);
                            angleAdapted = true;
                        }
                    }

                    if ((Math.Abs(ang3) < angLimit) ||
                        (Math.Abs(ang3 - Math.PI) < angLimit) ||
                        (Math.Abs(ang3 - 2 * Math.PI) < angLimit))
                    {
                        if (edges.ContainsKey(Geti(end + 1)) && (pixelPath[Geti(end + 1)].p.X == pixelPath[Geti(end)].p.X))
                        {
                            dmax = Math.Abs(edges[Geti(end)].Y - pixelPath[Geti(end + 1)].p.Y);
                            dcent = Math.Abs(edges[Geti(end)].Y - pCenter.Y);
                            if (dcent <= dmax / 2)
                            {
                                pixelPath[Geti(end)].p.Y = pCenter.Y;
                            }
                            else
                            {
                                pixelPath[Geti(end)].p.Y = (edges[Geti(end)].Y + edges[Geti(end + 1)].Y) / 2;
                            }
                            if (log) logtxt += string.Format(" # Adapt end-y  {0}  {1}", start, end);
                            angleAdapted = true;
                        }
                    }

                    pStart = pixelPath[Geti(start)].p;
                    pEnd = pixelPath[Geti(end)].p;


                    //                  if (false && !angleAdapted && (radius > 10 * pixelScale))
                    if ((radius > 10 * pixelScale))
                    {
                        double ang0;
                        ang1 = GetAngle2(pixelOrig[Geti(start + 2)], pixelOrig[Geti(start)]);
                        double angDiff;
                        int ostart = start;
                        for (int ti = start - 1; ti > start - scanRange; ti--)
                        {
                            if (Geti(start) < Geti(end))
                                break;
                            FindCircleCenter(pixelOrig[Geti(ti)], pEnd, pixelOrig[Geti(pdMaxIndex)], out double rp1);
                            ang0 = GetAngle2(pixelOrig[Geti(ti + 2)], pixelOrig[Geti(ti)]);
                            angDiff = Math.Abs(ang0 - ang1);
                            if (angDiff > Math.PI) { angDiff -= Math.PI * 2; }
                            if ((Math.Abs(radius - rp1) < radiusVariation) && (Math.Abs(angDiff) < Math.PI / 6))
                            {
                                if (iRemove.Contains(Geti(ti)))
                                    iRemove.Remove(Geti(ti));
                                pixelPath[Geti(start)].isEdge = false;
                                start = ti;
                                pStart = pixelOrig[Geti(start)];
                                ang1 = ang0;
                                continue;
                            }
                            break;
                            //    Logger.Trace("start radius - rp1:{0}  radiusVariation:{1}  angDiff:{2}", Math.Abs(radius - rp1), radiusVariation, angDiff * 180 / Math.PI);
                        }
                        if (log) logtxt += string.Format(" # Extend start {0} {1}", ostart, start);
                        pixelPath[Geti(start)].isEdge = true;

                        ang1 = GetAngle2(pixelOrig[Geti(end - 2)], pixelOrig[Geti(end)]);
                        int oend = end;
                        for (int ti = end + 1; ti < end + scanRange; ti++)
                        {
                            if (Geti(end) > Geti(start))
                                break;
                            FindCircleCenter(pixelOrig[Geti(ostart)], pixelOrig[Geti(ti)], pixelOrig[Geti(pdMaxIndex)], out double rp2);
                            ang0 = GetAngle2(pixelOrig[Geti(ti - 2)], pixelOrig[Geti(ti)]);
                            angDiff = Math.Abs(ang0 - ang1);
                            if (angDiff > Math.PI) { angDiff -= Math.PI * 2; }
                            if ((Math.Abs(radius - rp2) < radiusVariation) && (Math.Abs(angDiff) < Math.PI / 6))
                            {
                                if (iRemove.Contains(Geti(ti)))
                                    iRemove.Remove(Geti(ti));
                                pixelPath[Geti(end)].isEdge = false;
                                end = ti;
                                pEnd = pixelOrig[Geti(end)];
                                ang1 = ang0;
                                continue;
                            }
                            //    Logger.Trace("end radius - rp2:{0}  radiusVariation:{1}  angDiff:{2}", Math.Abs(radius - rp2), radiusVariation, angDiff*180/Math.PI);
                            break;
                        }
                        if (log) logtxt += string.Format(" # Extend end {0} {1}", oend, end);
                        pixelPath[Geti(end)].isEdge = true;
                    }
                    if (Geti(start) != Geti(end))
                        pCenter = FindCircleCenter(pStart, pEnd, pixelOrig[Geti(pdMaxIndex)], out radius);

                    if (double.IsNaN(radius) || double.IsInfinity(radius))
                    {
                        MarkToRemovePointsBetween(start, end);          // convert to diagonale by removing all points in range                                                             //    break;
                        if (log) Logger.Trace("     ApplyArcs nr:{0,3}  start:{1}  end:{2}    pdmax:{3,4:0}  pdmaxi:{4,4} center:{5,15}  radius:{6,4:0}  {7:0}  !!! radius is NaN or infinite", nr, start, end, pdMax, pdMaxIndex, pCenter, radius, logtxt);
                        return newEndIndex;
                    }

                    if (log) BitmapMarkArc(pCenter, pStart, pEnd, Color.BlueViolet, Color.LightBlue);
                    if (log) Logger.Trace("     ApplyArcs nr:{0,3}  start:{1,4}  end:{2,4}    pdmax:{3,4:0}  pdmaxi:{4,4} center:{5,15}  radius:{6,4:0}  chord:{7:0}  {8}", nr, start, end, pdMax, pdMaxIndex, pCenter, radius, GetDistance(pStart, pEnd), logtxt);
                    ArrangePointsOnArc(pCenter, radius, start, end);
                }
            }
            return newEndIndex;
        }

        private static Point FindCircleCenter(Point p1, Point p2, Point p3, out double r)
        {
            // https://stackoverflow.com/questions/62488827/solving-equation-to-find-center-point-of-circle-from-3-points
            //    NumberFormatInfo setPrecision = new NumberFormatInfo();
            //    setPrecision.NumberDecimalDigits = 3; // 3 digits after the double point

            int x1 = p1.X, y1 = p1.Y;
            int x2 = p2.X, y2 = p2.Y;
            int x3 = p3.X, y3 = p3.Y;

            int x12 = x1 - x2;
            int x13 = x1 - x3;

            int y12 = y1 - y2;
            int y13 = y1 - y3;

            int y31 = y3 - y1;
            int y21 = y2 - y1;

            int x31 = x3 - x1;
            int x21 = x2 - x1;

            double sx13 = (double)(Math.Pow(x1, 2) - Math.Pow(x3, 2));
            double sy13 = (double)(Math.Pow(y1, 2) - Math.Pow(y3, 2));
            double sx21 = (double)(Math.Pow(x2, 2) - Math.Pow(x1, 2));
            double sy21 = (double)(Math.Pow(y2, 2) - Math.Pow(y1, 2));

            double f = ((sx13) * (x12)
                    + (sy13) * (x12)
                    + (sx21) * (x13)
                    + (sy21) * (x13))
                    / (2 * ((y31) * (x12) - (y21) * (x13)));
            double g = ((sx13) * (y12)
                    + (sy13) * (y12)
                    + (sx21) * (y13)
                    + (sy21) * (y13))
                    / (2 * ((x31) * (y12) - (x21) * (y13)));

            double c = -(double)Math.Pow(x1, 2) - (double)Math.Pow(y1, 2) -
                                        2 * g * x1 - 2 * f * y1;
            double h = -g;
            double k = -f;
            double sqr_of_r = h * h + k * k - c;
            r = Math.Sqrt(sqr_of_r);
            return new Point((int)h, (int)k);
        }


        private static void ArrangePointsOnArc(Point pCenter, double radius, int start, int end)
        {
            if (radius < 2 * pixelScale)
                return;

            int cnt = end - start;
            double dx = pCenter.X - pixelPath[Geti(start)].p.X;
            double dy = pCenter.Y - pixelPath[Geti(start)].p.Y;

            double ang1 = GetAngle2(pCenter, pixelPath[Geti(start)].p);
            double angx = GetAngle2(pCenter, pixelPath[Geti(start + 2)].p);
            double ang2 = GetAngle2(pCenter, pixelPath[Geti(end)].p);
            if (Math.Abs(angx - ang1) > Math.PI)
            {
                if (ang1 > Math.PI)
                    ang1 -= Math.PI * 2;
                else
                    angx -= Math.PI * 2;
            }
            //    else if ((angx - ang1) < Math.PI)
            //      angx += Math.PI * 2;
            if (angx < ang1)
            { if (ang2 > ang1) ang2 -= 2 * Math.PI; }
            else
            { if (ang2 < ang1) ang2 += 2 * Math.PI; }

            if (pixelPath[Geti(start)].p == pixelPath[Geti(end)].p) // full circle
            {
                if (log) Logger.Trace(" FULL CIRCLE: r:{0}  orig dimension x:{1}  y:{2}", radius, traceMaxX - traceMinX, traceMaxY - traceMinY);
                ang2 += 2 * Math.PI;
                movePixel(start, ang1);
            }

            double delta = ang2 - ang1;
            double step = delta / cnt;
            //       if (log) Logger.Trace("   center:{0}  radius:{1,5:0}  a1:{2,6:0.0}   a2:{3,6:0.0}  step:{4,6:0.00}", pCenter, radius, ang1 * 180 / Math.PI, ang2 * 180 / Math.PI, step * 180 / Math.PI);
            Point orig;
            int dist = 0;

            /* adjust pixelPath coordinates to arc positions and calc. deviation (old/new pixel-pos) */
            for (int ia = start + 1; ia < end; ia++)
            {
                orig = pixelPath[Geti(ia)].p;
                ang1 += step;
                movePixel(ia, ang1);

                // better check radius deviation
                dist += (int)Math.Abs(GetDistance(pCenter, orig) - GetDistance(pCenter, pixelPath[Geti(ia)].p));
            }
            void movePixel(int indx, double ang)
            {
                pixelPath[Geti(indx)].p.X = pCenter.X + (int)(radius * Math.Cos(ang));
                pixelPath[Geti(indx)].p.Y = pCenter.Y + (int)(radius * Math.Sin(ang));
                pixelPath[Geti(indx)].isEdge = true;
            }
            //segment, section
            //      if (log) Logger.Trace("   shift:{0}   avg shift:{1}", dist, dist / cnt);
            //  int maxd = (int)Math.Max(GetDistance(pixelPath[Geti(start)].p, pixelPath[Geti(end)].p) / 10, pixelScale * pixelScale);
            /* if deviation is too high... */
            int maxd = 3 * pixelScale;// * pixelScale;
            if ((dist / cnt) > maxd)
            {
                Logger.Trace("     ArrangePointsOnArc high shift {0}, try bezier: i1:{1} p1:{2}   i2:{3}  p2:{4}   range:{5}  shift:{6}   max:{7}", (dist / cnt), start, pixelPath[start].p, end, pixelPath[end].p, end - start, dist, maxd);
                List<Point> pixelPathPartial = new List<Point>();
                for (int l = start; l < end; l++)
                {
                    pixelPathPartial.Add(pixelOrig[l]);
                }
                List<Point> secPoint = DouglasPeuckerReduction(pixelPathPartial, 20);
                Logger.Trace("    DouglasPeuckerReduction  {0}  {1}", pixelPathPartial.Count, secPoint.Count);
                /* show DouglasPeucker result */
                /*    int ii = 0;
                    for (int k = start; k < end; k++)
                    {
                        if (ii < secPoint.Count)
                            pixelPath[k].p = secPoint[ii++];
                        else
                            pixelPath[k].p = secPoint[secPoint.Count-1];
                        pixelPath[k].isEdge = true;
                    }*/

                ApplyBezierFitCurveSection(secPoint, start, end, 10);/* adjust original pixelPath coordinates to match resultPath*/

                /* check deviation again */
                dist = 0;
                for (int ia = start + 1; ia < end; ia++)
                {
                    dist += (int)GetDistance(pixelOrig[ia], pixelPath[Geti(ia)].p);
                }

                if ((dist / cnt) > 5 * pixelScale * pixelScale)
                {
                    Logger.Trace("     ArrangePointsOnArc high shift {0}, better keep orig coordinates: i1:{1} p1:{2}   i2:{3}  p2:{4}   range:{5}  shift:{6}   max:{7}", (dist / cnt), start, pixelPath[start].p, end, pixelPath[end].p, end - start, dist, maxd);
                    //MarkToRemovePointsBetween(start, end);
                    for (int k = start; k < end; k++)
                    {
                        pixelPath[k].p = pixelOrig[k];
                        pixelPath[k].isEdge = true;
                    }

                }
            }
        }


        private static Point GetPointBetween(Point a, Point b)
        {
            return new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }

        private static float GetDistance(Point a, Point b)
        {
            if ((a.X - b.X) == 0) { return (float)Math.Abs(a.Y - b.Y); }
            if ((a.Y - b.Y) == 0) { return (float)Math.Abs(a.X - b.X); }
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
        private static float GetDistance(PointF a, PointF b)
        {
            if ((a.X - b.X) == 0) { return (float)Math.Abs(a.Y - b.Y); }
            if ((a.Y - b.Y) == 0) { return (float)Math.Abs(a.X - b.X); }
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        private static double GetAngle2(PointF p1, PointF p2)
        {
            double tmp;
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            double diff = Math.Atan2(dy, dx);
            if (diff < 0) { diff += (2 * Math.PI); }
            return diff;
        }

        private static double CalcAngleDiff(double a0, double a1)
        {
            double diff = a1 - a0;
            if (diff > Math.PI) { diff -= (2 * Math.PI); }
            else if (diff < -Math.PI) { diff += (2 * Math.PI); }
            return diff;
        }

        public static List<Point> DouglasPeuckerReduction(List<Point> points, double epsilon)
        {
            if (points == null || points.Count < 3) return new List<Point>();// points;

            int firstPoint = 0;
            int lastPoint = points.Count - 1;
            List<int> pointIndexsToKeep = new List<int>();

            //Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            //The first and the last point cannot be the same
            while (points[firstPoint].Equals(points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReduction(points, firstPoint, lastPoint, epsilon, ref pointIndexsToKeep);
            List<Point> returnPoints = new List<Point>();
            pointIndexsToKeep.Sort();
            foreach (int index in pointIndexsToKeep)
            {
                returnPoints.Add(points[index]);
            }

            return returnPoints;
        }


        private static void DouglasPeuckerReduction(List<Point> points, int firstPoint, int lastPoint, double tolerance, ref List<int> pointIndexsToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            for (int index = firstPoint; index < lastPoint; index++)
            {
                double distance = PerpendicularDistance
                    (points[firstPoint], points[lastPoint], points[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(points, firstPoint,
                indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest,
                lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        public static double PerpendicularDistance(PointF Point1, PointF Point2, PointF Point)
        {
            double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X * Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X * Point2.Y - Point1.X * Point.Y));
            double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) + Math.Pow(Point1.Y - Point2.Y, 2));
            double height = area / bottom * 2;
            return height;
        }

        public static int PerpendicularDistanceInt(PointF Point1, PointF Point2, PointF Point)
        {
            double area = (.5 * (Point1.X * Point2.Y + Point2.X * Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X * Point2.Y - Point1.X * Point.Y));
            double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) + Math.Pow(Point1.Y - Point2.Y, 2));
            int height = (int)(area / bottom * 2 * pixelScale);
            return height;
        }

        public static int GetIndexOfMaxPerpendicularDistanceInt(int start, int end, out int hmax)
        {
            Point p1 = pixelPath[Geti(start)].p;
            Point p2 = pixelPath[Geti(end)].p;
            int h0, imax = start + 1;
            hmax = 0;
            for (int ih = start + 1; ih < end; ih++)
            {
                h0 = Math.Abs(PerpendicularDistanceInt(p1, p2, pixelPath[Geti(ih)].p));
                if (h0 > hmax) { imax = ih; hmax = h0; }
            }
            return imax;
        }

        private static void ApplyBezierFitCurveSection(List<Point> secP, int start, int end, double error)
        {
            bool logBezier = false;
            List<Point> resultPath = new List<Point>();
            System.Windows.Point[] sourcePoints;
            sourcePoints = new System.Windows.Point[secP.Count];// end - start + 1];
            if (logBezier) Logger.Trace("start {0}   end {1}", start, end);
            int ji = 0;
            for (int j = 0; j < secP.Count; j++)
            {
                sourcePoints[ji++] = new System.Windows.Point(secP[j].X, secP[j].Y);
                if (logBezier) Logger.Trace("sourcePoints {0}  {1}  {2}", ji, j, secP[j]);
            }
            if (ji > 1)
            {
                List<System.Windows.Point> result = FitCurves.FitCurve(sourcePoints, error);       // convert to bezier

                System.Windows.Point first = new System.Windows.Point(pixelPath[start].p.X, pixelPath[start].p.Y);
                result.Insert(0, first);
                if (logBezier) Logger.Trace("FitCurve {0}  ", result.Count);
                foreach (System.Windows.Point p in result)
                {
                    if (logBezier) Logger.Trace("  FitCurve {0}  ", p);
                }

                AdjustControlPoints(result, pixelPath, start, end);

                foreach (System.Windows.Point p in result)
                {
                    if (Double.IsNaN(p.X) || Double.IsNaN(p.Y))
                    {
                        Logger.Error("ApplyBezier NaN index {0} ({1})   to {2} ({3})", start, pixelPath[start].p, end, pixelPath[end].p);
                        continue;
                    }
                }

                if (result.Count >= 4)
                {
                    int rest;

                    /* Get line-segment-coordinates from bezier control points (via 'AddResultPath' into resultPath) */
                    for (int c = 0; c < result.Count - 1; c += 3)
                    {
                        if (logBezier) Logger.Trace("cubic {0}  {1}  {2}  {3}", result[c], result[c + 1], result[c + 2], result[c + 3]);
                        ImportMath.CalcCubicBezier(result[c], result[c + 1], result[c + 2], result[c + 3], AddResultPath, "");
                        rest = result.Count - c;
                        if (rest < 3)
                        {
                            break;
                        }
                    }
                    if (logBezier) Logger.Trace("CalcCubicBezier ready  resultPath.Count:{0}  pixelpath:{1}", resultPath.Count, (end - start));
                    int ri = 0;
                    double step = (double)resultPath.Count / (end - start);
                    double ci = 0;

                    /* adjust original pixelPath coordinates to match resultPath - no add or remove of pixelPath items! */
                    for (int z = start + 1; z < end; z++)
                    {
                        if ((int)ci < resultPath.Count)
                        {
                            pixelPath[z].p = new Point((int)resultPath[(int)ci].X, (int)resultPath[(int)ci].Y);
                            pixelPath[z].isEdge = true;
                        }
                        ci += step;
                    }
                }
            }

            System.Windows.Point toPointD(Point p)
            { return new System.Windows.Point(p.X, p.Y); }

            void AddResultPath(System.Windows.Point p, string cmt)
            {
                resultPath.Add(new Point((int)p.X, (int)p.Y));
            }
        }

        private static void AdjustControlPoints(List<System.Windows.Point> result, List<PxProp> pixelPath, int start, int end)
        {
            /* 	adjust 1st and last control point to get smooth transition from isEdge point */
            bool logBezier = false;
            if (start > 0)
            {
                PointF p0 = new PointF((float)result[0].X, (float)result[0].Y); // should be same as pixelPath[start]
                PointF p1 = new PointF((float)result[1].X, (float)result[1].Y);
                PointF ps = new PointF(pixelPath[start - 1].p.X, pixelPath[start - 1].p.Y);
                double br = GetDistance(p0, p1);
                double ba = GetAngle(p0, p1);
                double bsa = GetAngle(ps, p0);
                if (logBezier) Logger.Trace("  p0:{0}  p1:{1}  ps:{2}", p0, p1, ps);
                if (logBezier) Logger.Trace("adjust1 start {0}  aEdge;{1:0.00}   aCtrl:{2:0..00}    r:{3:0.00}", p0, bsa * 180 / Math.PI, ba * 180 / Math.PI, br);
                if (Math.Abs(bsa - ba) < Math.PI / 4)       // straight line
                {
                    double newvalx = br / 2 * Math.Cos(bsa);      //dx * Math.Cos(bsa) - dy * Math.Sin(bsa);
                    double newvaly = br / 2 * Math.Sin(bsa);         //dx * Math.Sin(bsa) + dy * Math.Cos(bsa);
                    result[1] = new System.Windows.Point(newvalx + result[0].X, newvaly + result[0].Y);
                    p1 = new PointF((float)result[1].X, (float)result[1].Y);
                    br = GetDistance(p0, p1); ba = GetAngle(p0, p1);
                    if (logBezier) Logger.Trace("adjust2 start {0}  aEdge;{1:0.00}   aCtrl:{2:0..00}    r:{3:0.00}", p0, bsa * 180 / Math.PI, ba * 180 / Math.PI, br);
                }
            }
            if (end < pixelPath.Count)
            {
                PointF p0 = new PointF((float)result[result.Count - 1].X, (float)result[result.Count - 1].Y); // should be same as pixelPath[start]
                PointF p1 = new PointF((float)result[result.Count - 2].X, (float)result[result.Count - 2].Y);
                Point ps;
                if (end < pixelPath.Count - 1)
                    ps = new Point(pixelPath[end + 1].p.X, pixelPath[end + 1].p.Y);
                else
                    ps = new Point(pixelPath[1].p.X, pixelPath[1].p.Y);
                double br = GetDistance(p0, p1);
                double ba = GetAngle(p0, p1);
                double bsa = GetAngle(ps, p0);
                if (logBezier) Logger.Trace("adjust1 end {0}  aEdge;{1:0.00}   aCtrl:{2:0..00}    r:{3:0.00}", p0, bsa * 180 / Math.PI, ba * 180 / Math.PI, br);
                if (Math.Abs(bsa - ba) < Math.PI / 4)       // straight line
                {
                    double newvalx = br / 2 * Math.Cos(bsa);      //dx * Math.Cos(bsa) - dy * Math.Sin(bsa);
                    double newvaly = br / 2 * Math.Sin(bsa);         //dx * Math.Sin(bsa) + dy * Math.Cos(bsa);
                    result[result.Count - 2] = new System.Windows.Point(newvalx + result[result.Count - 1].X, newvaly + result[result.Count - 1].Y);
                    p1 = new PointF((float)result[result.Count - 2].X, (float)result[result.Count - 2].Y);
                    br = GetDistance(p0, p1); ba = GetAngle(p0, p1);
                    if (logBezier) Logger.Trace("adjust2 end {0}  aEdge;{1:0.00}   aCtrl:{2:0..00}    r:{3:0.00}", p0, bsa * 180 / Math.PI, ba * 180 / Math.PI, br);
                }
            }
        }

        private static double GetAngle(PointF p1, PointF p2)
        {
            double tmp;
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            return Math.Atan2(dy, dx);
        }

    }
}