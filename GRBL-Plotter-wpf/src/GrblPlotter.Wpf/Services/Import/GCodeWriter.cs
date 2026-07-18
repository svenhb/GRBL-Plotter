using GrblPlotter.Wpf.Services;
using System.Text;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>
/// Minimal G-code text builder shared by the import services. Emits simple,
/// absolute, metric G0/G1 motion using a configurable pen-up/pen-down (Z) style
/// that suits laser/plotter/router jobs alike. This is intentionally not a
/// general-purpose G-code emitter - just enough to turn a set of 2D polylines
/// (and single points, for drilling/flashes) into valid, previewable G-code.
/// </summary>
internal sealed class GCodeWriter
{
    private readonly StringBuilder _sb = new();
    private double _curX;
    private double _curY;
    private bool _havePos;
    private bool _penDown;

    public double FeedXY { get; set; } = 1000;
    public double FeedZ { get; set; } = 300;
    public double ZUp { get; set; } = 2;
    public double ZDown { get; set; } = -1;

    public static string F(double v) => GCodeParser.FormatNumber(v);

    public void Header(string title)
    {
        Comment(title);
        Raw("G21 (millimeters)");
        Raw("G90 (absolute)");
        // Note: deliberately no leading "G0 Z.." here - GCodeParser.Parse treats any
        // G0/G1 line as a motion segment even without X/Y, and emitting one before the
        // first real move would anchor a bogus (0,0) point into the bounding box.
    }

    public void Footer()
    {
        Retract();
        Raw("M2");
    }

    public void Comment(string text) => _sb.AppendLine($"({text.Replace('(', '[').Replace(')', ']')})");

    public void Raw(string line) => _sb.AppendLine(line);

    /// <summary>Rapid move (lifting the tool first if it was down).</summary>
    public void MoveTo(double x, double y)
    {
        Retract();
        Raw($"G0 X{F(x)} Y{F(y)}");
        _curX = x; _curY = y; _havePos = true;
    }

    /// <summary>Linear (cutting/drawing) move, plunging the tool down first if needed.</summary>
    public void LineTo(double x, double y)
    {
        if (!_havePos) { MoveTo(x, y); return; }
        if (!_penDown) PlungeDown();
        Raw($"G1 X{F(x)} Y{F(y)} F{F(FeedXY)}");
        _curX = x; _curY = y;
    }

    public void PlungeDown()
    {
        Raw($"G1 Z{F(ZDown)} F{F(FeedZ)}");
        _penDown = true;
    }

    public void Retract()
    {
        if (_penDown)
        {
            Raw($"G0 Z{F(ZUp)}");
            _penDown = false;
        }
    }

    /// <summary>Rapid to a point, plunge, retract - used for drills and Gerber flashes.</summary>
    public void Dot(double x, double y)
    {
        MoveTo(x, y);
        PlungeDown();
        Retract();
    }

    /// <summary>Draws a full polyline: rapid to the first point, then cut through the rest.</summary>
    public void DrawPolyline(IReadOnlyList<(double X, double Y)> points, bool closed = false)
    {
        if (points.Count == 0) return;
        if (points.Count == 1) { Dot(points[0].X, points[0].Y); return; }

        MoveTo(points[0].X, points[0].Y);
        for (int i = 1; i < points.Count; i++) LineTo(points[i].X, points[i].Y);
        if (closed) LineTo(points[0].X, points[0].Y);
    }

    public (double X, double Y) CurrentPosition => (_curX, _curY);
    public bool PenIsDown => _penDown;

    public string ToText() => _sb.ToString();
}
