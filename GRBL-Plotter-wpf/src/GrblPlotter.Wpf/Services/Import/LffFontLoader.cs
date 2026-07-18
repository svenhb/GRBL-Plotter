using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>LibreCAD Font Format (.lff) stroke font loader — WinForms data/fonts/lff parity.</summary>
public static class LffFontLoader
{
    private static readonly Regex VertexRx = new(@"\[?\s*(-?\d+\.?\d*)\s*,\s*(-?\d+\.?\d*)\s*\]?", RegexOptions.Compiled);

    public static string FontsDirectory =>
        Path.Combine(AppContext.BaseDirectory, "data", "fonts");

    public static IReadOnlyList<string> ListFonts()
    {
        var dir = Path.Combine(FontsDirectory, "lff");
        if (!Directory.Exists(dir)) return Array.Empty<string>();
        return Directory.GetFiles(dir, "*.lff")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(n => n != null)
            .Select(n => n!)
            .OrderBy(x => x)
            .ToList();
    }

    public static GCodeDocument Render(string text, string fontName, double heightMm, double x0 = 0, double y0 = 0)
    {
        var path = Path.Combine(FontsDirectory, "lff", fontName + ".lff");
        if (!File.Exists(path))
            path = Path.Combine(FontsDirectory, "lff", "romans.lff");
        if (!File.Exists(path))
            return HersheyStrokeFont.Generate(text, heightMm, x0, y0);

        var glyphs = ParseLff(File.ReadAllText(path));
        var writer = new GCodeWriter();
        writer.Header($"LFF font {Path.GetFileName(path)}");
        double cursor = x0;
        double scale = heightMm; // LFF coords typically 0..1 letter height units, or letter height ≈ 1
        // Detect letter height from 'A' or first glyph bbox
        double letterH = 1;
        if (glyphs.TryGetValue('A', out var ag) && ag.Count > 0)
        {
            var ys = ag.SelectMany(p => p).Select(v => v.Y);
            letterH = Math.Max(ys.Max() - ys.Min(), 1e-6);
        }
        scale = heightMm / letterH;

        foreach (var ch in text)
        {
            if (ch == ' ') { cursor += heightMm * 0.4; continue; }
            if (!glyphs.TryGetValue(ch, out var polys) && !glyphs.TryGetValue(char.ToUpperInvariant(ch), out polys))
            {
                cursor += heightMm * 0.5;
                continue;
            }
            double maxX = 0;
            foreach (var poly in polys)
            {
                var pts = poly.Select(v => (cursor + v.X * scale, y0 + v.Y * scale)).ToList();
                foreach (var p in poly) maxX = Math.Max(maxX, p.X);
                if (pts.Count > 0) writer.DrawPolyline(pts, false);
            }
            cursor += Math.Max(maxX * scale, heightMm * 0.35) + heightMm * 0.12;
        }
        writer.Footer();
        return GCodeParser.Parse(writer.ToText(), "lff-text.nc");
    }

    /// <summary>glyph → list of polylines (each polyline = list of vertices).</summary>
    public static Dictionary<char, List<List<(double X, double Y)>>> ParseLff(string content)
    {
        var map = new Dictionary<char, List<List<(double X, double Y)>>>();
        char? current = null;
        List<List<(double X, double Y)>>? polys = null;

        foreach (var raw in content.Replace("\r\n", "\n").Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#') || line.StartsWith("//")) continue;

            // Character header: [0041] or C41
            if (line.StartsWith('[') && line.Contains(']'))
            {
                var hex = line.Trim('[', ']').Split(' ', '\t')[0];
                if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var cp) && cp is >= 0 and <= 0xFFFF)
                {
                    current = (char)cp;
                    polys = new List<List<(double, double)>>();
                    map[current.Value] = polys;
                }
                continue;
            }
            if (polys == null || current == null) continue;

            // Vertex lines: x,y;x,y;... or blank separator between polylines
            if (line == ";" || line == ".")
            {
                polys.Add(new List<(double, double)>());
                continue;
            }

            var verts = new List<(double X, double Y)>();
            foreach (Match m in VertexRx.Matches(line))
            {
                double.TryParse(m.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var x);
                double.TryParse(m.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var y);
                verts.Add((x, y));
            }
            if (verts.Count > 0)
            {
                if (polys.Count == 0 || (polys[^1].Count > 0 && line.Contains(';') && verts.Count >= 2))
                {
                    // LibreCAD: multiple polylines separated by empty coords ""
                    var parts = line.Split(new[] { ",," }, StringSplitOptions.None);
                    foreach (var part in parts)
                    {
                        var pv = new List<(double, double)>();
                        foreach (Match m in VertexRx.Matches(part))
                        {
                            double.TryParse(m.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var x);
                            double.TryParse(m.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var y);
                            pv.Add((x, y));
                        }
                        if (pv.Count > 0) polys.Add(pv);
                    }
                }
                else
                    polys.Add(verts);
            }
        }
        return map;
    }
}
