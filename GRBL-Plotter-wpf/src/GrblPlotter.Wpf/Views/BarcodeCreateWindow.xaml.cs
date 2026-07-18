using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GrblPlotter.Wpf.Views;

/// <summary>Barcode / QR-style module pattern â†?G-Code (no extra NuGet).</summary>
public class BarcodeCreateWindow : Window
{
    private readonly Action<string>? _onGenerated;

    public BarcodeCreateWindow(Action<string>? onGenerated = null)
    {
        _onGenerated = onGenerated;
        Title = "Create Barcode";
        Width = 440;
        Height = 360;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        try
        {
            Background = (System.Windows.Media.Brush)FindResource("BgAppBrush");
            Foreground = (System.Windows.Media.Brush)FindResource("TextBrush");
        }
        catch { /* design-time */ }

        var root = new DockPanel { Margin = new Thickness(12) };
        var gen = new Button { Content = "Generate & Apply", Margin = new Thickness(0, 10, 0, 0), Height = 32 };
        DockPanel.SetDock(gen, Dock.Bottom);

        var panel = new StackPanel();
        panel.Children.Add(new TextBlock { Text = "Barcode â†?G-Code", FontWeight = FontWeights.SemiBold, FontSize = 14, Margin = new Thickness(0, 0, 0, 8) });

        var type = new ComboBox { Margin = new Thickness(0, 2, 0, 6) };
        type.Items.Add("1D Code (bars)");
        type.Items.Add("2D Matrix (modules)");
        type.SelectedIndex = 0;

        var text = new TextBox { Text = "GRBL-PLOTTER", Margin = new Thickness(0, 2, 0, 6) };
        var width = new TextBox { Text = "60", Margin = new Thickness(0, 2, 0, 6) };
        var height = new TextBox { Text = "20", Margin = new Thickness(0, 2, 0, 6) };
        var pitch = new TextBox { Text = "0.3", Margin = new Thickness(0, 2, 0, 6) };

        void L(string t) => panel.Children.Add(new TextBlock { Text = t, Margin = new Thickness(0, 4, 0, 0) });
        L("Type"); panel.Children.Add(type);
        L("Content"); panel.Children.Add(text);
        L("Width (mm)"); panel.Children.Add(width);
        L("Height (mm)"); panel.Children.Add(height);
        L("Min pitch (mm)"); panel.Children.Add(pitch);

        gen.Click += (_, _) =>
        {
            double.TryParse(width.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var w);
            double.TryParse(height.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var h);
            double.TryParse(pitch.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var p);
            if (w <= 0) w = 60; if (h <= 0) h = 20; if (p <= 0) p = 0.3;
            var content = text.Text ?? "GRBL";
            var gcode = type.SelectedIndex == 0
                ? Services.Import.Code128Encoder.ToGCode(content, w, h, p)
                : BuildMatrix(content, w, p);
            _onGenerated?.Invoke(gcode);
            Close();
        };

        root.Children.Add(gen);
        root.Children.Add(new ScrollViewer { Content = panel, VerticalScrollBarVisibility = ScrollBarVisibility.Auto });
        Content = root;
    }

    private static string Build1D(string content, double widthMm, double heightMm, double pitch)
    {
        // Map each char to a 4-bit bar pattern
        var bits = new List<bool>();
        bits.AddRange(new[] { true, true, false }); // start
        foreach (var ch in content.ToUpperInvariant())
        {
            int v = ch % 16;
            for (int i = 3; i >= 0; i--) bits.Add(((v >> i) & 1) == 1);
            bits.Add(false); // gap
        }
        bits.AddRange(new[] { true, true, true }); // stop

        int n = bits.Count;
        double unit = Math.Max(pitch, widthMm / n);
        var sb = new StringBuilder();
        sb.AppendLine("; 1D barcode");
        sb.AppendLine("G21 G90 G94");
        sb.AppendLine("G0 Z2");
        double x = 0;
        for (int i = 0; i < n; i++)
        {
            if (bits[i])
            {
                sb.AppendLine(Inv($"G0 X{x:0.###} Y0"));
                sb.AppendLine("G1 Z-0.15 F400");
                sb.AppendLine(Inv($"G1 Y{heightMm:0.###} F900"));
                sb.AppendLine("G0 Z2");
                sb.AppendLine(Inv($"G0 Y0"));
            }
            x += unit;
        }
        sb.AppendLine("G0 X0 Y0");
        sb.AppendLine("M2");
        return sb.ToString();
    }

    private static string BuildMatrix(string content, double sizeMm, double pitch)
    {
        // Deterministic pseudo-matrix from content hash (QR-like fill for engraving)
        int n = Math.Max(16, Math.Min(48, (int)(sizeMm / Math.Max(pitch, 0.2))));
        var cells = new bool[n, n];
        var seed = content.GetHashCode();
        var rng = new Random(seed);
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                // finder-ish corners
                bool finder = (x < 7 && y < 7) || (x >= n - 7 && y < 7) || (x < 7 && y >= n - 7);
                if (finder)
                    cells[y, x] = (x == 0 || y == 0 || x == 6 || y == 6 || x == n - 1 || y == n - 1 || x == n - 7 || y == n - 7)
                                  || (x >= 2 && x <= 4 && y >= 2 && y <= 4)
                                  || (x >= n - 5 && x <= n - 3 && y >= 2 && y <= 4)
                                  || (x >= 2 && x <= 4 && y >= n - 5 && y <= n - 3);
                else
                    cells[y, x] = ((content[(x + y) % content.Length] + x * 3 + y * 5 + rng.Next(3)) % 3) == 0;
            }

        double scale = sizeMm / n;
        var sb = new StringBuilder();
        sb.AppendLine("; 2D matrix barcode");
        sb.AppendLine("G21 G90 G94");
        sb.AppendLine("G0 Z2");
        for (int y = 0; y < n; y++)
        {
            bool down = false;
            double rowY = (n - 1 - y) * scale;
            for (int x = 0; x <= n; x++)
            {
                bool dark = x < n && cells[y, x];
                if (dark && !down)
                {
                    sb.AppendLine(Inv($"G0 X{x * scale:0.###} Y{rowY:0.###}"));
                    sb.AppendLine("G1 Z-0.15 F400");
                    down = true;
                }
                else if (!dark && down)
                {
                    sb.AppendLine(Inv($"G1 X{x * scale:0.###} Y{rowY:0.###} F1000"));
                    sb.AppendLine("G0 Z2");
                    down = false;
                }
            }
        }
        sb.AppendLine("G0 X0 Y0");
        sb.AppendLine("M2");
        return sb.ToString();
    }

    private static string Inv(FormattableString fs) =>
        string.Format(CultureInfo.InvariantCulture, fs.Format, fs.GetArguments());
}
