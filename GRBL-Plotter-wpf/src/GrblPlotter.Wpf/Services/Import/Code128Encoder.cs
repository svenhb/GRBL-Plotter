using System.Text;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>Minimal Code128B encoder → bar pattern (true symbology for barcode window).</summary>
public static class Code128Encoder
{
    // Patterns for Code Set B (value 0..106) — each is 6 widths alternating bar/space starting with bar
    private static readonly string[] Patterns =
    {
        "212222","222122","222221","121223","121322","131222","122213","122312","132212","221213",
        "221312","231212","112232","122132","122231","113222","123122","123221","223211","221132",
        "221231","213212","223112","312131","311222","321122","321221","312212","322112","322211",
        "212123","212321","232121","111323","131123","131321","112313","132113","132311","211313",
        "231113","231311","213113","213311","213131","311123","311321","331121","312113","312311",
        "332111","314111","221411","431111","111224","111422","121124","121421","141122","141221",
        "112214","112412","122114","122411","142112","142211","241211","221114","413111","241112",
        "134111","111242","121142","121241","114212","124112","124211","411212","421112","421211",
        "212141","214121","412121","111143","111341","131141","114113","114311","411113","411311",
        "113141","114131","311141","411131","211412","211214","211232","2331112"
    };

    public static bool[] EncodeBars(string content)
    {
        // Code128B: StartB=104, data, checksum, Stop=106
        const int startB = 104;
        var values = new List<int> { startB };
        foreach (var ch in content)
        {
            int v = ch;
            if (v is < 32 or > 126) v = '?';
            values.Add(v - 32);
        }
        int sum = startB;
        for (int i = 1; i < values.Count; i++) sum += values[i] * i;
        values.Add(sum % 103);
        values.Add(106); // stop

        var bits = new List<bool>();
        foreach (var v in values)
        {
            var pat = Patterns[Math.Clamp(v, 0, Patterns.Length - 1)];
            bool bar = true;
            foreach (var c in pat)
            {
                int w = c - '0';
                for (int i = 0; i < w; i++) bits.Add(bar);
                bar = !bar;
            }
        }
        // stop pattern already includes terminator in last pattern entry
        return bits.ToArray();
    }

    public static string ToGCode(string content, double widthMm, double heightMm, double minPitch)
    {
        var bits = EncodeBars(content);
        int n = bits.Length;
        double unit = Math.Max(minPitch, widthMm / n);
        var sb = new StringBuilder();
        sb.AppendLine("; Code128B barcode");
        sb.AppendLine("G21 G90 G94");
        sb.AppendLine("G0 Z2");
        double x = 0;
        for (int i = 0; i < n; i++)
        {
            if (bits[i])
            {
                sb.AppendLine(FormattableString.Invariant($"G0 X{x:0.###} Y0"));
                sb.AppendLine("G1 Z-0.15 F400");
                sb.AppendLine(FormattableString.Invariant($"G1 Y{heightMm:0.###} F900"));
                sb.AppendLine("G0 Z2");
                sb.AppendLine("G0 Y0");
            }
            x += unit;
        }
        sb.AppendLine("G0 X0 Y0");
        sb.AppendLine("M2");
        return sb.ToString();
    }
}
