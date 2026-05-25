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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using VECTOR = System.Numerics.Vector2;
using FLOAT = System.Single;

namespace burningmime.curves
{
    /// <summary>
    /// Maps a set of 2D Bezier curves so that samples are equally spaced across the spline. Basically, it does a lot of preprocessing and
    /// such on a set of curves so that when you call sample(0.5) you get a point that's halfway along the spline. This means that if you
    /// "move" something along the spline, it will move at a constant velocity. This is also useful for rendering the spline since the points 
    /// will be evenly spaced.
    /// </summary>
    public sealed class Spline
    {
        public const int MIN_SAMPLES_PER_CURVE = 8;
        public const int MAX_SAMPLES_PER_CURVE = 1024;
        private const FLOAT EPSILON = VectorHelper.EPSILON;

        private readonly List<CubicBezier> _curves; 
        private readonly ReadOnlyCollection<CubicBezier> _curvesView; 
        private readonly List<FLOAT> _arclen;
        private readonly int _samplesPerCurve;
        
        /// <summary>
        /// Creates an empty spline.
        /// </summary>
        /// <param name="samplesPerCurve">Resolution of the curve. Values 32-256 work well. You may need more or less depending on how big the curves are.</param>
        public Spline(int samplesPerCurve)
        {
            if(samplesPerCurve < MIN_SAMPLES_PER_CURVE || samplesPerCurve > MAX_SAMPLES_PER_CURVE)
                throw new InvalidOperationException("samplesPerCurve must be between " + MIN_SAMPLES_PER_CURVE + " and " + MAX_SAMPLES_PER_CURVE);
            _samplesPerCurve = samplesPerCurve;
            _curves = new List<CubicBezier>(16);
            _curvesView = new ReadOnlyCollection<CubicBezier>(_curves);
            _arclen = new List<FLOAT>(16 * samplesPerCurve);
        }

        /// <summary>
        /// Creates a new spline from the given curves.
        /// </summary>
        /// <param name="curves">Curves to create the spline from.</param>
        /// <param name="samplesPerCurve">Resolution of the curve. Values 32-256 work well. You may need more or less depending on how big the curves are.</param>
        public Spline(ICollection<CubicBezier> curves, int samplesPerCurve)
        {
            if(curves == null)
                throw new ArgumentNullException("curves");
            if(samplesPerCurve < MIN_SAMPLES_PER_CURVE || samplesPerCurve > MAX_SAMPLES_PER_CURVE)
                throw new InvalidOperationException("samplesPerCurve must be between " + MIN_SAMPLES_PER_CURVE + " and " + MAX_SAMPLES_PER_CURVE);
            _samplesPerCurve = samplesPerCurve;
            _curves = new List<CubicBezier>(curves.Count);
            _curvesView = new ReadOnlyCollection<CubicBezier>(_curves);
            _arclen = new List<FLOAT>(_curves.Count * samplesPerCurve);
            foreach(CubicBezier curve in curves)
                Add(curve);
        }

        /// <summary>
        /// Adds a curve to the end of the spline.
        /// </summary>
        public void Add(CubicBezier curve)
        {
             if(_curves.Count > 0 && !VectorHelper.EqualsOrClose(_curves[_curves.Count - 1].p3, curve.p0))
                throw new InvalidOperationException("The new curve does at index " + _curves.Count + " does not connect with the previous curve at index " + (_curves.Count - 1));
            _curves.Add(curve);
            for(int i = 0; i < _samplesPerCurve; i++) // expand the array since updateArcLengths expects these values to be there
                _arclen.Add(0);
            UpdateArcLengths(_curves.Count - 1);
        }

        /// <summary>
        /// Modifies a curve in the spline. It must connect with the previous and next curves (if applicable). This requires that the
        /// arc length table be recalculated for that curve and all curves after it -- for example, if you update the first curve in the
        /// spline, each curve after that would need to be recalculated (could avoid this by caching the lengths on a per-curve basis if you're
        /// doing this often, but since the typical case is only updating the last curve, and the entire array needs to be visited anyway, it
        /// wouldn't save much).
        /// </summary>
        /// <param name="index">Index of the curve to update in <see cref="Curves"/>.</param>
        /// <param name="curve">The new curve with which to replace it.</param>
        public void Update(int index, CubicBezier curve)
        {
            if(index < 0)
                throw new IndexOutOfRangeException("Negative index");
            if(index >= _curves.Count)
                throw new IndexOutOfRangeException("Curve index " + index + " is out of range (there are " + _curves.Count + " curves in the spline)");
            if(index > 0 && !VectorHelper.EqualsOrClose(_curves[index - 1].p3, curve.p0))
                throw new InvalidOperationException("The updated curve at index " + index + " does not connect with the previous curve at index " + (index - 1));
            if(index < _curves.Count - 1 && !VectorHelper.EqualsOrClose(_curves[index + 1].p0, curve.p3))
                throw new InvalidOperationException("The updated curve at index " + index + " does not connect with the next curve at index " + (index + 1));
            _curves[index] = curve;
            for(int i = index; i < _curves.Count; i++)
                UpdateArcLengths(i);
        }

