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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using VECTOR = System.Numerics.Vector2;
using FLOAT = System.Single;

namespace burningmime.curves
{
    /// <summary>
    /// This is a version of <see cref="CurveFit"/> that works on partial curves so that a spline can be built in "realtime"
    /// as the user is drawing it. The quality of the generated spline may be lower, and it might use more Bezier curves
    /// than is necessary. Only the most recent two curves will be modified, once another curve is being built on top of it, curves
    /// lower in the "stack" are permanent. This reduces visual jumpiness as the user draws since the entire spline doesn't move
    /// around as points are added. It only uses linearization-based preprocessing; it doesn't support the RDP method.
    /// 
    /// Add points using the <see cref="AddPoint"/> method.To get the results, either enumerate (foreach) the CurveBuilder itself
    /// or use the <see cref="Curves"/> property. The results might be updated every time a point is added.
    /// </summary>
    public sealed class CurveBuilder : CurveFitBase, IEnumerable<CubicBezier>
    {
        private readonly List<CubicBezier> _result;                      // result curves (updated whenever a new point is added)
        private readonly ReadOnlyCollection<CubicBezier> _resultView;    // ReadOnlyCollection view of _result
        private readonly FLOAT _linDist;                                 // distance between points
        private VECTOR _prev;                                            // most recent point added
        private VECTOR _tanL;                                            // left tangent of current curve (can't change this except on first curve or we'll lose C1 continuity)
        private FLOAT _totalLength;                                      // Total length of the curve so far (for updating arclen)
        private int _first;                                              // Index of first point in current curve

        public CurveBuilder(FLOAT linDist, FLOAT error)
        {
            _squaredError = error * error;
            _result = new List<CubicBezier>(16);
            _resultView = new ReadOnlyCollection<CubicBezier>(_result);
            _linDist = linDist;
        }

        /// <summary>
        /// Adds a data point to the curve builder. This doesn't always result in the generated curve changing immediately.
        /// </summary>
        /// <param name="p">The data point to add.</param>
        /// <returns><see cref="AddPointResult"/> for info about this.</returns>
        public AddPointResult AddPoint(VECTOR p)
        {
            VECTOR prev = _prev;
            List<VECTOR> pts = _pts;
            int count = pts.Count;
            if(count != 0)
            {
                FLOAT td = VectorHelper.Distance(prev, p);
                FLOAT md = _linDist;
                if(td > md)
                {
                    int first = int.MaxValue;
                    bool add = false;
                    FLOAT rd = td - md;
                    // OPTIMIZE if we're adding many points at once, we could do them in a batch
                    VECTOR dir = VectorHelper.Normalize(p - prev);
                    do
                    {
                        VECTOR np = prev + dir * md;
                        AddPointResult res = AddInternal(np);
                        first = Math.Min(first, res.FirstChangedIndex);
                        add |= res.WasAdded;
                        prev = np;
                        rd -= md;
                    } while(rd > md);
                    _prev = prev;
                    return new AddPointResult(first, add);
                }
                return AddPointResult.NO_CHANGE;
            }
            else
            {
                _prev = p;
                _pts.Add(p);
                _arclen.Add(0);
                return AddPointResult.NO_CHANGE; // no curves were actually added yet
            }
        }

