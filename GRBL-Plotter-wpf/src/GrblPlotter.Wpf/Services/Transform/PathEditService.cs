using GrblPlotter.Wpf.Models;
using GrblPlotter.Wpf.Services;

namespace GrblPlotter.Wpf.Services.Transform;

/// <summary>Path / segment selection edits for the 2D preview (WinForms picture-box context subset).</summary>
public static class PathEditService
{
    public static int HitTestSegment(GCodeDocument doc, double worldX, double worldY, double tolerance)
    {
        int best = -1;
        double bestD = tolerance;
        for (int i = 0; i < doc.Segments.Count; i++)
        {
            var s = doc.Segments[i];
            double d = DistPointToSegment(worldX, worldY, s.X0, s.Y0, s.X1, s.Y1);
            if (d < bestD) { bestD = d; best = i; }
        }
        return best;
    }

    /// <summary>Expand a segment index to a connected non-rapid "path" (shared endpoints).</summary>
    public static List<int> ExpandToPath(GCodeDocument doc, int seed)
    {
        var result = new List<int>();
        if (seed < 0 || seed >= doc.Segments.Count) return result;
        var used = new bool[doc.Segments.Count];
        var q = new Queue<int>();
        q.Enqueue(seed);
        used[seed] = true;
        while (q.Count > 0)
        {
            int i = q.Dequeue();
            result.Add(i);
            var a = doc.Segments[i];
            for (int j = 0; j < doc.Segments.Count; j++)
            {
                if (used[j] || doc.Segments[j].Rapid) continue;
                var b = doc.Segments[j];
                if (Near(a.X0, a.Y0, b.X0, b.Y0) || Near(a.X0, a.Y0, b.X1, b.Y1) ||
                    Near(a.X1, a.Y1, b.X0, b.Y0) || Near(a.X1, a.Y1, b.X1, b.Y1))
                {
                    used[j] = true;
                    q.Enqueue(j);
                }
            }
        }
        result.Sort();
        return result;
    }

    public static void DeleteIndices(GCodeDocument doc, IReadOnlyList<int> indices)
    {
        var remove = new HashSet<int>(indices);
        var keep = doc.Segments.Where((_, i) => !remove.Contains(i)).ToList();
        RebuildFromSegments(doc, keep);
    }

    public static void DuplicateIndices(GCodeDocument doc, IReadOnlyList<int> indices, double dx = 2, double dy = 2)
    {
        var add = indices.OrderBy(i => i).Select(i =>
        {
            var s = doc.Segments[i];
            return new GCodeSegment
            {
                Rapid = s.Rapid,
                X0 = s.X0 + dx, Y0 = s.Y0 + dy,
                X1 = s.X1 + dx, Y1 = s.Y1 + dy
            };
        }).ToList();
        var all = doc.Segments.Concat(add).ToList();
        RebuildFromSegments(doc, all);
    }

    public static void ReverseIndices(GCodeDocument doc, IReadOnlyList<int> indices)
    {
        var set = new HashSet<int>(indices);
        foreach (var i in set)
        {
            var s = doc.Segments[i];
            (s.X0, s.X1) = (s.X1, s.X0);
            (s.Y0, s.Y1) = (s.Y1, s.Y0);
        }
        // reverse order of selected block if contiguous
        if (indices.Count > 1)
        {
            int lo = indices.Min(), hi = indices.Max();
            if (hi - lo + 1 == indices.Count)
                doc.Segments.Reverse(lo, indices.Count);
        }
        RebuildFromSegments(doc, doc.Segments.ToList());
    }

    public static void RotateIndices(GCodeDocument doc, IReadOnlyList<int> indices, double angleDeg)
    {
        double cx = 0, cy = 0;
        foreach (var i in indices)
        {
            var s = doc.Segments[i];
            cx += s.X0 + s.X1; cy += s.Y0 + s.Y1;
        }
        cx /= indices.Count * 2; cy /= indices.Count * 2;
        double rad = angleDeg * Math.PI / 180;
        double cos = Math.Cos(rad), sin = Math.Sin(rad);
        foreach (var i in indices)
        {
            var s = doc.Segments[i];
            (s.X0, s.Y0) = Rot(s.X0, s.Y0);
            (s.X1, s.Y1) = Rot(s.X1, s.Y1);
        }
        RebuildFromSegments(doc, doc.Segments.ToList());

        (double X, double Y) Rot(double x, double y)
        {
            double dx = x - cx, dy = y - cy;
            return (cx + dx * cos - dy * sin, cy + dx * sin + dy * cos);
        }
    }

    /// <summary>Keep only selected indices (crop).</summary>
    public static void CropToIndices(GCodeDocument doc, IReadOnlyList<int> indices)
    {
        var keep = indices.OrderBy(i => i).Select(i => doc.Segments[i]).ToList();
        RebuildFromSegments(doc, keep);
    }

    private static void RebuildFromSegments(GCodeDocument doc, List<GCodeSegment> segs)
    {
        var rebuilt = GCodeParser.BuildFromSegments(segs, path: doc.FilePath);
        doc.Lines.Clear();
        doc.Lines.AddRange(rebuilt.Lines);
        doc.Segments.Clear();
        doc.Segments.AddRange(rebuilt.Segments);
        doc.MinX = rebuilt.MinX; doc.MaxX = rebuilt.MaxX;
        doc.MinY = rebuilt.MinY; doc.MaxY = rebuilt.MaxY;
    }

    private static bool Near(double x0, double y0, double x1, double y1) =>
        Math.Abs(x0 - x1) < 1e-4 && Math.Abs(y0 - y1) < 1e-4;

    private static double DistPointToSegment(double px, double py, double x0, double y0, double x1, double y1)
    {
        double dx = x1 - x0, dy = y1 - y0;
        double len2 = dx * dx + dy * dy;
        if (len2 < 1e-18) return Math.Sqrt((px - x0) * (px - x0) + (py - y0) * (py - y0));
        double t = Math.Clamp(((px - x0) * dx + (py - y0) * dy) / len2, 0, 1);
        double qx = x0 + t * dx, qy = y0 + t * dy;
        return Math.Sqrt((px - qx) * (px - qx) + (py - qy) * (py - qy));
    }
}
