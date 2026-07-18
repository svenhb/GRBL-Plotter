using System.Globalization;
using System.IO;
using System.Text;

namespace GrblPlotter.Wpf.Services;

public sealed class ToolEntry
{
    public int Number { get; set; }
    public string Name { get; set; } = "";
    public string Color { get; set; } = "#FFFFFF";
    public double Diameter { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}

public static class ToolListService
{
    public static List<ToolEntry> LoadCsv(string path)
    {
        var list = new List<ToolEntry>();
        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#') || line.StartsWith(';')) continue;
            if (line.StartsWith("nr", StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith("number", StringComparison.OrdinalIgnoreCase)) continue;
            var p = line.Split(',', ';', '\t');
            if (p.Length < 2) continue;
            int.TryParse(p[0].Trim(), out var nr);
            var name = p.Length > 1 ? p[1].Trim() : $"T{nr}";
            var color = p.Length > 2 ? p[2].Trim() : "#FFFFFF";
            double.TryParse(p.ElementAtOrDefault(3) ?? "0", NumberStyles.Float, CultureInfo.InvariantCulture, out var dia);
            double.TryParse(p.ElementAtOrDefault(4) ?? "0", NumberStyles.Float, CultureInfo.InvariantCulture, out var x);
            double.TryParse(p.ElementAtOrDefault(5) ?? "0", NumberStyles.Float, CultureInfo.InvariantCulture, out var y);
            list.Add(new ToolEntry { Number = nr, Name = name, Color = color, Diameter = dia, X = x, Y = y });
        }
        return list;
    }

    public static void SaveCsv(string path, IEnumerable<ToolEntry> tools)
    {
        var sb = new StringBuilder();
        sb.AppendLine("nr,name,color,diameter,x,y");
        foreach (var t in tools)
            sb.AppendLine(string.Create(CultureInfo.InvariantCulture,
                $"{t.Number},{Escape(t.Name)},{t.Color},{t.Diameter:0.###},{t.X:0.###},{t.Y:0.###}"));
        File.WriteAllText(path, sb.ToString());
    }

    public static List<ToolEntry> GroupByColor(IEnumerable<ToolEntry> tools) =>
        tools.OrderBy(t => t.Color).ThenBy(t => t.Number).ToList();

    private static string Escape(string s) => s.Contains(',') ? $"\"{s}\"" : s;
}
