using System.Globalization;
using System.Text;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>Simple axis-aligned hatch fill over a bounding box (incremental Graphic pipeline).</summary>
public static class HatchService
{
    public static string GenerateHatch(double minX, double minY, double maxX, double maxY, double spacing = 1.0, double angleDeg = 0)
    {
        if (spacing <= 0) spacing = 1;
        var sb = new StringBuilder();
        sb.AppendLine("; hatch fill");
        sb.AppendLine("G21 G90 G94");
        sb.AppendLine("G0 Z2");

        double w = maxX - minX, h = maxY - minY;
        if (w <= 0 || h <= 0) { sb.AppendLine("M2"); return sb.ToString(); }

        // Generate horizontal lines then rotate around center
        double cx = (minX + maxX) / 2, cy = (minY + maxY) / 2;
        double rad = angleDeg * Math.PI / 180;
        double cos = Math.Cos(rad), sin = Math.Sin(rad);

        bool flip = false;
        for (double y = minY; y <= maxY + 1e-9; y += spacing)
        {
            double x0 = minX, x1 = maxX;
            if (flip) (x0, x1) = (x1, x0);
            var (ax, ay) = Rot(x0, y);
            var (bx, by) = Rot(x1, y);
            sb.AppendLine(FormattableString.Invariant($"G0 X{ax:0.###} Y{ay:0.###}"));
            sb.AppendLine("G1 Z-0.1 F400");
            sb.AppendLine(FormattableString.Invariant($"G1 X{bx:0.###} Y{by:0.###} F1000"));
            sb.AppendLine("G0 Z2");
            flip = !flip;
        }
        sb.AppendLine("G0 X0 Y0");
        sb.AppendLine("M2");
        return sb.ToString();

        (double X, double Y) Rot(double x, double y)
        {
            double dx = x - cx, dy = y - cy;
            return (cx + dx * cos - dy * sin, cy + dx * sin + dy * cos);
        }
    }

    public static GCodeDocument HatchDocument(GCodeDocument source, double spacing = 1.0, double angleDeg = 45)
    {
        var text = GenerateHatch(source.MinX, source.MinY, source.MaxX, source.MaxY, spacing, angleDeg);
        var hatch = GCodeParser.Parse(text, "hatch.nc");
        var merged = new GCodeDocument { FilePath = source.FilePath };
        merged.Lines.AddRange(source.Lines);
        merged.Lines.Add("; --- hatch ---");
        merged.Lines.AddRange(hatch.Lines);
        merged.Segments.AddRange(source.Segments);
        merged.Segments.AddRange(hatch.Segments);
        merged.MinX = Math.Min(source.MinX, hatch.MinX);
        merged.MaxX = Math.Max(source.MaxX, hatch.MaxX);
        merged.MinY = Math.Min(source.MinY, hatch.MinY);
        merged.MaxY = Math.Max(source.MaxY, hatch.MaxY);
        return merged;
    }
}
