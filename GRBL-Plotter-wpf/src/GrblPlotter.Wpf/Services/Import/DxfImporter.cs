using System.Globalization;
using System.IO;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>
/// Minimal ASCII DXF importer supporting LINE, CIRCLE, ARC, LWPOLYLINE and the
/// classic POLYLINE/VERTEX/SEQEND entities. Curved entities are approximated
/// with straight segments (circles as 36-gon, arcs at ~10 degrees/segment).
/// Coordinates are assumed to already be in millimeters (no unit conversion).
/// </summary>
public static class DxfImporter
{
    public static GCodeDocument Load(string path) => Parse(File.ReadAllText(path), path);

    public static GCodeDocument Parse(string dxfContent, string? path = null)
    {
        var writer = new GCodeWriter();
        writer.Header("DXF import");

        try
        {
            var pairs = ReadPairs(dxfContent);
            int i = 0;
            List<(double X, double Y)>? polylineVertices = null;
            bool polylineClosed = false;

            while (i < pairs.Count)
            {
                var (code, value) = pairs[i];
                if (code != 0) { i++; continue; }

                switch (value)
                {
                    case "LINE":
                    {
                        var g = ReadEntityGroups(pairs, ref i);
                        var p0 = (D(g, 10), D(g, 20));
                        var p1 = (D(g, 11), D(g, 21));
                        writer.DrawPolyline(new List<(double X, double Y)> { p0, p1 });
                        break;
                    }
                    case "CIRCLE":
                    {
                        var g = ReadEntityGroups(pairs, ref i);
                        double cx = D(g, 10), cy = D(g, 20), r = D(g, 40);
                        writer.DrawPolyline(Circle(cx, cy, r, 36), closed: true);
                        break;
                    }
                    case "ARC":
                    {
                        var g = ReadEntityGroups(pairs, ref i);
                        double cx = D(g, 10), cy = D(g, 20), r = D(g, 40);
                        double a0 = D(g, 50), a1 = D(g, 51);
                        writer.DrawPolyline(Arc(cx, cy, r, a0, a1));
                        break;
                    }
                    case "LWPOLYLINE":
                    {
                        var g = ReadEntityGroups(pairs, ref i);
                        var xs = g.TryGetValue(10, out var lx) ? lx : new List<string>();
                        var ys = g.TryGetValue(20, out var ly) ? ly : new List<string>();
                        int flag = g.TryGetValue(70, out var f) && f.Count > 0 ? (int)ParseD(f[0]) : 0;
                        var pts = new List<(double X, double Y)>();
                        for (int k = 0; k < Math.Min(xs.Count, ys.Count); k++)
                            pts.Add((ParseD(xs[k]), ParseD(ys[k])));
                        writer.DrawPolyline(pts, closed: (flag & 1) == 1);
                        break;
                    }
                    case "POLYLINE":
                    {
                        var g = ReadEntityGroups(pairs, ref i);
                        int flag = g.TryGetValue(70, out var f) && f.Count > 0 ? (int)ParseD(f[0]) : 0;
                        polylineVertices = new List<(double X, double Y)>();
                        polylineClosed = (flag & 1) == 1;
                        break;
                    }
                    case "VERTEX":
                    {
                        var g = ReadEntityGroups(pairs, ref i);
                        polylineVertices?.Add((D(g, 10), D(g, 20)));
                        break;
                    }
                    case "SEQEND":
                    {
                        if (polylineVertices is { Count: > 0 })
                            writer.DrawPolyline(polylineVertices, closed: polylineClosed);
                        polylineVertices = null;
                        i++;
                        break;
                    }
                    default:
                        i++;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            writer.Comment($"DXF parse error: {ex.Message}");
        }

        writer.Footer();
        return GCodeParser.Parse(writer.ToText(), path);
    }

    private static double D(Dictionary<int, List<string>> g, int code) =>
        g.TryGetValue(code, out var v) && v.Count > 0 ? ParseD(v[0]) : 0;

    private static double ParseD(string s) => double.Parse(s.Trim(), CultureInfo.InvariantCulture);

    private static List<(double X, double Y)> Circle(double cx, double cy, double r, int segments)
    {
        var pts = new List<(double X, double Y)>(segments);
        for (int i = 0; i < segments; i++)
        {
            double a = 2 * Math.PI * i / segments;
            pts.Add((cx + r * Math.Cos(a), cy + r * Math.Sin(a)));
        }
        return pts;
    }

    private static List<(double X, double Y)> Arc(double cx, double cy, double r, double startDeg, double endDeg)
    {
        double sweep = endDeg - startDeg;
        while (sweep <= 0) sweep += 360;
        int segs = Math.Max(2, (int)Math.Ceiling(sweep / 10.0));
        var pts = new List<(double X, double Y)>(segs + 1);
        for (int i = 0; i <= segs; i++)
        {
            double a = (startDeg + sweep * i / segs) * Math.PI / 180.0;
            pts.Add((cx + r * Math.Cos(a), cy + r * Math.Sin(a)));
        }
        return pts;
    }

    /// <summary>Reads the group-code/value pairs belonging to the current entity (until the next 0-code) without consuming it.</summary>
    private static Dictionary<int, List<string>> ReadEntityGroups(List<(int Code, string Value)> pairs, ref int i)
    {
        var result = new Dictionary<int, List<string>>();
        i++; // skip the "0 <ENTITY>" marker itself
        while (i < pairs.Count && pairs[i].Code != 0)
        {
            var (code, value) = pairs[i];
            if (!result.TryGetValue(code, out var list)) { list = new List<string>(); result[code] = list; }
            list.Add(value);
            i++;
        }
        return result;
    }

    private static List<(int Code, string Value)> ReadPairs(string text)
    {
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var result = new List<(int Code, string Value)>(lines.Length / 2);
        for (int i = 0; i + 1 < lines.Length; i += 2)
        {
            var codeStr = lines[i].Trim();
            var value = lines[i + 1].TrimEnd('\r').Trim();
            if (!int.TryParse(codeStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var code)) continue;
            result.Add((code, value));
        }
        return result;
    }
}
