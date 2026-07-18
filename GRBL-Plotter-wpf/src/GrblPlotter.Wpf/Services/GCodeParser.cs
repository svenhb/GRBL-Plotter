using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services;

public static class GCodeParser
{
    private static readonly Regex WordRx = new(@"([GMTXYZABCIJKFSP])\s*([-+]?(?:\d+\.?\d*|\.\d+))",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static GCodeDocument Parse(string text, string? path = null)
    {
        var doc = new GCodeDocument { FilePath = path ?? "" };
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        double x = 0, y = 0, z = 0;
        bool absolute = true;
        bool first = true;
        double minX = 0, minY = 0, maxX = 0, maxY = 0;

        foreach (var raw in lines)
        {
            doc.Lines.Add(raw);
            var line = raw.Split(';')[0].Trim();
            if (line.Length == 0 || line.StartsWith('(')) continue;

            int? motion = null;
            double? nx = null, ny = null, nz = null;

            foreach (Match m in WordRx.Matches(line))
            {
                var letter = char.ToUpperInvariant(m.Groups[1].Value[0]);
                var val = double.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
                switch (letter)
                {
                    case 'G':
                        var g = (int)val;
                        if (g is 0 or 1 or 2 or 3) motion = g;
                        if (g == 90) absolute = true;
                        if (g == 91) absolute = false;
                        break;
                    case 'X': nx = val; break;
                    case 'Y': ny = val; break;
                    case 'Z': nz = val; break;
                }
            }

            if (motion is null && (nx is null && ny is null && nz is null)) continue;
            motion ??= 1;

            var x1 = nx.HasValue ? (absolute ? nx.Value : x + nx.Value) : x;
            var y1 = ny.HasValue ? (absolute ? ny.Value : y + ny.Value) : y;
            var z1 = nz.HasValue ? (absolute ? nz.Value : z + nz.Value) : z;

            if (motion is 0 or 1)
            {
                doc.Segments.Add(new GCodeSegment
                {
                    Rapid = motion == 0,
                    X0 = x, Y0 = y, X1 = x1, Y1 = y1
                });
            }

            x = x1; y = y1; z = z1;
            if (first)
            {
                minX = maxX = x; minY = maxY = y; first = false;
            }
            else
            {
                minX = Math.Min(minX, x); maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y); maxY = Math.Max(maxY, y);
            }
        }

        doc.MinX = minX; doc.MaxX = maxX; doc.MinY = minY; doc.MaxY = maxY;
        return doc;
    }

    public static GCodeDocument LoadFile(string path) =>
        Parse(File.ReadAllText(path), path);

    /// <summary>
    /// Formats a coordinate/number using invariant culture with up to 4 decimals,
    /// trimming trailing zeros. Shared by the import/transform services so all
    /// generated G-code uses a consistent numeric style.
    /// </summary>
    public static string FormatNumber(double value) =>
        value.ToString("0.####", CultureInfo.InvariantCulture);

    /// <summary>
    /// Rebuilds a minimal, valid G-code text listing (G0/G1 absolute mm moves)
    /// plus the matching <see cref="GCodeSegment"/> list and bounding box from a
    /// flat list of segments. Used by import services (after generating geometry)
    /// and by <c>GCodeTransformService</c> (after mutating segments) to regenerate
    /// a consistent <see cref="GCodeDocument"/> without hand-rolling text output.
    /// </summary>
    public static GCodeDocument BuildFromSegments(IReadOnlyList<GCodeSegment> segments, double feedXY = 1000, string? path = null)
    {
        var doc = new GCodeDocument { FilePath = path ?? "" };
        doc.Lines.Add("G21 (millimeters)");
        doc.Lines.Add("G90 (absolute)");

        bool havePos = false;
        double curX = 0, curY = 0;
        bool first = true;
        double minX = 0, minY = 0, maxX = 0, maxY = 0;

        foreach (var s in segments)
        {
            if (!havePos || Math.Abs(curX - s.X0) > 1e-6 || Math.Abs(curY - s.Y0) > 1e-6)
            {
                doc.Lines.Add($"G0 X{FormatNumber(s.X0)} Y{FormatNumber(s.Y0)}");
            }

            var word = s.Rapid ? "G0" : "G1";
            var feed = s.Rapid ? "" : $" F{FormatNumber(feedXY)}";
            doc.Lines.Add($"{word} X{FormatNumber(s.X1)} Y{FormatNumber(s.Y1)}{feed}");

            doc.Segments.Add(new GCodeSegment { Rapid = s.Rapid, X0 = s.X0, Y0 = s.Y0, X1 = s.X1, Y1 = s.Y1 });
            curX = s.X1; curY = s.Y1; havePos = true;

            if (first) { minX = maxX = Math.Min(s.X0, s.X1); minY = maxY = Math.Min(s.Y0, s.Y1); first = false; }
            minX = Math.Min(minX, Math.Min(s.X0, s.X1)); maxX = Math.Max(maxX, Math.Max(s.X0, s.X1));
            minY = Math.Min(minY, Math.Min(s.Y0, s.Y1)); maxY = Math.Max(maxY, Math.Max(s.Y0, s.Y1));
        }

        doc.MinX = minX; doc.MaxX = maxX; doc.MinY = minY; doc.MaxY = maxY;
        return doc;
    }
}
