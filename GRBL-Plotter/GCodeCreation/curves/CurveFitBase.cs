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
using System.Diagnostics;

using VECTOR = System.Numerics.Vector2;
using FLOAT = System.Single;

namespace burningmime.curves
{
    /// <summary>
    /// This is the base class containing implementations common to <see cref="CurveFit"/> and <see cref="CurveBuilder"/>. Most of this
    /// is ported from http://tog.acm.org/resources/GraphicsGems/gems/FitCurves.c
    /// </summary>
    public abstract class CurveFitBase
    {
        protected const FLOAT EPSILON = VectorHelper.EPSILON;  // below this, we can't trust floating point values
        protected const int MAX_ITERS = 4;                     // maximum number of iterations of newton's method to run before giving up and splitting curve
        protected const int END_TANGENT_N_PTS = 8;             // maximum number of points to base end tangent on
        protected const int MID_TANGENT_N_PTS = 4;             // maximum number of points on each side to base mid tangent on
        
        /// <summary>
        /// Points in the whole line being used for fitting.
        /// </summary>
        protected readonly List<VECTOR> _pts = new List<VECTOR>(256);

        /// <summary>
        /// length of curve before each point (so, arclen[0] = 0, arclen[1] = distance(pts[0], pts[1]),
        /// arclen[2] = arclen[1] + distance(pts[1], pts[2]) ... arclen[n -1] = length of the entire curve, etc).
        /// </summary>
        protected readonly List<FLOAT> _arclen = new List<FLOAT>(256);

        /// <summary>
        /// current parametrization of the curve. When fitting, u[i] is the parametrization for the point in pts[first + i]. This is
        /// an optimization for CurveBuilder, since it might not need to allocate as big of a _u as is necessary to hold the whole
        /// curve.
        /// </summary>
        protected readonly List<FLOAT> _u = new List<FLOAT>(256);

        /// <summary>
        /// maximum squared error before we split the curve
        /// </summary>
        protected FLOAT _squaredError;

