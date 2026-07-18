using System.Text;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>Minimal Hershey-style stroke font for A–Z / 0–9 (Phase 6).</summary>
public static class HersheyStrokeFont
{
    // Each glyph: list of polylines in unit square (0..1), Y up
    private static readonly Dictionary<char, string> Glyphs = new()
    {
        ['A'] = "0,0 0.5,1 1,0;0.25,0.4 0.75,0.4",
        ['B'] = "0,0 0,1 0.7,1 0.9,0.8 0.7,0.5 0,0.5;0.7,0.5 0.95,0.3 0.7,0 0,0",
        ['C'] = "1,0.2 0.7,0 0.3,0 0,0.3 0,0.7 0.3,1 0.7,1 1,0.8",
        ['0'] = "0.2,0 0.8,0 1,0.2 1,0.8 0.8,1 0.2,1 0,0.8 0,0.2 0.2,0",
        ['1'] = "0.3,0.8 0.5,1 0.5,0;0.3,0 0.7,0",
        ['2'] = "0,0.8 0.2,1 0.8,1 1,0.8 1,0.6 0,0 1,0",
        [' '] = "",
    };

    public static GCodeDocument Generate(string text, double heightMm = 10, double x0 = 0, double y0 = 0, double spacing = 0.2)
    {
        var writer = new GCodeWriter();
        writer.Header("Hershey stroke text");
        double cursor = x0;
        double w = heightMm * 0.7;
        foreach (var ch in text.ToUpperInvariant())
        {
            if (!Glyphs.TryGetValue(ch, out var def))
                def = Glyphs['A'];
            if (string.IsNullOrEmpty(def)) { cursor += w * 0.5; continue; }
            foreach (var poly in def.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var pts = new List<(double X, double Y)>();
                foreach (var pair in poly.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    var xy = pair.Split(',');
                    if (xy.Length != 2) continue;
                    double.TryParse(xy[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var u);
                    double.TryParse(xy[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v);
                    pts.Add((cursor + u * w, y0 + v * heightMm));
                }
                if (pts.Count > 0) writer.DrawPolyline(pts, closed: false);
            }
            cursor += w * (1 + spacing);
        }
        writer.Footer();
        return GCodeParser.Parse(writer.ToText(), "hershey.nc");
    }
}
