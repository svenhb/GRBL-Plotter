using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GrblPlotter.Wpf.Views;

public class WireCutterWindow : Window
{
    public WireCutterWindow(Action<string>? onGenerated = null)
    {
        Title = "Wire Cutter / Hot-wire";
        Width = 420; Height = 360;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        try { Background = (System.Windows.Media.Brush)FindResource("BgAppBrush"); } catch { }

        var panel = new StackPanel { Margin = new Thickness(12) };
        panel.Children.Add(new TextBlock { Text = "Hot-wire / foam cut path", FontWeight = FontWeights.SemiBold, FontSize = 14 });

        TextBox W(string d) { var t = new TextBox { Text = d, Margin = new Thickness(0, 2, 0, 6) }; return t; }
        var length = W("100");
        var height = W("40");
        var taper = W("10");
        var feed = W("200");
        var zClear = W("5");

        void L(string s) => panel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(0, 4, 0, 0) });
        L("Length X (mm)"); panel.Children.Add(length);
        L("Height Y (mm)"); panel.Children.Add(height);
        L("Taper (mm each side)"); panel.Children.Add(taper);
        L("Feed"); panel.Children.Add(feed);
        L("Z clear"); panel.Children.Add(zClear);

        var btn = new Button { Content = "Generate path", Margin = new Thickness(0, 10, 0, 0), Height = 32 };
        btn.Click += (_, _) =>
        {
            double.TryParse(length.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var lx);
            double.TryParse(height.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var hy);
            double.TryParse(taper.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var tp);
            double.TryParse(feed.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var f);
            double.TryParse(zClear.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var zc);
            if (lx <= 0) lx = 100; if (hy <= 0) hy = 40; if (f <= 0) f = 200;
            var sb = new StringBuilder();
            sb.AppendLine("; Wire cutter profile");
            sb.AppendLine("G21 G90 G94");
            sb.AppendLine(FormattableString.Invariant($"G0 Z{zc:0.###}"));
            sb.AppendLine("G0 X0 Y0");
            sb.AppendLine("G1 Z0 F100");
            sb.AppendLine(FormattableString.Invariant($"G1 X{tp:0.###} Y{hy:0.###} F{f:0.###}"));
            sb.AppendLine(FormattableString.Invariant($"G1 X{lx - tp:0.###} Y{hy:0.###}"));
            sb.AppendLine(FormattableString.Invariant($"G1 X{lx:0.###} Y0"));
            sb.AppendLine(FormattableString.Invariant($"G0 Z{zc:0.###}"));
            sb.AppendLine("G0 X0 Y0");
            sb.AppendLine("M2");
            onGenerated?.Invoke(sb.ToString());
            Close();
        };
        panel.Children.Add(btn);
        Content = panel;
    }
}
