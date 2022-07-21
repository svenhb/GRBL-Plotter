﻿/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2018-2021 Sven Hasemann contact: svenhb@web.de

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
/* 2018-11  new
 * 2019-04-22 improove function simplifyContour
 * 2021-07-02 code clean up / code quality
 */

// https://www.iis.sinica.edu.tw/papers/fchang/1362-F.pdf
// http://www.imageprocessingplace.com/downloads_V3/root_downloads/tutorials/contour_tracing_Abeer_George_Ghuneim/alg.html

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

#pragma warning disable IDE0059

namespace GrblPlotter
{
    static class Vectorize
    {
        private static int Width, Height;
        private static float penRadius;
        private static short toolNr;
        private static short[,] bitmap;
        private const short markObject = short.MaxValue;
        private static readonly short markBackground = short.MinValue;
        private static readonly List<List<PointF>> outlinePaths = new List<List<PointF>>();
        private static int Smooth = 0;
        public static StringBuilder logList = new StringBuilder();
        public static bool log = false;
        public static bool shrinkPath = false;

        /// <summary>
        /// Get a List of PointF-Lists from paths found in sbyte-array
        /// </summary>
        public static List<List<PointF>> GetPaths(short[,] bitm, int sizeX, int sizeY, short toolN, int smooth, float pradius, bool shrink)
        {
            Width = sizeX;
            Height = sizeY;
            toolNr = toolN;
            Smooth = smooth;
            penRadius = pradius;      // width in pixels
            shrinkPath = shrink;
            bitmap = new short[Width, Height];
            for (int y = 0; y < Height; y++)    // make working copy
                for (int x = 0; x < Width; x++)
                { bitmap[x, y] = bitm[x, y]; }

            outlinePaths.Clear();       // reset all paths
            StartTracing();             // find paths
            bitmap = new short[1, 1];   // free memory
            return outlinePaths;
        }

        private static int abortCount = 1000;     // watch dog to escape from do-while

        /// <summary>
        /// Scan line by line for 0/1 transient to start contour tracing
        /// </summary>
        private static void StartTracing()
        {
            logList.Clear();
            bool isObjectLast, isObjectCur;
            abortCount = Width * Height;        // maximum pixels to trace
            int cnt = 1;

            for (int y = 0; y < Height; y++)
            {
                isObjectLast = false; isObjectCur = false;
                for (int x = 0; x < Width; x++)
                {
                    if (bitmap[x, y] >= markObject) { isObjectCur = true; }    // already found as object, nothing to do
                    else if (bitmap[x, y] <= markBackground) { isObjectCur = false; }    // already found as background, nothing to do
                    else if (bitmap[x, y] == toolNr)                            // object found, 0/1 transient = start point
                    {
                        bitmap[x, y] = markObject;                                    // mark as found
                        if (!isObjectLast)
                        {
                            logList.AppendFormat("startTracing-CW  ({0}) at {1} ; {2}\r\n", cnt++, x, y);
                            TraceContour(x, y, true);                           // find contour CW
                        }
                        isObjectCur = true;
                    }
                    else
                    {
                        bitmap[x, y] = markBackground;                                   // must be background
                        isObjectCur = false;                                    // mark as background
                        if (isObjectLast)                                       // found 1/0 transient
                        {                                                       // edge is upper left
                            logList.AppendFormat("startTracing-CCW ({0}) at {1} ; {2}\r\n", cnt++, (x - 1), (y - 0));
                            TraceContour(x - 1, y - 0, false);                      // find contour with last object x, CCW
                        }
                    }
                    isObjectLast = isObjectCur;
                }
            }
        }

