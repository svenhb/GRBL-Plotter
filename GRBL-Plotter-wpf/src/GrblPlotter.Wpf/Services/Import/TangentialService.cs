using System.Globalization;
using System.Text;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>Adds tangential-axis (A) angle words along path direction — WinForms importGCTangential subset.</summary>
public static class TangentialService
{
    public static GCodeDocument Apply(GCodeDocument doc, string rotaryAxis = "A", double angleOffsetDeg = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("; tangential axis applied");
        sb.AppendLine("G21 G90 G94");
        double x = 0, y = 0;
        bool have = false;
        foreach (var s in doc.Segments)
        {
            double dx = s.X1 - s.X0, dy = s.Y1 - s.Y0;
            double ang = Math.Atan2(dy, dx) * 180.0 / Math.PI + angleOffsetDeg;
            if (!have || Math.Abs(x - s.X0) > 1e-6 || Math.Abs(y - s.Y0) > 1e-6)
                sb.AppendLine(FormattableString.Invariant($"G0 X{s.X0:0.####} Y{s.Y0:0.####} {rotaryAxis}{ang:0.###}"));
            var g = s.Rapid ? "G0" : "G1";
            sb.AppendLine(FormattableString.Invariant($"{g} X{s.X1:0.####} Y{s.Y1:0.####} {rotaryAxis}{ang:0.###}"));
            x = s.X1; y = s.Y1; have = true;
        }
        sb.AppendLine("M2");
        return GCodeParser.Parse(sb.ToString(), doc.FilePath);
    }
}
