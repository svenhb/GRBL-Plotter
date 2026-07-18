using System.Globalization;
using System.Text.RegularExpressions;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services;

public static class GrblStatusParser
{
    private static readonly Regex StatusRx = new(@"<(?<body>[^>]+)>", RegexOptions.Compiled);
    private static readonly Regex PairRx = new(@"(?<k>[A-Za-z]+):(?<v>[^|]+)", RegexOptions.Compiled);

    public static bool TryParse(string line, GrblStatusSnapshot target)
    {
        var m = StatusRx.Match(line);
        if (!m.Success) return false;

        target.Raw = line;
        target.Utc = DateTime.UtcNow;
        var body = m.Groups["body"].Value;
        var parts = body.Split('|');
        if (parts.Length == 0) return false;

        target.State = ParseState(parts[0]);

        for (int i = 1; i < parts.Length; i++)
        {
            var pm = PairRx.Match(parts[i]);
            if (!pm.Success) continue;
            var key = pm.Groups["k"].Value;
            var val = pm.Groups["v"].Value;
            switch (key)
            {
                case "MPos":
                    ParseAxes(val, target.Machine);
                    ApplyWco(target);
                    break;
                case "WPos":
                    ParseAxes(val, target.Work);
                    break;
                case "WCO":
                    ParseAxes(val, target.Wco);
                    ApplyWco(target);
                    break;
                case "FS":
                case "F":
                    ParseFs(val, target);
                    break;
                case "Ov":
                    ParseOv(val, target);
                    break;
            }
        }
        return true;
    }

    private static void ApplyWco(GrblStatusSnapshot s)
    {
        s.Work.X = s.Machine.X - s.Wco.X;
        s.Work.Y = s.Machine.Y - s.Wco.Y;
        s.Work.Z = s.Machine.Z - s.Wco.Z;
        s.Work.A = s.Machine.A - s.Wco.A;
        s.Work.B = s.Machine.B - s.Wco.B;
        s.Work.C = s.Machine.C - s.Wco.C;
    }

    private static GrblMachineState ParseState(string s)
    {
        var name = s.Split(':')[0].Trim();
        return Enum.TryParse<GrblMachineState>(name, true, out var st) ? st : GrblMachineState.Unknown;
    }

    private static void ParseAxes(string v, AxisPosition p)
    {
        var nums = v.Split(',');
        if (nums.Length > 0) p.X = ParseD(nums[0]);
        if (nums.Length > 1) p.Y = ParseD(nums[1]);
        if (nums.Length > 2) p.Z = ParseD(nums[2]);
        if (nums.Length > 3) p.A = ParseD(nums[3]);
        if (nums.Length > 4) p.B = ParseD(nums[4]);
        if (nums.Length > 5) p.C = ParseD(nums[5]);
    }

    private static void ParseFs(string v, GrblStatusSnapshot s)
    {
        var nums = v.Split(',');
        if (nums.Length > 0) s.Feed = ParseD(nums[0]);
        if (nums.Length > 1) s.Spindle = ParseD(nums[1]);
    }

    private static void ParseOv(string v, GrblStatusSnapshot s)
    {
        var nums = v.Split(',');
        if (nums.Length > 0) s.OvFeed = (int)ParseD(nums[0]);
        if (nums.Length > 1) s.OvRapid = (int)ParseD(nums[1]);
        if (nums.Length > 2) s.OvSpindle = (int)ParseD(nums[2]);
    }

    private static double ParseD(string s) =>
        double.TryParse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : 0;
}
