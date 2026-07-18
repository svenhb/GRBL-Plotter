using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace GrblPlotter.Wpf.Services.Preview;

/// <summary>Builds WPF geometries for ruler / machine limits / tool table markers.</summary>
public static class PreviewOverlayBuilder
{
    public sealed class RulerLabel
    {
        public double X { get; init; }
        public double Y { get; init; }
        public string Text { get; init; } = "";
    }

    public sealed class RulerOverlay
    {
        public Geometry Ticks { get; set; } = Geometry.Empty;
        public Geometry Grid { get; set; } = Geometry.Empty;
        public List<RulerLabel> Labels { get; } = new();
    }

    public static Geometry BuildMachineLimits(double minX, double minY, double maxX, double maxY,
        Func<double, double, Point> map)
    {
        var g = new StreamGeometry();
        using var ctx = g.Open();
        var p0 = map(minX, minY);
        ctx.BeginFigure(p0, false, true);
        ctx.LineTo(map(maxX, minY), true, false);
        ctx.LineTo(map(maxX, maxY), true, false);
        ctx.LineTo(map(minX, maxY), true, false);
        g.Freeze();
        return g;
    }

    /// <summary>
    /// CAD-style ruler: major/minor ticks on bottom (X) and left (Y), optional light grid, numeric labels.
    /// Tick lengths are expressed in screen pixels via <paramref name="pixelsPerUnit"/>.
    /// </summary>
    public static RulerOverlay BuildRuler(double minX, double minY, double maxX, double maxY,
        Func<double, double, Point> map, double pixelsPerUnit, double rulerBandPx = 28)
    {
        var result = new RulerOverlay();
        if (maxX <= minX || maxY <= minY || pixelsPerUnit <= 1e-9)
            return result;

        double span = Math.Max(maxX - minX, maxY - minY);
        double major = NiceStep(span / 6);
        double med = major / 2;
        double minor = major / 10;
        if (minor * pixelsPerUnit < 4) minor = med;
        if (med * pixelsPerUnit < 6) med = major;

        double px(double screenPx) => screenPx / pixelsPerUnit;

        var ticks = new StreamGeometry();
        var grid = new StreamGeometry();
        using (var t = ticks.Open())
        using (var g = grid.Open())
        {
            // Drawing frame
            t.BeginFigure(map(minX, minY), false, true);
            t.LineTo(map(maxX, minY), true, false);
            t.LineTo(map(maxX, maxY), true, false);
            t.LineTo(map(minX, maxY), true, false);

            // Outer ruler band edges
            double band = px(rulerBandPx);
            t.BeginFigure(map(minX, minY - band), false, false);
            t.LineTo(map(maxX, minY - band), true, false);
            t.BeginFigure(map(minX - band, minY), false, false);
            t.LineTo(map(minX - band, maxY), true, false);

            // X ticks along bottom (below minY)
            for (double x = Math.Ceiling(minX / minor) * minor; x <= maxX + 1e-9; x += minor)
            {
                bool isMajor = NearlyMultiple(x, major);
                bool isMed = !isMajor && NearlyMultiple(x, med);
                double len = isMajor ? band * 0.85 : isMed ? band * 0.55 : band * 0.3;
                var a = map(x, minY);
                var b = map(x, minY - len);
                t.BeginFigure(a, false, false);
                t.LineTo(b, true, false);

                if (isMajor)
                {
                    g.BeginFigure(map(x, minY), false, false);
                    g.LineTo(map(x, maxY), true, false);
                    var lp = map(x, minY - band * 0.92);
                    result.Labels.Add(new RulerLabel
                    {
                        X = lp.X - 10,
                        Y = lp.Y - 2,
                        Text = FormatTick(x)
                    });
                }
            }

            // Y ticks along left (left of minX); CNC Y-up → screen Y decreases upward
            for (double y = Math.Ceiling(minY / minor) * minor; y <= maxY + 1e-9; y += minor)
            {
                bool isMajor = NearlyMultiple(y, major);
                bool isMed = !isMajor && NearlyMultiple(y, med);
                double len = isMajor ? band * 0.85 : isMed ? band * 0.55 : band * 0.3;
                var a = map(minX, y);
                var b = map(minX - len, y);
                t.BeginFigure(a, false, false);
                t.LineTo(b, true, false);

                if (isMajor)
                {
                    g.BeginFigure(map(minX, y), false, false);
                    g.LineTo(map(maxX, y), true, false);
                    var lp = map(minX - band * 0.95, y);
                    result.Labels.Add(new RulerLabel
                    {
                        X = Math.Max(0, lp.X - 2),
                        Y = lp.Y - 6,
                        Text = FormatTick(y)
                    });
                }
            }
        }

        ticks.Freeze();
        grid.Freeze();
        result.Ticks = ticks;
        result.Grid = grid;
        return result;
    }

    public static Geometry BuildToolMarkers(IEnumerable<(double X, double Y)> tools, Func<double, double, Point> map, double size = 3)
    {
        var g = new StreamGeometry();
        using var ctx = g.Open();
        foreach (var (x, y) in tools)
        {
            var c = map(x, y);
            ctx.BeginFigure(new Point(c.X - size, c.Y), false, false);
            ctx.LineTo(new Point(c.X + size, c.Y), true, false);
            ctx.BeginFigure(new Point(c.X, c.Y - size), false, false);
            ctx.LineTo(new Point(c.X, c.Y + size), true, false);
        }
        g.Freeze();
        return g;
    }

    private static bool NearlyMultiple(double value, double step)
    {
        if (step <= 0) return false;
        double q = Math.Abs(value / step);
        return Math.Abs(q - Math.Round(q)) < 1e-6;
    }

    private static string FormatTick(double v)
    {
        if (Math.Abs(v) < 1e-9) return "0";
        if (Math.Abs(v - Math.Round(v)) < 1e-6)
            return ((long)Math.Round(v)).ToString(CultureInfo.InvariantCulture);
        return v.ToString("0.##", CultureInfo.InvariantCulture);
    }

    private static double NiceStep(double raw)
    {
        if (raw <= 0) return 10;
        double exp = Math.Pow(10, Math.Floor(Math.Log10(raw)));
        double f = raw / exp;
        double nf = f < 1.5 ? 1 : f < 3 ? 2 : f < 7 ? 5 : 10;
        return nf * exp;
    }
}
