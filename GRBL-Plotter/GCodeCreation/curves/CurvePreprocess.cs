// Copyright (c) 2015 burningmime
// 
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
// 
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgement in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using VECTOR = System.Numerics.Vector2;
using FLOAT = System.Single;

namespace burningmime.curves
{
    public static class CurvePreprocess
    {
        private const FLOAT EPSILON = VectorHelper.EPSILON;

        /// <summary>
        /// Creates a list of equally spaced points that lie on the path described by straight line segments between
        /// adjacent points in the source list.
        /// </summary>
        /// <param name="src">Source list of points.</param>
        /// <param name="md">Distance between points on the new path.</param>
        /// <returns>List of equally-spaced points on the path.</returns>
        public static List<VECTOR> Linearize(List<VECTOR> src, FLOAT md)
        {
            if(src == null) throw new ArgumentNullException("src");
            if(md <= VectorHelper.EPSILON) throw new InvalidOperationException("md " + md + " is be less than epislon " + EPSILON);
            List<VECTOR> dst = new List<VECTOR>();
            if(src.Count > 0)
            {
                VECTOR pp = src[0];
                dst.Add(pp);
                FLOAT cd = 0;
                for(int ip = 1; ip < src.Count; ip++)
                {
                    VECTOR p0 = src[ip - 1];
                    VECTOR p1 = src[ip];
                    FLOAT td = VectorHelper.Distance(p0, p1);
                    if(cd + td > md)
                    {
                        FLOAT pd = md - cd;
                        dst.Add(VectorHelper.Lerp(p0, p1, pd / td));
                        FLOAT rd = td - pd;
                        while(rd > md)
                        {
                            rd -= md;
                            VECTOR np = VectorHelper.Lerp(p0, p1, (td - rd) / td);
                            if(!VectorHelper.EqualsOrClose(np, pp))
                            {
                                dst.Add(np);
                                pp = np;
                            }
                        }
                        cd = rd;
                    }
                    else
                    {
                        cd += td;
                    }
                }
                // last point
                VECTOR lp = src[src.Count - 1];
                if(!VectorHelper.EqualsOrClose(pp, lp))
                    dst.Add(lp);
            }
            return dst;
        }

        /// <summary>
        /// Removes any repeated points (that is, one point extremely close to the previous one). The same point can
        /// appear multiple times just not right after one another. This does not modify the input list. If no repeats
        /// were found, it returns the input list; otherwise it creates a new list with the repeats removed.
        /// </summary>
        /// <param name="pts">Initial list of points.</param>
        /// <returns>Either pts (if no duplicates were found), or a new list containing pts with duplicates removed.</returns>
        public static List<VECTOR> RemoveDuplicates(List<VECTOR> pts)
        {
            if(pts.Count < 2)
                return pts;

            // Common case -- no duplicates, so just return the source list
            VECTOR prev = pts[0];
            int len = pts.Count;
            int nDup = 0;
            for(int i = 1; i < len; i++)
            {
                VECTOR cur = pts[i];
                if(VectorHelper.EqualsOrClose(prev, cur))
                    nDup++;
                else
                    prev = cur;
            }

            if(nDup == 0)
                return pts;
            else
            {
                // Create a copy without them
                List<VECTOR> dst = new List<VECTOR>(len - nDup);
                prev = pts[0];
                dst.Add(prev);
                for(int i = 1; i < len; i++)
                {
                    VECTOR cur = pts[i];
                    if(!VectorHelper.EqualsOrClose(prev, cur))
                    {
                        dst.Add(cur);
                        prev = cur;
                    }
                }
                return dst;
            }
        }

