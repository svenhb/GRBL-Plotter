using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GrblPlotter.Wpf.Services;

namespace GrblPlotter.Wpf.Views;

/// <summary>Laser material test pattern generator (WinForms ControlLaser subset).</summary>
public class LaserToolsWindow : Window
{
    public LaserToolsWindow(GrblSerialService serial, Action<string>? onGenerated = null)
    {
        Title = "Laser Tools";
        Width = 420;
        Height = 380;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        try { Background = (System.Windows.Media.Brush)FindResource("BgAppBrush"); } catch { }

        var panel = new StackPanel { Margin = new Thickness(12) };
        panel.Children.Add(new TextBlock { Text = "Material power / speed test", FontWeight = FontWeights.SemiBold, FontSize = 14 });

        TextBox W(string d) => new() { Text = d, Margin = new Thickness(0, 2, 0, 6) };
        var sMin = W("100"); var sMax = W("1000");
        var fMin = W("500"); var fMax = W("3000");
        var steps = W("5"); var cell = W("10"); var gap = W("2");

        void L(string s) => panel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(0, 4, 0, 0) });
        L("Power S min"); panel.Children.Add(sMin);
        L("Power S max"); panel.Children.Add(sMax);
        L("Feed F min"); panel.Children.Add(fMin);
        L("Feed F max"); panel.Children.Add(fMax);
        L("Steps (grid size)"); panel.Children.Add(steps);
        L("Cell size mm"); panel.Children.Add(cell);
        L("Gap mm"); panel.Children.Add(gap);

        var gen = new Button { Content = "Generate test pattern", Height = 30, Margin = new Thickness(0, 8, 0, 0) };
        var pilot = new Button { Content = "Pilot laser M3 S1", Height = 28, Margin = new Thickness(0, 6, 0, 0) };
        var off = new Button { Content = "Laser OFF M5", Height = 28, Margin = new Thickness(0, 4, 0, 0) };

        gen.Click += (_, _) =>
        {
            double.TryParse(sMin.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var s0);
            double.TryParse(sMax.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var s1);
            double.TryParse(fMin.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var f0);
            double.TryParse(fMax.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var f1);
            int.TryParse(steps.Text, out var n);
            double.TryParse(cell.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var c);
            double.TryParse(gap.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var g);
            if (n < 2) n = 5; if (c <= 0) c = 10;

            var sb = new StringBuilder();
            sb.AppendLine("; Laser material test grid");
            sb.AppendLine("G21 G90 G94");
            sb.AppendLine("M4");
            for (int iy = 0; iy < n; iy++)
            for (int ix = 0; ix < n; ix++)
            {
                double s = s0 + (s1 - s0) * ix / Math.Max(n - 1, 1);
                double f = f0 + (f1 - f0) * iy / Math.Max(n - 1, 1);
                double x = ix * (c + g);
                double y = iy * (c + g);
                sb.AppendLine(FormattableString.Invariant($"G0 X{x:0.###} Y{y:0.###}"));
                sb.AppendLine(FormattableString.Invariant($"G1 X{x + c:0.###} S{s:0} F{f:0}"));
                sb.AppendLine(FormattableString.Invariant($"G1 Y{y + c:0.###}"));
                sb.AppendLine(FormattableString.Invariant($"G1 X{x:0.###}"));
                sb.AppendLine(FormattableString.Invariant($"G1 Y{y:0.###}"));
            }
            sb.AppendLine("M5");
            sb.AppendLine("G0 X0 Y0");
            sb.AppendLine("M2");
            onGenerated?.Invoke(sb.ToString());
            Close();
        };
        pilot.Click += (_, _) => serial.SendLine("M3 S1");
        off.Click += (_, _) => serial.SendLine("M5");

        panel.Children.Add(gen);
        panel.Children.Add(pilot);
        panel.Children.Add(off);
        Content = new ScrollViewer { Content = panel };
    }
}
