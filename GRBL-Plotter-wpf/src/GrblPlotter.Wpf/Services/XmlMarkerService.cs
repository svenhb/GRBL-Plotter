using System.Text;
using System.Text.RegularExpressions;

namespace GrblPlotter.Wpf.Services;

/// <summary>XML figure/block markers in G-code comments — WinForms XmlMarker subset for sort/fold.</summary>
public static class XmlMarkerService
{
    private static readonly Regex OpenRx = new(
        @"<\s*(Figure|Group|Layer|Path)\s+([^>]*)>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex AttrRx = new(
        @"(\w+)\s*=\s*""([^""]*)""",
        RegexOptions.Compiled);

    public sealed class Block
    {
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public string Kind { get; set; } = "";
        public string Id { get; set; } = "";
        public string Color { get; set; } = "";
        public string Layer { get; set; } = "";
        public string Type { get; set; } = "";
        public string Geometry { get; set; } = "";
        public string ToolNr { get; set; } = "";
        public string ToolName { get; set; } = "";
        public double CodeSize { get; set; }
        public double CodeArea { get; set; }
        public List<string> Lines { get; } = new();
    }

    public static List<Block> ParseBlocks(IReadOnlyList<string> lines)
    {
        var blocks = new List<Block>();
        Block? cur = null;
        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var m = OpenRx.Match(line);
            if (m.Success)
            {
                cur = new Block { StartLine = i, Kind = m.Groups[1].Value };
                foreach (Match a in AttrRx.Matches(m.Groups[2].Value))
                {
                    var k = a.Groups[1].Value.ToLowerInvariant();
                    var v = a.Groups[2].Value;
                    switch (k)
                    {
                        case "id": cur.Id = v; break;
                        case "penColor":
                        case "color": cur.Color = v; break;
                        case "layer": cur.Layer = v; break;
                        case "type": cur.Type = v; break;
                        case "geometry": cur.Geometry = v; break;
                        case "toolnr": cur.ToolNr = v; break;
                        case "toolname": cur.ToolName = v; break;
                        case "codesize": double.TryParse(v, out var cs); cur.CodeSize = cs; break;
                        case "codearea": double.TryParse(v, out var ca); cur.CodeArea = ca; break;
                    }
                }
                cur.Lines.Add(line);
                continue;
            }
            if (cur != null)
            {
                cur.Lines.Add(line);
                if (line.Contains("</", StringComparison.OrdinalIgnoreCase) &&
                    line.Contains(cur.Kind, StringComparison.OrdinalIgnoreCase))
                {
                    cur.EndLine = i;
                    blocks.Add(cur);
                    cur = null;
                }
            }
        }
        return blocks;
    }

    public static string SortBy(IReadOnlyList<string> lines, string key, bool reverse = false)
    {
        var blocks = ParseBlocks(lines);
        if (blocks.Count == 0) return string.Join(Environment.NewLine, lines);

        IOrderedEnumerable<Block> ordered = key.ToLowerInvariant() switch
        {
            "id" => blocks.OrderBy(b => b.Id),
            "color" => blocks.OrderBy(b => b.Color),
            "layer" => blocks.OrderBy(b => b.Layer),
            "type" => blocks.OrderBy(b => b.Type),
            "geometry" => blocks.OrderBy(b => b.Geometry),
            "tool" or "toolnr" => blocks.OrderBy(b => b.ToolNr),
            "toolname" => blocks.OrderBy(b => b.ToolName),
            "size" or "codesize" => blocks.OrderBy(b => b.CodeSize),
            "area" or "codearea" => blocks.OrderBy(b => b.CodeArea),
            "distance" => blocks.OrderBy(b => b.StartLine),
            _ => blocks.OrderBy(b => b.Id)
        };
        var list = reverse ? ordered.Reverse().ToList() : ordered.ToList();

        // Keep preamble before first block
        int first = blocks.Min(b => b.StartLine);
        var sb = new StringBuilder();
        for (int i = 0; i < first; i++) sb.AppendLine(lines[i]);
        foreach (var b in list)
            foreach (var l in b.Lines) sb.AppendLine(l);
        // Trailing after last block
        int last = blocks.Max(b => b.EndLine);
        for (int i = last + 1; i < lines.Count; i++) sb.AppendLine(lines[i]);
        return sb.ToString().TrimEnd() + Environment.NewLine;
    }

    public static string RemoveXmlTags(IReadOnlyList<string> lines, bool groupsOnly)
    {
        var sb = new StringBuilder();
        foreach (var line in lines)
        {
            var t = line.Trim();
            bool isTag = t.StartsWith('<') && t.Contains('>');
            if (!isTag) { sb.AppendLine(line); continue; }
            if (groupsOnly && !t.Contains("Group", StringComparison.OrdinalIgnoreCase))
                sb.AppendLine(line);
            // else drop tag
        }
        return sb.ToString().TrimEnd() + Environment.NewLine;
    }

    public static IEnumerable<(int Start, int End)> FoldRanges(IReadOnlyList<string> lines, int level)
    {
        // level 1 = Figure, 2 = Group, 3 = Layer
        string kind = level switch { 2 => "Group", 3 => "Layer", _ => "Figure" };
        foreach (var b in ParseBlocks(lines).Where(b => b.Kind.Equals(kind, StringComparison.OrdinalIgnoreCase)))
            yield return (b.StartLine, b.EndLine);
    }
}