        /// <summary>
        /// Tries to fit single Bezier curve to the points in [first ... last]. Destroys anything in <see cref="_u"/> in the process.
        /// Assumes there are at least two points to fit.
        /// </summary>
        /// <param name="first">Index of first point to consider.</param>
        /// <param name="last">Index of last point to consider (inclusive).</param>
        /// <param name="tanL">Tangent at teh start of the curve ("left").</param>
        /// <param name="tanR">Tangent on the end of the curve ("right").</param>
        /// <param name="curve">The fitted curve.</param>
        /// <param name="split">Point at which to split if this method returns false.</param>
        /// <returns>true if the fit was within error tolerence, false if the curve should be split. Even if this returns false, curve will contain
        /// a curve that somewhat fits the points; it's just outside error tolerance.</returns>
        protected bool FitCurve(int first, int last, VECTOR tanL, VECTOR tanR, out CubicBezier curve, out int split)
        {
            List<VECTOR> pts = _pts;
            int nPts = last - first + 1;
            if(nPts < 2)
            {
                throw new InvalidOperationException("INTERNAL ERROR: Should always have at least 2 points here");
            }
            else if(nPts == 2)
            {
                // if we only have 2 points left, estimate the curve using Wu/Barsky
                VECTOR p0 = pts[first];
                VECTOR p3 = pts[last];
                FLOAT alpha = VectorHelper.Distance(p0, p3) / 3;
                VECTOR p1 = (tanL * alpha) + p0;
                VECTOR p2 = (tanR * alpha) + p3;
                curve = new CubicBezier(p0, p1, p2, p3);
                split = 0;
                return true;
            }
            else
            {
                split = 0;
                ArcLengthParamaterize(first, last); // initially start u with a simple chord-length paramaterization
                curve = default(CubicBezier);
                for(int i = 0; i < MAX_ITERS + 1; i++)
                {
                    if(i != 0) Reparameterize(first, last, curve);                                  // use newton's method to find better parameters (except on first run, since we don't have a curve yet)
                    curve = GenerateBezier(first, last, tanL, tanR);                                // generate the curve itself
                    FLOAT error = FindMaxSquaredError(first, last, curve, out split);               // calculate error and get split point (point of max error)
                    if(error < _squaredError)  return true;                                         // if we're within error tolerance, awesome!
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the tangent for the start of the cure.
        /// </summary>
        protected VECTOR GetLeftTangent(int last)
        {
            List<VECTOR> pts = _pts;
            List<FLOAT> arclen = _arclen;
            FLOAT totalLen = arclen[arclen.Count - 1];
            VECTOR p0 = pts[0];
            VECTOR tanL = VectorHelper.Normalize(pts[1] - p0);
            VECTOR total = tanL;
            FLOAT weightTotal = 1;
            last = Math.Min(END_TANGENT_N_PTS, last - 1);
            for(int i = 2; i <= last; i++)
            {
                FLOAT ti = 1 - (arclen[i] / totalLen);
                FLOAT weight = ti * ti * ti;
                VECTOR v = VectorHelper.Normalize(pts[i] - p0);
                total += v * weight;
                weightTotal += weight;
            }
            // if the vectors add up to zero (ie going opposite directions), there's no way to normalize them
            if(VectorHelper.Length(total) > EPSILON)
                tanL = VectorHelper.Normalize(total / weightTotal);
            return tanL;
        }

        /// <summary>
        /// Gets the tangent for the the end of the curve.
        /// </summary>
        protected VECTOR GetRightTangent(int first)
        {
            List<VECTOR> pts = _pts;
            List<FLOAT> arclen = _arclen;
            FLOAT totalLen = arclen[arclen.Count - 1];
            VECTOR p3 = pts[pts.Count - 1];
            VECTOR tanR = VectorHelper.Normalize(pts[pts.Count - 2] - p3);
            VECTOR total = tanR;
            FLOAT weightTotal = 1;
            first = Math.Max(pts.Count - (END_TANGENT_N_PTS + 1), first + 1);
            for(int i = pts.Count - 3; i >= first; i--)
            {
                FLOAT t = arclen[i] / totalLen;
                FLOAT weight = t * t * t;
                VECTOR v = VectorHelper.Normalize(pts[i] - p3);
                total += v * weight;
                weightTotal += weight;
            }
            if(VectorHelper.Length(total) > EPSILON)
                tanR = VectorHelper.Normalize(total / weightTotal);
            return tanR;
        }

        /// <summary>
        /// Gets the tangent at a given point in the curve.
        /// </summary>
        protected VECTOR GetCenterTangent(int first, int last, int split)
        {
            List<VECTOR> pts = _pts;
            List<FLOAT> arclen = _arclen;

            // because we want to maintain C1 continuity on the spline, the tangents on either side must be inverses of one another
            Debug.Assert(first < split && split < last);
            FLOAT splitLen = arclen[split];
            VECTOR pSplit = pts[split];

            // left side
            FLOAT firstLen = arclen[first];
            FLOAT partLen = splitLen - firstLen;
            VECTOR total = default(VECTOR);
            FLOAT weightTotal = 0;
            for(int i = Math.Max(first, split - MID_TANGENT_N_PTS); i < split; i++)
            {
                FLOAT t = (arclen[i] - firstLen) / partLen;
                FLOAT weight = t * t * t;
                VECTOR v = VectorHelper.Normalize(pts[i] - pSplit);
                total += v * weight;
                weightTotal += weight;
            }
            VECTOR tanL = VectorHelper.Length(total) > EPSILON && weightTotal > EPSILON ? 
                VectorHelper.Normalize(total / weightTotal) :
                VectorHelper.Normalize(pts[split - 1] - pSplit);

            // right side
            partLen = arclen[last] - splitLen;
            int rMax = Math.Min(last, split + MID_TANGENT_N_PTS);
            total = default(VECTOR);
            weightTotal = 0;
            for(int i = split + 1; i <= rMax; i++)
            {
                FLOAT ti = 1 - ((arclen[i] - splitLen) / partLen);
                FLOAT weight = ti * ti * ti;
                VECTOR v = VectorHelper.Normalize(pSplit- pts[i]);
                total += v * weight;
                weightTotal += weight;
            }
            VECTOR tanR = VectorHelper.Length(total) > EPSILON && weightTotal > EPSILON ?
                VectorHelper.Normalize(total / weightTotal) :
                VectorHelper.Normalize(pSplit - pts[split + 1]);

            // The reason we separate this into two halves is because we want the right and left tangents to be weighted
            // equally no matter the weights of the individual parts of them, so that one of the curves doesn't get screwed
            // for the pleasure of the other half
            total = tanL + tanR;
            
            // Since the points are never coincident, the vector between any two of them will be normalizable, however this can happen in some really
            // odd cases when the points are going directly opposite directions (therefore the tangent is undefined)
            if(VectorHelper.LengthSquared(total) < EPSILON)
            {
                // try one last time using only the three points at the center, otherwise just use one of the sides
                tanL = VectorHelper.Normalize(pts[split - 1] - pSplit);
                tanR = VectorHelper.Normalize(pSplit - pts[split + 1]);
                total = tanL + tanR;
                return VectorHelper.LengthSquared(total) < EPSILON ? tanL : VectorHelper.Normalize(total / 2);
            }
            else
            {
                return VectorHelper.Normalize(total / 2);
            }
        }

        /// <summary>
        /// Builds the arc length array using the points array. Assumes _pts has points and _arclen is empty.
        /// </summary>
        protected void InitializeArcLengths()
        {
            List<VECTOR> pts = _pts;
            List<FLOAT> arclen = _arclen;
            int count = pts.Count;
            Debug.Assert(arclen.Count == 0);
            arclen.Add(0);
            FLOAT clen = 0;
            VECTOR pp = pts[0];
            for(int i = 1; i < count; i++)
            {
                VECTOR np = pts[i];
                clen += VectorHelper.Distance(pp, np);
                arclen.Add(clen);
                pp = np;
            }
        }

        /// <summary>
        /// Initializes the first (last - first) elements of u with scaled arc lengths.
        /// </summary>
        protected void ArcLengthParamaterize(int first, int last)
        {
            List<FLOAT> arclen = _arclen;
            List<FLOAT> u = _u;

            u.Clear();
            FLOAT diff = arclen[last] - arclen[first];
            FLOAT start = arclen[first];
            int nPts = last - first;
            u.Add(0);
            for(int i = 1; i < nPts; i++)
                u.Add((arclen[first + i] - start) / diff);
            u.Add(1);
        }

        /// <summary>
        /// Generates a bezier curve for the segment using a least-squares approximation. for the derivation of this and why it works,
        /// see http://read.pudn.com/downloads141/ebook/610086/Graphics_Gems_I.pdf page 626 and beyond. tl;dr: math.
        /// </summary>
        protected CubicBezier GenerateBezier(int first, int last, VECTOR tanL, VECTOR tanR)
        {
            List<VECTOR> pts = _pts;
            List<FLOAT> u = _u;
            int nPts = last - first + 1;
            VECTOR p0 = pts[first], p3 = pts[last]; // first and last points of curve are actual points on data
            FLOAT c00 = 0, c01 = 0, c11 = 0, x0 = 0, x1 = 0; // matrix members -- both C[0,1] and C[1,0] are the same, stored in c01
            for(int i = 1; i < nPts; i++)
            {
                // Calculate cubic bezier multipliers
                FLOAT t = u[i];
                FLOAT ti = 1 - t;
                FLOAT t0 = ti * ti * ti;
                FLOAT t1 = 3 * ti * ti * t;
                FLOAT t2 = 3 * ti * t * t;
                FLOAT t3 = t * t * t;

                // For X matrix; moving this up here since profiling shows it's better up here (maybe a0/a1 not in registers vs only v not in regs)
                VECTOR s = (p0 * t0) + (p0 * t1) + (p3 * t2) + (p3 * t3); // NOTE: this would be Q(t) if p1=p0 and p2=p3
                VECTOR v = pts[first + i] - s;

                // C matrix
                VECTOR a0 = tanL * t1;
                VECTOR a1 = tanR * t2;
                c00 += VectorHelper.Dot(a0, a0);
                c01 += VectorHelper.Dot(a0, a1);
                c11 += VectorHelper.Dot(a1, a1);

                // X matrix
                x0 += VectorHelper.Dot(a0, v);
                x1 += VectorHelper.Dot(a1, v);
            }

            // determinents of X and C matrices
            FLOAT det_C0_C1 = c00 * c11 - c01 * c01;
            FLOAT det_C0_X = c00 * x1 - c01 * x0;
            FLOAT det_X_C1 = x0 * c11 - x1 * c01;
            FLOAT alphaL = det_X_C1 / det_C0_C1;
            FLOAT alphaR = det_C0_X / det_C0_C1;
            
            // if alpha is negative, zero, or very small (or we can't trust it since C matrix is small), fall back to Wu/Barsky heuristic
            FLOAT linDist = VectorHelper.Distance(p0, p3);
            FLOAT epsilon2 = EPSILON * linDist;
            if(Math.Abs(det_C0_C1) < EPSILON || alphaL < epsilon2 || alphaR < epsilon2)
            {
                FLOAT alpha = linDist / 3;
                VECTOR p1 = (tanL * alpha) + p0;
                VECTOR p2 = (tanR * alpha) + p3;
                return new CubicBezier(p0, p1, p2, p3);
            }
            else
            {
                VECTOR p1 = (tanL * alphaL) + p0;
                VECTOR p2 = (tanR * alphaR) + p3;
                return new CubicBezier(p0, p1, p2, p3);
            }
        }

        /// <summary>
        /// Attempts to find a slightly better parameterization for u on the given curve.
        /// </summary>
        protected void Reparameterize(int first, int last, CubicBezier curve)
        {
            List<VECTOR> pts = _pts;
            List<FLOAT> u = _u;
            int nPts = last - first;
            for(int i = 1; i < nPts; i++)
            {
                VECTOR p = pts[first + i];
                FLOAT t = u[i];
                FLOAT ti = 1 - t;

                // Control vertices for Q'
                VECTOR qp0 = (curve.p1 - curve.p0) * 3;
                VECTOR qp1 = (curve.p2 - curve.p1) * 3;
                VECTOR qp2 = (curve.p3 - curve.p2) * 3;

                // Control vertices for Q''
                VECTOR qpp0 = (qp1 - qp0) * 2;
                VECTOR qpp1 = (qp2 - qp1) * 2;

                // Evaluate Q(t), Q'(t), and Q''(t)
                VECTOR p0 = curve.Sample(t);
                VECTOR p1 = ((ti * ti) * qp0) + ((2 * ti * t) * qp1) + ((t * t) * qp2);
                VECTOR p2 = (ti * qpp0) + (t * qpp1);

                // these are the actual fitting calculations using http://en.wikipedia.org/wiki/Newton%27s_method
                // We can't just use .X and .Y because Unity uses lower-case "x" and "y".
                FLOAT num = ((VectorHelper.GetX(p0) - VectorHelper.GetX(p)) * VectorHelper.GetX(p1)) + ((VectorHelper.GetY(p0) - VectorHelper.GetY(p)) * VectorHelper.GetY(p1));
                FLOAT den = (VectorHelper.GetX(p1) * VectorHelper.GetX(p1)) + (VectorHelper.GetY(p1) * VectorHelper.GetY(p1)) + ((VectorHelper.GetX(p0) - VectorHelper.GetX(p)) * VectorHelper.GetX(p2)) + ((VectorHelper.GetY(p0) - VectorHelper.GetY(p)) * VectorHelper.GetY(p2));
                FLOAT newU = t - num/den;
                if(Math.Abs(den) > EPSILON && newU >= 0 && newU <= 1)
                    u[i] = newU;
            }
        }

        /// <summary>
        /// Computes the maximum squared distance from a point to the curve using the current parameterization.
        /// </summary>
        protected FLOAT FindMaxSquaredError(int first, int last, CubicBezier curve, out int split)
        {
            List<VECTOR> pts = _pts;
            List<FLOAT> u = _u;
            int s = (last - first + 1) / 2;
            int nPts = last - first + 1;
            FLOAT max = 0;
            for(int i = 1; i < nPts; i++)
            {
                VECTOR v0 = pts[first + i];
                VECTOR v1 = curve.Sample(u[i]);
                FLOAT d = VectorHelper.DistanceSquared(v0, v1);
                if(d > max)
                {
                    max = d;
                    s = i;
                }
            }

            // split at point of maximum error
            split = s + first;
            if(split <= first)
                split = first + 1;
            if(split >= last)
                split = last - 1;

            return max;
        }
    }
}