        /// <summary>
        /// Clears the spline.
        /// </summary>
        public void Clear()
        {
            _curves.Clear();
            _arclen.Clear();
        }

        /// <summary>
        /// Gets the total length of the spline.
        /// </summary>
        public FLOAT Length
        {
            #if !UNITY
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            #endif
            get
            {
                List<FLOAT> arclen = _arclen;
                int count = arclen.Count;
                return count == 0 ? 0 : arclen[count - 1];
            }
        }

        /// <summary>
        /// Gets a read-only view of the current curves collection.
        /// </summary>
        public ReadOnlyCollection<CubicBezier> Curves
        {
            get { return _curvesView; }
        }
        
        /// <summary>
        /// Gets the position of a point on the spline that's close to the desired point along the spline. For example, if u = 0.5, then a point
        /// that's about halfway through the spline will be returned. The returned point will lie exactly on one of the curves that make up the
        /// spline.
        /// </summary>
        /// <param name="u">How far along the spline to sample (for example, 0.5 will be halfway along the length of the spline). Should be between 0 and 1.</param>
        /// <returns>The position on the spline.</returns>
        public VECTOR Sample(FLOAT u)
        {
            SamplePos pos = GetSamplePosition(u);
            return _curves[pos.Index].Sample(pos.Time);
        }

        /// <summary>
        /// Gets the curve index and t-value to sample to get a point at the desired part of the spline.
        /// </summary>
        /// <param name="u">How far along the spline to sample (for example, 0.5 will be halfway along the length of the spline). Should be between 0 and 1.</param>
        /// <returns>The position to sample at.</returns>
        public SamplePos GetSamplePosition(FLOAT u)
        {
            if(_curves.Count == 0) 
                throw new InvalidOperationException("No curves have been added to the spline");
            if(u < 0) 
                return new SamplePos(0, 0);
            if(u > 1)
                return new SamplePos(_curves.Count - 1, 1);

            List<FLOAT> arclen = _arclen;
            FLOAT total = Length;
            FLOAT target = u * total;
            Debug.Assert(target >= 0);

            // Binary search to find largest value <= target
            int index = 0;
            int low = 0;
            int high = arclen.Count - 1;
            FLOAT found = float.NaN;
            while(low < high)
            {
                index = (low + high) / 2;
                found = arclen[index];
                if (found < target)
                    low = index + 1;
                else
                    high = index;
            }

            // this should be a rather rare scenario: we're past the end, but this wasn't picked up by the test for u >= 1
            if(index >= arclen.Count - 1)
                return new SamplePos(_curves.Count - 1, 1);

            // this can happen because the binary search can give us either index or index + 1
            if(found > target)
                index--;

            if(index < 0)
            {
                // We're at the beginning of the spline
                FLOAT max = arclen[0];
                Debug.Assert(target <= max + EPSILON);
                FLOAT part = target / max;
                FLOAT t = part / _samplesPerCurve;
                return new SamplePos(0, t);
            }
            else
            {
                // interpolate between two values to see where the index would be if continuous values
                FLOAT min = arclen[index];
                FLOAT max = arclen[index + 1];
                Debug.Assert(target >= min - EPSILON && target <= max + EPSILON);
                FLOAT part = target < min ? 0 : target > max ? 1 : (target - min) / (max - min);
                FLOAT t = (((index + 1) % _samplesPerCurve) + part) / _samplesPerCurve;
                int curveIndex = (index + 1) / _samplesPerCurve;
                return new SamplePos(curveIndex, t);
            }
        }

        /// <summary>
        /// Updates the internal arc length array for a curve. Expects the list to contain enough elements.
        /// </summary>
        private void UpdateArcLengths(int iCurve)
        {
            CubicBezier curve = _curves[iCurve];
            int nSamples = _samplesPerCurve;
            List<FLOAT> arclen = _arclen;
            FLOAT clen = iCurve > 0 ? arclen[iCurve * nSamples - 1] : 0;
            VECTOR pp = curve.p0;
            Debug.Assert(arclen.Count >= ((iCurve + 1) * nSamples));
            for(int iPoint = 0; iPoint < nSamples; iPoint++)
            {
                int idx = (iCurve * nSamples) + iPoint;
                FLOAT t = (iPoint + 1) / (FLOAT) nSamples;
                VECTOR np = curve.Sample(t);
                FLOAT d = VectorHelper.Distance(np, pp);
                clen += d;
                arclen[idx] = clen;
                pp = np;
            }
        }

        /// <summary>
        /// Point at which to sample the spline.
        /// </summary>
        public struct SamplePos
        {
            /// <summary>
            /// Index of sampled curve in the spline curves array.
            /// </summary>
            public readonly int Index;

            /// <summary>
            /// The "t" value from which to sample the curve.
            /// </summary>
            public readonly FLOAT Time;

            public SamplePos(int curveIndex, FLOAT t)
            {
                Index = curveIndex;
                Time = t;
            }
        }
    }
}