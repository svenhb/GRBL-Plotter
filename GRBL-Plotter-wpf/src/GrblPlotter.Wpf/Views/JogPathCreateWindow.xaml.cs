using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GrblPlotter.Wpf.Views;

/// <summary>Simple jog-path / framing rectangle generator (WinForms JogPathCreator subset).</summary>
public class JogPathCreateWindow : Window
{
    public JogPathCreateWindow(Action<string>? onGenerated = null)
    {
        Title = "Create Jog Path / Frame";
        Width = 400;
        Height = 340;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        try { Background = (System.Windows.Media.Brush)FindResource("BgAppBrush"); } catch { }

        var panel = new StackPanel { Margin = new Thickness(12) };
        panel.Children.Add(new TextBlock { Text = "Rectangular framing path", FontWeight = FontWeights.SemiBold, FontSize = 14 });

        TextBox W(string d) => new() { Text = d, Margin = new Thickness(0, 2, 0, 6) };
        var x0 = W("0"); var y0 = W("0");
        var w = W("100"); var h = W("80");
        var feed = W("1000"); var zClear = W("5");
        var passes = W("1");

        void L(string s) => panel.Children.Add(new TextBlock { Text = s, Margin = new Thickness(0, 4, 0, 0) });
        L("Origin X"); panel.Children.Add(x0);
        L("Origin Y"); panel.Children.Add(y0);
        L("Width"); panel.Children.Add(w);
        L("Height"); panel.Children.Add(h);
        L("Feed"); panel.Children.Add(feed);
        L("Z clear"); panel.Children.Add(zClear);
        L("Passes"); panel.Children.Add(passes);

        var btn = new Button { Content = "Generate & Apply", Height = 32, Margin = new Thickness(0, 10, 0, 0) };
        btn.Click += (_, _) =>
        {
            double.TryParse(x0.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var ox);
            double.TryParse(y0.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var oy);
            double.TryParse(w.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var ww);
            double.TryParse(h.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var hh);
            double.TryParse(feed.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var f);
            double.TryParse(zClear.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var zc);
            int.TryParse(passes.Text, out var n);
            if (ww <= 0) ww = 100; if (hh <= 0) hh = 80; if (f <= 0) f = 1000; if (n < 1) n = 1;

            var sb = new StringBuilder();
            sb.AppendLine("; Jog / frame path");
            sb.AppendLine("G21 G90 G94");
            sb.AppendLine(FormattableString.Invariant($"G0 Z{zc:0.###}"));
            for (int i = 0; i < n; i++)
            {
                sb.AppendLine(FormattableString.Invariant($"G0 X{ox:0.###} Y{oy:0.###}"));
                sb.AppendLine(FormattableString.Invariant($"G1 F{f:0.###}"));
                sb.AppendLine(FormattableString.Invariant($"G1 X{ox + ww:0.###} Y{oy:0.###}"));
                sb.AppendLine(FormattableString.Invariant($"G1 X{ox + ww:0.###} Y{oy + hh:0.###}"));
                sb.AppendLine(FormattableString.Invariant($"G1 X{ox:0.###} Y{oy + hh:0.###}"));
                sb.AppendLine(FormattableString.Invariant($"G1 X{ox:0.###} Y{oy:0.###}"));
            }
            sb.AppendLine(FormattableString.Invariant($"G0 Z{zc:0.###}"));
            sb.AppendLine("M2");
            onGenerated?.Invoke(sb.ToString());
            Close();
        };
        panel.Children.Add(btn);
        Content = new ScrollViewer { Content = panel, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
    }
}
