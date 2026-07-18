using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>Generates simple, parametric shapes directly as G-code (all units in mm, all angles in degrees).</summary>
public static class ShapeGenerator
{
    public static GCodeDocument Rectangle(double x, double y, double width, double height, double cornerRadius = 0, string? path = null)
    {
        var writer = new GCodeWriter();
        writer.Header("Rectangle");

        if (cornerRadius > 0 && cornerRadius * 2 < Math.Min(width, height))
        {
            var r = cornerRadius;
            var pts = new List<(double X, double Y)>();
            pts.AddRange(ArcPoints(x + width - r, y + r, r, -90, 0));
            pts.AddRange(ArcPoints(x + width - r, y + height - r, r, 0, 90));
            pts.AddRange(ArcPoints(x + r, y + height - r, r, 90, 180));
            pts.AddRange(ArcPoints(x + r, y + r, r, 180, 270));
            writer.DrawPolyline(pts, closed: true);
        }
        else
        {
            writer.DrawPolyline(new List<(double X, double Y)>
            {
                (x, y), (x + width, y), (x + width, y + height), (x, y + height)
            }, closed: true);
        }

        writer.Footer();
        return GCodeParser.Parse(writer.ToText(), path);
    }

    public static GCodeDocument Circle(double centerX, double centerY, double radius, int segments = 72, string? path = null) =>
        Ellipse(centerX, centerY, radius, radius, 0, segments, path);

    public static GCodeDocument Ellipse(double centerX, double centerY, double radiusX, double radiusY, double rotationDeg = 0, int segments = 72, string? path = null)
    {
        var writer = new GCodeWriter();
        writer.Header("Ellipse");

        double rot = rotationDeg * Math.PI / 180.0;
        double cosR = Math.Cos(rot), sinR = Math.Sin(rot);
        var pts = new List<(double X, double Y)>(segments);
        for (int i = 0; i < segments; i++)
        {
            double a = 2 * Math.PI * i / segments;
            double ex = radiusX * Math.Cos(a), ey = radiusY * Math.Sin(a);
            pts.Add((centerX + ex * cosR - ey * sinR, centerY + ex * sinR + ey * cosR));
        }
        writer.DrawPolyline(pts, closed: true);

        writer.Footer();
        return GCodeParser.Parse(writer.ToText(), path);
    }

    /// <summary>Regular polygon with the given number of sides, inscribed in a circle of the given radius.</summary>
    public static GCodeDocument Polygon(double centerX, double centerY, double radius, int sides, double rotationDeg = 0, string? path = null)
    {
        sides = Math.Max(3, sides);
        var writer = new GCodeWriter();
        writer.Header("Polygon");

        double rot = rotationDeg * Math.PI / 180.0;
        var pts = new List<(double X, double Y)>(sides);
        for (int i = 0; i < sides; i++)
        {
            double a = rot + 2 * Math.PI * i / sides;
            pts.Add((centerX + radius * Math.Cos(a), centerY + radius * Math.Sin(a)));
        }
        writer.DrawPolyline(pts, closed: true);

        writer.Footer();
        return GCodeParser.Parse(writer.ToText(), path);
    }

    private static List<(double X, double Y)> ArcPoints(double cx, double cy, double r, double startDeg, double endDeg, int segments = 8)
    {
        var pts = new List<(double X, double Y)>(segments + 1);
        for (int i = 0; i <= segments; i++)
        {
            double a = (startDeg + (endDeg - startDeg) * i / segments) * Math.PI / 180.0;
            pts.Add((cx + r * Math.Cos(a), cy + r * Math.Sin(a)));
        }
        return pts;
    }
}
