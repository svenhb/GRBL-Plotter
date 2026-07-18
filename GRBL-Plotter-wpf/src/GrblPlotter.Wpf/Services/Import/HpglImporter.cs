using System.Globalization;
using System.IO;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>
/// Minimal HPGL/HPGL2 importer. Supports PU (pen up move), PD (pen down draw),
/// PA (absolute mode, optionally followed by coordinates), PR (relative mode,
/// optionally followed by coordinate deltas) and SP (pen select - emitted as a
/// comment only). Plotter units (1/1016 inch) are converted to millimeters.
/// </summary>
public static class HpglImporter
{
    private const double MmPerUnit = 25.4 / 1016.0;

    public static GCodeDocument Load(string path) => Parse(File.ReadAllText(path), path);

    public static GCodeDocument Parse(string hpglContent, string? path = null)
    {
        var writer = new GCodeWriter();
        writer.Header("HPGL import");

        try
        {
            bool absolute = true;
            bool penDown = false;
            double curX = 0, curY = 0; // plotter units

            foreach (var raw in hpglContent.Split(';'))
            {
                var cmd = raw.Trim();
                if (cmd.Length < 2) continue;

                var mnemonic = cmd.Substring(0, 2).ToUpperInvariant();
                var argsStr = cmd.Substring(2).Trim();
                var nums = ParseNumbers(argsStr);

                switch (mnemonic)
                {
                    case "PU":
                        penDown = false;
                        ApplyPoints(nums, ref curX, ref curY, absolute, penDown, writer);
                        break;
                    case "PD":
                        penDown = true;
                        ApplyPoints(nums, ref curX, ref curY, absolute, penDown, writer);
                        break;
                    case "PA":
                        absolute = true;
                        if (nums.Count > 0) ApplyPoints(nums, ref curX, ref curY, absolute, penDown, writer);
                        break;
                    case "PR":
                        absolute = false;
                        if (nums.Count > 0) ApplyPoints(nums, ref curX, ref curY, absolute, penDown, writer);
                        break;
                    case "SP":
                        writer.Comment($"Pen select: {argsStr}");
                        break;
                    default:
                        // IN, IP, SC, VS, etc. are not needed for a basic geometry preview - ignore.
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            writer.Comment($"HPGL parse error: {ex.Message}");
        }

        writer.Footer();
        return GCodeParser.Parse(writer.ToText(), path);
    }

    private static void ApplyPoints(List<double> nums, ref double curX, ref double curY, bool absolute, bool penDown, GCodeWriter writer)
    {
        for (int i = 0; i + 1 < nums.Count; i += 2)
        {
            double ux = nums[i], uy = nums[i + 1];
            double tx = absolute ? ux : curX + ux;
            double ty = absolute ? uy : curY + uy;

            double mmX = tx * MmPerUnit, mmY = ty * MmPerUnit;
            if (penDown) writer.LineTo(mmX, mmY);
            else writer.MoveTo(mmX, mmY);

            curX = tx; curY = ty;
        }
    }

    private static List<double> ParseNumbers(string s)
    {
        var result = new List<double>();
        if (string.IsNullOrWhiteSpace(s)) return result;
        foreach (var part in s.Split(','))
        {
            var trimmed = part.Trim();
            if (trimmed.Length == 0) continue;
            if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                result.Add(v);
        }
        return result;
    }
}
