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

using System.Collections.ObjectModel;
using System.Diagnostics;

using VECTOR = System.Numerics.Vector2;
using FLOAT = System.Single;

namespace burningmime.curves
{
    /// <summary>
    /// Wraps a <see cref="CurveBuilder"/> and <see cref="Spline"/> together. Allows you to add data points as they come in and
    /// generate a smooth spline from them without doing unnecessary computation.
    /// </summary>
    public sealed class SplineBuilder
    {
        private readonly CurveBuilder _builder;      // Underlying curve fitter
        private readonly Spline _spline;             // Underlyig spline

        public SplineBuilder(FLOAT pointDistance, FLOAT error, int samplesPerCurve)
        {
            _builder = new CurveBuilder(pointDistance, error);
            _spline = new Spline(samplesPerCurve);
        }

        /// <summary>
        /// Adds a data point.
        /// </summary>
        /// <param name="p">Data point to add.</param>
        /// <returns>True if the spline was modified.</returns>
        public bool Add(VECTOR p)
        {
            CurveBuilder.AddPointResult res = _builder.AddPoint(p);
            if(!res.WasChanged)
                return false;

            // update spline
            ReadOnlyCollection<CubicBezier> curves = _builder.Curves;
            if(res.WasAdded && curves.Count == 1)
            {
                // first curve
                Debug.Assert(_spline.Curves.Count == 0);
                _spline.Add(curves[0]);
            }
            else if(res.WasAdded)
            {
                // split
                _spline.Update(_spline.Curves.Count - 1, curves[res.FirstChangedIndex]);
                for(int i = res.FirstChangedIndex + 1; i < curves.Count; i++)
                    _spline.Add(curves[i]);
            }
            else
            {
                // last curve updated
                Debug.Assert(res.FirstChangedIndex == curves.Count - 1);
                _spline.Update(_spline.Curves.Count - 1, curves[curves.Count - 1]);
            }

            return true;
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
            return _spline.Sample(u);
        }

        /// <summary>
        /// Gets the tangent of a point on the spline that's close to the desired point along the spline. For example, if u = 0.5, then the direction vector
        /// that's about halfway through the spline will be returned. The returned value will be a normalized direction vector.
        /// </summary>
        /// <param name="u">How far along the spline to sample (for example, 0.5 will be halfway along the length of the spline). Should be between 0 and 1.</param>
        /// <returns>The position on the spline.</returns>
        public VECTOR Tangent(FLOAT u)
        {
            Spline.SamplePos pos = _spline.GetSamplePosition(u);
            return _spline.Curves[pos.Index].Tangent(pos.Time);
        }

        /// <summary>
        /// Clears the SplineBuilder.
        /// </summary>
        public void Clear()
        {
            _builder.Clear();
            _spline.Clear();
        }

        /// <summary>
        /// The curves that make up the spline.
        /// </summary>
        public ReadOnlyCollection<CubicBezier> Curves
        {
            get
            {
                return _spline.Curves;
            }
        }
    }
}