        private const int searchOffset = 5;
        /// <summary>
        /// Create list with contour points starting at x,y
        /// </summary>
        private static void TraceContour(int x, int y, bool isCW)
        {
            int searchDirCur = 1;                       // if CW  start with 6 (1+searchOffset)
            if (!isCW) searchDirCur = 3;                // if CCW start with 0 (3+searchOffset)
            int searchDirLast = searchDirCur;

            Point start1 = new Point(x, y);         // 1st point in list = initial coord.
            Point start2 = new Point(x, y);         // 2nd point in list
            Point next = new Point(x, y);           // current found pixel coord
            Point last = new Point(x, y);           // last found pixel coord
            List<PointF> onePath = new List<PointF>();
            bool startFound = false;
            onePath.Add(start1);                        // store 1st point
            int cnt = 0;
            do
            {
                searchDirCur = TraceCheck(ref next, searchDirCur);
                startFound = ((last == start1) && (next == start2));            // stop when 1st and 2nd point repeats
                if (cnt == 0) { start2 = next; searchDirLast = searchDirCur; }  // store 2nd point for stopping condition

                if ((Smooth > 0) || (searchDirLast != searchDirCur))
                    onePath.Add(new Point(last.X, last.Y));             // only store point when direction changed      

                if (searchDirCur == -1)                                 // failure ?
                {
                    logList.AppendFormat("traceContour break at {0} ; {1}\r\n", next.X, next.Y);
                    break;
                }
                searchDirLast = searchDirCur;

                last = next;
                if (cnt++ > abortCount) { logList.Append("Abort\r\n"); return; }    // safety stop
            } while (!startFound);

            if (onePath.Count <= 3)     // 
                return;

            if (Smooth > 0)
                for (int k = 0; k < Smooth; k++)
                    SmoothContour(onePath);

            if (shrinkPath)
                ShrinkContour(onePath, penRadius);   // half width in pixels
            SimplifyContour(onePath, 0.2f);

            outlinePaths.Add(new List<PointF>(onePath));

            if (log) ShowList(onePath);
        }

        private static List<PointF> tmpList;
        /// <summary>
        /// Smooth points by calc. average of last, current and next point
        /// </summary>
        private static void SmoothContour(List<PointF> list)
        {
            if (list.Count < 10) return;    // too less points to smooth
            PointF a, b, c;
            a = list[0]; b = list[1]; c = list[2];
            float newX, newY;
            tmpList = new List<PointF>();
            for (int i = 1; i < list.Count - 1; i++)
            {
                a = list[i - 1]; b = list[i]; c = list[i + 1];
                newX = (a.X + b.X + c.X) / 3;
                newY = (a.Y + b.Y + c.Y) / 3;
                tmpList.Add(new PointF(newX, newY));
            }
            newX = (b.X + c.X + list[0].X) / 3;             // last point
            newY = (b.Y + c.Y + list[0].Y) / 3;
            tmpList.Add(new PointF(newX, newY));
            newX = (c.X + list[0].X + list[1].X) / 3;       // first point
            newY = (c.Y + list[0].Y + list[1].Y) / 3;
            tmpList.Add(new PointF(newX, newY));

            for (int i = 0; i < list.Count; i++)            // copy points to referenced list
            { list[i] = tmpList[i]; }
        }

        private static void ShrinkContour(List<PointF> list, float width)   // width in pixels
        {   // average of last end next vector [i-1]-[i] and [i]-[i+1]
            // move i 90 degree of vector
            if (list.Count < 3) return;     // too less points to shrink
            PointF a, b, c, p;
            a = list[0]; b = list[1]; c = list[2];

            float newX, newY;
            tmpList = new List<PointF>();
            for (int i = 1; i < list.Count - 1; i++)
            {
                a = list[i - 1]; b = list[i]; c = list[i + 1];
                p = CalcOffset(width, GetAngle(a, c) + Math.PI / 2);
                newX = b.X + p.X;
                newY = b.Y + p.Y;
                tmpList.Add(new PointF(newX, newY));
            }
            p = CalcOffset(width, GetAngle(b, list[0]) + Math.PI / 2);// last point
            newX = c.X + p.X;
            newY = c.Y + p.Y;
            tmpList.Add(new PointF(newX, newY));

            p = CalcOffset(width, GetAngle(c, list[1]) + Math.PI / 2);// first point
            newX = list[0].X + p.X;
            newY = list[0].Y + p.Y;
            tmpList.Add(new PointF(newX, newY));

            for (int i = 0; i < list.Count; i++)            // copy points to referenced list
            { list[i] = tmpList[i]; }

        }

