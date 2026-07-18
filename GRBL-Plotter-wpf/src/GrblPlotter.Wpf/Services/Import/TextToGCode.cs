using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>
/// Turns a text string into engraving/plotting G-code by asking WPF to build the
/// glyph outline geometry for the requested font (<see cref="FormattedText"/>),
/// flattening any curves to line segments, and drawing each resulting contour.
/// </summary>
public static class TextToGCode
{
    /// <param name="text">The string to render.</param>
    /// <param name="fontFamily">Installed font family name, e.g. "Arial".</param>
    /// <param name="heightMm">Nominal text (em) height in millimeters.</param>
    /// <param name="x0">Left position of the text baseline area, in mm.</param>
    /// <param name="y0">Baseline/bottom reference position, in mm.</param>
    /// <param name="bold">Use a bold weight.</param>
    /// <param name="italic">Use an italic style.</param>
    /// <param name="flattenToleranceMm">Curve flattening tolerance (smaller = smoother, more points).</param>
    public static GCodeDocument Generate(
        string text,
        string fontFamily = "Arial",
        double heightMm = 10,
        double x0 = 0,
        double y0 = 0,
        bool bold = false,
        bool italic = false,
        double flattenToleranceMm = 0.1,
        string? path = null)
    {
        var writer = new GCodeWriter();
        writer.Header("Text import");

        try
        {
            const double pxPerMm = 96.0 / 25.4;
            double emSizePx = heightMm * pxPerMm;

            var typeface = new Typeface(
                new FontFamily(fontFamily),
                italic ? FontStyles.Italic : FontStyles.Normal,
                bold ? FontWeights.Bold : FontWeights.Normal,
                FontStretches.Normal);

            var formatted = new FormattedText(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                emSizePx,
                Brushes.Black,
                1.0);

            var geometry = formatted.BuildGeometry(new Point(0, 0));
            var flattened = geometry.GetFlattenedPathGeometry(flattenToleranceMm * pxPerMm, ToleranceType.Absolute);

            double totalHeightPx = formatted.Height;

            foreach (var figure in flattened.Figures)
            {
                var pts = new List<(double X, double Y)> { ToMm(figure.StartPoint, totalHeightPx, pxPerMm, x0, y0) };
                foreach (var seg in figure.Segments)
                {
                    if (seg is PolyLineSegment poly)
                        foreach (var p in poly.Points) pts.Add(ToMm(p, totalHeightPx, pxPerMm, x0, y0));
                    else if (seg is LineSegment line)
                        pts.Add(ToMm(line.Point, totalHeightPx, pxPerMm, x0, y0));
                }
                writer.DrawPolyline(pts, closed: figure.IsClosed);
            }

            if (flattened.Figures.Count == 0)
                writer.Comment($"No glyph outlines were produced for '{text}' with font '{fontFamily}'.");
        }
        catch (Exception ex)
        {
            writer.Comment($"Text import error: {ex.Message}");
        }

        writer.Footer();
        return GCodeParser.Parse(writer.ToText(), path);
    }

    private static (double X, double Y) ToMm(Point p, double totalHeightPx, double pxPerMm, double x0, double y0)
    {
        double mmX = x0 + p.X / pxPerMm;
        double mmY = y0 + (totalHeightPx - p.Y) / pxPerMm;
        return (mmX, mmY);
    }
}