        /// <summary>
        /// "Reduces" a set of line segments by removing points that are too far away. Does not modify the input list; returns
        /// a new list with the points removed.
        /// The image says it better than I could ever describe: http://upload.wikimedia.org/wikipedia/commons/3/30/Douglas-Peucker_animated.gif
        /// The wiki article: http://en.wikipedia.org/wiki/Ramer%E2%80%93Douglas%E2%80%93Peucker_algorithm
        /// Based on:  http://www.codeproject.com/Articles/18936/A-Csharp-Implementation-of-Douglas-Peucker-Line-Ap
        /// </summary>
        /// <param name="pts">Points to reduce</param>
        /// <param name="error">Maximum distance of a point to a line. Low values (~2-4) work well for mouse/touchscreen data.</param>
        /// <returns>A new list containing only the points needed to approximate the curve.</returns>
        public static List<VECTOR> RdpReduce(List<VECTOR> pts, FLOAT error)
        {
            if(pts == null) throw new ArgumentNullException("pts");
            pts = RemoveDuplicates(pts);
            if(pts.Count < 3)
                return new List<VECTOR>(pts);
            List<int> keepIndex = new List<int>(Math.Max(pts.Count / 2, 16));
            keepIndex.Add(0);
            keepIndex.Add(pts.Count - 1);
            RdpRecursive(pts, error, 0, pts.Count - 1, keepIndex);
            keepIndex.Sort();
            List<VECTOR> res = new List<VECTOR>(keepIndex.Count);
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach(int idx in keepIndex)
                res.Add(pts[idx]);
            return res;
        }

        private static void RdpRecursive(List<VECTOR> pts, FLOAT error, int first, int last, List<int> keepIndex)
        {
            int nPts = last - first + 1;
            if(nPts < 3)
                return;

            VECTOR a = pts[first];
            VECTOR b = pts[last];
            FLOAT abDist = VectorHelper.Distance(a, b);
            FLOAT aCrossB = VectorHelper.GetX(a) * VectorHelper.GetY(b) - VectorHelper.GetX(b) * VectorHelper.GetY(a);
            FLOAT maxDist = error;
            int split = 0;
            for(int i = first + 1; i < last - 1; i++)
            {
                VECTOR p = pts[i];
                FLOAT pDist = PerpendicularDistance(a, b, abDist, aCrossB, p);
                if(pDist > maxDist)
                {
                    maxDist = pDist;
                    split = i;
                }
            }

            if(split != 0)
            {
                keepIndex.Add(split);
                RdpRecursive(pts, error, first, split, keepIndex);
                RdpRecursive(pts, error, split, last, keepIndex);
            }
        }

        /// <summary>
        /// Finds the shortest distance between a point and a line. See: http://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
        /// </summary>
        /// <param name="a">First point of the line.</param>
        /// <param name="b">Last point of the line.</param>
        /// <param name="abDist">Distance between a and b (length of the line).</param>
        /// <param name="aCrossB">"a.X*b.Y - b.X*a.Y" This would be the Z-component of (⟪a.X, a.Y, 0⟫ ⨯ ⟪b.X, b.Y, 0⟫) in 3-space.</param>
        /// <param name="p">The point to test.</param>
        /// <returns>The perpendicular distance to the line.</returns>
        #if !UNITY
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // originally this method wasn't be inlined
        #endif
        private static FLOAT PerpendicularDistance(VECTOR a, VECTOR b, FLOAT abDist, FLOAT aCrossB, VECTOR p)
        {
            // a profile with the test data showed that originally this was eating up ~44% of the runtime. So, this went through
            // several iterations of optimization and staring at the disassembly. I tried different methods of using cross
            // products, doing the computation with larger vector types, etc... this is the best I could do in ~45 minutes
            // running on 3 hours of sleep, which is all scalar math, but RyuJIT puts it into XMM registers and does
            // ADDSS/SUBSS/MULSS/DIVSS because that's what it likes to do whenever it sees a vector in a function.
            FLOAT area = Math.Abs(aCrossB + 
                VectorHelper.GetX(b) * VectorHelper.GetY(p) + VectorHelper.GetX(p) * VectorHelper.GetY(a) -
                VectorHelper.GetX(p) * VectorHelper.GetY(b) - VectorHelper.GetX(a) * VectorHelper.GetY(p));
            FLOAT height = area / abDist;
            return height;
        }
    }
}