        private AddPointResult AddInternal(VECTOR np)
        {
            List<VECTOR> pts = _pts;
            int last = pts.Count;
            Debug.Assert(last != 0); // should always have one point at least
            _pts.Add(np);
            _arclen.Add(_totalLength = _totalLength + _linDist);
            if(last == 1)
            {
                // This is the second point
                Debug.Assert(_result.Count == 0);
                VECTOR p0 = pts[0];
                VECTOR tanL = VectorHelper.Normalize(np - p0);
                VECTOR tanR = -tanL;
                _tanL = tanL;
                FLOAT alpha = _linDist / 3;
                VECTOR p1 = (tanL * alpha) + p0;
                VECTOR p2 = (tanR * alpha) + np;
                _result.Add(new CubicBezier(p0, p1, p2, np));
                return new AddPointResult(0, true);
            }
            else
            {
                int lastCurve = _result.Count - 1;
                int first = _first;

                // If we're on the first curve, we're free to improve the left tangent
                VECTOR tanL = lastCurve == 0 ? GetLeftTangent(last) : _tanL;

                // We can always do the end tangent
                VECTOR tanR = GetRightTangent(first);

                // Try fitting with the new point
                int split;
                CubicBezier curve;
                if(FitCurve(first, last, tanL, tanR, out curve, out split))
                {
                    _result[lastCurve] = curve;
                    return new AddPointResult(lastCurve, false);
                }
                else
                {
                    // Need to split
                    // first, get mid tangent
                    VECTOR tanM1 = GetCenterTangent(first, last, split);
                    VECTOR tanM2 = -tanM1;

                    // PERHAPS do a full fitRecursive here since its our last chance?

                    // our left tangent might be based on points outside the new curve (this is possible for mid tangents too
                    // but since we need to maintain C1 continuity, it's too late to do anything about it)
                    if(first == 0 && split < END_TANGENT_N_PTS)
                        tanL = GetLeftTangent(split);

                    // do a final pass on the first half of the curve
                    int unused;
                    FitCurve(first, split, tanL, tanM1, out curve, out unused);
                    _result[lastCurve] = curve;

                    // perpare to fit the second half
                    FitCurve(split, last, tanM2, tanR, out curve, out unused);
                    _result.Add(curve);
                    _first = split;
                    _tanL = tanM2;

                    return new AddPointResult(lastCurve, true);
                }
            }
        }

        /// <summary>
        /// Clears the curve builder.
        /// </summary>
        public void Clear()
        {
            _result.Clear();
            _pts.Clear();
            _arclen.Clear();
            _u.Clear();
            _totalLength = 0;
            _first = 0;
            _tanL = default(VECTOR);
            _prev = default(VECTOR);
        }

        // We provide these for both convience and performance, since a call to List<T>.GetEnumerator() doesn't actually allocate if
        // the type is never boxed
        public List<CubicBezier>.Enumerator GetEnumerator() { return _result.GetEnumerator(); } 
        IEnumerator<CubicBezier> IEnumerable<CubicBezier>.GetEnumerator() { return GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        /// <summary>
        /// The current curves in the builder.
        /// </summary>
        public ReadOnlyCollection<CubicBezier> Curves { get { return _resultView; } }

        /// <summary>
        /// Changes made to the CurveBuilder.curves list after a call to <see cref="CurveBuilder.AddPoint"/>.
        /// This seems like a prime candidate for an F#-style discriminated union/algebraic data type.
        /// </summary>
        public struct AddPointResult
        {
            private readonly int _data; // packed value... need this so that default(AddPointResult) which is always 0 to represent no change

            /// <summary>
            /// No changes were made.
            /// </summary>
            public static readonly AddPointResult NO_CHANGE = default(AddPointResult);
            
            /// <summary>
            /// Were any curves changed or added?
            /// </summary>
            public bool WasChanged { get { return _data != 0; } }

            /// <summary>
            /// Index into curves array of first curve that was changed, or -1 if no curves were changed.
            /// All curves after this are assumed to have changed/been added as well. If a curve was added
            /// this is a considered a "change" so <see cref="WasAdded"/> will always be true.
            /// </summary>
            public int FirstChangedIndex { get { return Math.Abs(_data) - 1; } }

            /// <summary>
            /// Were any curves added?
            /// </summary>
            public bool WasAdded { get { return _data < 0; } }

            public AddPointResult(int firstChangedIndex, bool curveAdded)
            {
                if(firstChangedIndex < 0 || firstChangedIndex == int.MaxValue)
                    throw new InvalidOperationException("firstChangedIndex must be greater than zero");
                _data = (firstChangedIndex + 1) * (curveAdded ? -1 : 1);
            }
        }
    }
}