using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>
/// Imports drill points from either a simple CSV file ("x,y" or "x;y" per
/// line, optionally with a third depth column) or a basic Excellon-like drill
/// file (T tool selects, X..Y.. coordinate lines). Produces peck-drill G-code:
/// rapid to XY, plunge to depth, retract, for every point found.
/// </summary>
public static class CsvDrillImporter
{
    private static readonly Regex ExcellonXyRx = new(@"^(?:X(?<x>[-+]?\d*\.?\d*))?(?:Y(?<y>[-+]?\d*\.?\d*))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static double DefaultDepth { get; set; } = -1.6;
    public static double DefaultFeedZ { get; set; } = 200;
    public static double DefaultZUp { get; set; } = 3;

    public static GCodeDocument Load(string path) => Parse(File.ReadAllText(path), path);

    public static GCodeDocument Parse(string content, string? path = null)
    {
        var writer = new GCodeWriter { ZUp = DefaultZUp, ZDown = DefaultDepth, FeedZ = DefaultFeedZ };
        writer.Header("Drill/CSV import");

        try
        {
            var lines = content.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            bool looksExcellon = lines.Any(l => Regex.IsMatch(l.Trim(), @"^T\d+", RegexOptions.IgnoreCase) && !l.Contains(','))
                                  || lines.Any(l => l.Trim().Equals("M48", StringComparison.OrdinalIgnoreCase));

            int points = 0;
            if (looksExcellon)
                points = ParseExcellon(lines, writer);
            else
                points = ParseCsv(lines, writer);

            if (points == 0)
            {
                // Fall back to trying the other format, in case detection guessed wrong.
                points = looksExcellon ? ParseCsv(lines, writer) : ParseExcellon(lines, writer);
            }

            if (points == 0)
                writer.Comment("No drill points could be parsed from this file.");
        }
        catch (Exception ex)
        {
            writer.Comment($"Drill/CSV parse error: {ex.Message}");
        }

        writer.Footer();
        return GCodeParser.Parse(writer.ToText(), path);
    }

    private static int ParseCsv(string[] lines, GCodeWriter writer)
    {
        int count = 0;
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#') || line.StartsWith(';') || line.StartsWith('(')) continue;

            var sep = line.Contains(';') ? ';' : ',';
            var parts = line.Split(sep, StringSplitOptions.TrimEntries);
            if (parts.Length < 2) continue;
            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)) continue;
            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)) continue;

            double depth = writer.ZDown;
            if (parts.Length >= 3 && double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
                depth = z;

            Drill(writer, x, y, depth);
            count++;
        }
        return count;
    }

    private static int ParseExcellon(string[] lines, GCodeWriter writer)
    {
        int count = 0;
        double lastX = 0, lastY = 0;
        bool metric = true;

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0) continue;

            if (line.Equals("METRIC", StringComparison.OrdinalIgnoreCase) || line.StartsWith("M71", StringComparison.OrdinalIgnoreCase)) { metric = true; continue; }
            if (line.Equals("INCH", StringComparison.OrdinalIgnoreCase) || line.StartsWith("M72", StringComparison.OrdinalIgnoreCase)) { metric = false; continue; }
            if (Regex.IsMatch(line, @"^T\d+", RegexOptions.IgnoreCase)) { writer.Comment($"Tool: {line}"); continue; } // tool select/definition
            if (line.Equals("M30", StringComparison.OrdinalIgnoreCase) || line.Equals("M00", StringComparison.OrdinalIgnoreCase)) break;
            if (line.StartsWith('%') || line.Equals("M48", StringComparison.OrdinalIgnoreCase) || line.Equals("%", StringComparison.OrdinalIgnoreCase)) continue;

            if (line[0] != 'X' && line[0] != 'x' && line[0] != 'Y' && line[0] != 'y') continue;
            var m = ExcellonXyRx.Match(line);
            if (!m.Success || (!m.Groups["x"].Success && !m.Groups["y"].Success)) continue;

            double x = m.Groups["x"].Success && m.Groups["x"].Value.Length > 0 ? ExcellonCoord(m.Groups["x"].Value, metric) : lastX;
            double y = m.Groups["y"].Success && m.Groups["y"].Value.Length > 0 ? ExcellonCoord(m.Groups["y"].Value, metric) : lastY;

            Drill(writer, x, y, writer.ZDown);
            lastX = x; lastY = y;
            count++;
        }
        return count;
    }

    /// <summary>
    /// Excellon coordinates may carry an explicit decimal point (used directly) or
    /// be a fixed-point integer with an implied decimal (commonly 3 decimals for
    /// metric, or 1/10000" for inch files without a decimal point).
    /// </summary>
    private static double ExcellonCoord(string raw, bool metric)
    {
        if (raw.Contains('.'))
            return double.Parse(raw, NumberStyles.Float, CultureInfo.InvariantCulture);

        if (!long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)) return 0;
        return metric ? v / 1000.0 : (v / 10000.0) * 25.4;
    }

    private static void Drill(GCodeWriter writer, double x, double y, double depth)
    {
        writer.MoveTo(x, y);
        var savedZDown = writer.ZDown;
        writer.ZDown = depth;
        writer.PlungeDown();
        writer.Retract();
        writer.ZDown = savedZDown;
    }
}