        private static void SimplifyContour(List<PointF> list, float error)
        {
            if (list.Count < 5) return;    // too less points to simplify
            PointF delta1 = new PointF();
            PointF delta2 = new PointF();
            PointF p1 = new PointF();
            int i, k, j, cnt;
            p1 = list[list.Count - 1];
            for (i = list.Count - 2; i > 0; i--)
            {
                delta1.X = list[i].X - p1.X;
                delta1.Y = list[i].Y - p1.Y;
                delta2.X = list[i - 1].X - list[i].X;
                delta2.Y = list[i - 1].Y - list[i].Y;
                while (SameDirection(delta1, delta2, error))
                {
                    list.RemoveAt(i);
                    i--;
                    if (i > 2)
                    {
                        delta2.X = list[i - 1].X - list[i].X;
                        delta2.Y = list[i - 1].Y - list[i].Y;
                    }
                    else
                        break;
                }
                p1 = list[i];// oder delta1 = delta2
            }

            if (list.Count < 3) return;    // too less points to simplify
            float aX, minX, maxX, aY, minY, maxY;
            bool setMin, setMax;
            for (i = list.Count - 1; i > 0; i--)        // scan for useless Y
            {
                cnt = 1; aX = list[i].X;
                minY = maxY = list[i].Y;
                for (k = i - 1; k >= 0; k--)            // find min / max Y for same X      
                {
                    if (aX == list[k].X)                // same X?
                    {
                        cnt++;
                        minY = Math.Min(minY, list[k].Y);
                        maxY = Math.Max(maxY, list[k].Y);
                    }
                    else
                    { k++; break; }
                }
                if (cnt > 2)
                {
                    setMin = false; setMax = false;
                    int jend = i - cnt;
                    for (j = i; j > jend; j--)              // sort out inbetween values   
                    {
                        aY = list[j].Y;
                        if (!setMin && (aY == minY))        // keep 1st min
                        { setMin = true; continue; }
                        else if (!setMax && (aY == maxY))   // keep 1st max
                        { setMax = true; continue; }
                        list.RemoveAt(j);
                        i--;
                    }
                }
            }

            for (i = list.Count - 1; i > 0; i--)        // scan for useless X
            {
                cnt = 1; aY = list[i].Y;
                minX = maxX = list[i].X;
                for (k = i - 1; k >= 0; k--)            // find min / max X for same Y      
                {
                    if (aY == list[k].Y)                // same Y?
                    {
                        cnt++;
                        minX = Math.Min(minX, list[k].X);
                        maxX = Math.Max(maxX, list[k].X);
                    }
                    else
                    { k++; break; }
                }
                if (cnt > 2)
                {
                    setMin = false; setMax = false;
                    int jend = i - cnt;
                    for (j = i; j > jend; j--)              // sort out inbetween values   
                    {
                        aX = list[j].X;
                        if (!setMin && (aX == minX))        // keep 1st min
                        { setMin = true; continue; }
                        else if (!setMax && (aX == maxX))   // keep 1st max
                        { setMax = true; continue; }
                        list.RemoveAt(j);
                        i--;
                    }
                }
            }


        }
        private static bool SameDirection(PointF p1, PointF p2, float delta)
        {
            if ((Math.Abs(p1.X - p2.X) <= delta) && (Math.Abs(p1.Y - p2.Y) <= delta))
                return true;
            return false;
        }

        //   private static double GetDistance(PointF p1, PointF p2)
        //   { return Math.Sqrt((p1.X-p2.X)* (p1.X - p2.X) + (p1.Y - p2.Y)* (p1.Y - p2.Y)); }
        private static double GetAngle(PointF p1, PointF p2)
        {
            if ((p2.X - p1.X) == 0)
            {
                if ((p2.Y - p1.Y) > 0)
                    return Math.PI / 2;
                else
                    return -Math.PI / 2;
            }
            if ((p2.X - p1.X) > 0)
                return Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X));
            else
                return Math.PI + Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X));
        }
        private static PointF CalcOffset(float r, double a)
        {
            PointF tmp = new PointF
            {
                X = (float)(r * Math.Cos(a)),
                Y = (float)(r * Math.Sin(a))
            };
            return tmp;
        }
        private static void ShowList(List<PointF> list)
        {
            logList.Append("showList\r\n");
            foreach (PointF p in list)
            { logList.AppendFormat("{0} ; {1}\r\n", p.X, p.Y); }
        }

        // 5 6 7
        // 4 p 0
        // 3 2 1
        private static readonly short[][] searchDirection = { new short[] { 1, 0 }, new short[] { 1, 1 }, new short[] { 0, 1 }, new short[] { -1, 1 }, new short[] { -1, 0 }, new short[] { -1, -1 }, new short[] { 0, -1 }, new short[] { 1, -1 } }; // xy-dir by index
        // go through neighbor points and check direction to continue 0                     1                    2                      3                      4                      5
        private static int TraceCheck(ref Point p, int oldDir)
        {
            int newDir = (oldDir + searchOffset) % 8;
            int checkX, checkY;
            bool isObjectLast, isObjectCur;                 // store last and current 'color'
            if (log) logList.AppendFormat("traceCheck old dir {0}   {1} ; {2}    ", oldDir, p.X, p.Y);
            checkX = p.X + searchDirection[newDir][0];     // coord of pixel to test
            checkY = p.Y + searchDirection[newDir][1];
            isObjectLast = CheckPoint(checkX, checkY);      // check 'color' - background = false, object = true
            for (int i = 0; i <= 7; i++)
            {
                checkX = p.X + searchDirection[newDir][0];
                checkY = p.Y + searchDirection[newDir][1];
                isObjectCur = CheckPoint(checkX, checkY);
                if (!isObjectLast && isObjectCur)               // find 0/1 transient
                {   // check for 90 deg angle when oldDir is 0,2,4 or 6 and newDir 7,1,3,5
                    if ((oldDir % 2 == 0) && (((oldDir - newDir) == 1) || ((oldDir - newDir) == -7)))
                    {
                        if (log) logList.Append("correct ?\r\n");

                        int tnewDir = (newDir + 1) % 8;
                        int tcheckX = p.X + searchDirection[tnewDir][0];
                        int tcheckY = p.Y + searchDirection[tnewDir][1];
                        if (CheckPoint(tcheckX, tcheckY))       // also 0/1 transient?
                        {
                            p.X = tcheckX; p.Y = tcheckY;       // set next point
                            if (log) logList.AppendFormat("corrected new dir {0}   {1} ; {2}\r\n", newDir, tcheckX, tcheckY);
                            return tnewDir;                     // return direction
                        }
                    }
                    p.X = checkX; p.Y = checkY;             // set next point
                    if (log) logList.AppendFormat("new dir {0}   {1} ; {2}\r\n", newDir, checkX, checkY);
                    return newDir;                             // return direction
                }
                isObjectLast = isObjectCur;                 // store last value
                newDir = (newDir + 1) % 8;
            }
            return -1;
        }
        private static bool CheckPoint(int cx, int cy)
        {
            if ((cx < 0) || (cx >= Width) || (cy < 0) || (cy >= Height))
            { return false; }
            else
            {
                if ((bitmap[cx, cy] == toolNr) || (bitmap[cx, cy] >= markObject))
                {
                    bitmap[cx, cy] = markObject;        // mark as object
                    return true;
                }
                else
                {
                    bitmap[cx, cy] = markBackground;    // mark as background
                    return false;
                }
            }
        }
    }
}