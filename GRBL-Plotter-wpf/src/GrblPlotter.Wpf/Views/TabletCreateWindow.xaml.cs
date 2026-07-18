using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace GrblPlotter.Wpf.Views;

/// <summary>Graphic tablet / mouse sketch → polyline G-Code (WinForms tablet subset).</summary>
public class TabletCreateWindow : Window
{
    private readonly InkCanvas _ink;
    private readonly TextBox _scale;
    private readonly TextBox _feed;

    public TabletCreateWindow(Action<string>? onGenerated = null)
    {
        Title = "Graphic Tablet / Sketch";
        Width = 720; Height = 560;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        try { Background = (Brush)FindResource("BgAppBrush"); } catch { }

        var dock = new DockPanel { Margin = new Thickness(8) };
        var top = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
        top.Children.Add(new TextBlock { Text = "Scale mm/px", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 6, 0) });
        _scale = new TextBox { Text = "0.25", Width = 60, Margin = new Thickness(0, 0, 12, 0) };
        top.Children.Add(_scale);
        top.Children.Add(new TextBlock { Text = "Feed", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 6, 0) });
        _feed = new TextBox { Text = "1000", Width = 70, Margin = new Thickness(0, 0, 12, 0) };
        top.Children.Add(_feed);
        var clear = new Button { Content = "Clear", MinWidth = 64, Margin = new Thickness(0, 0, 6, 0) };
        var gen = new Button { Content = "Generate G-Code", MinWidth = 120 };
        top.Children.Add(clear);
        top.Children.Add(gen);
        DockPanel.SetDock(top, Dock.Top);

        _ink = new InkCanvas
        {
            Background = new SolidColorBrush(Color.FromRgb(0x1A, 0x1F, 0x26)),
            DefaultDrawingAttributes = new DrawingAttributes
            {
                Color = Color.FromRgb(0x6F, 0xBF, 0x8A),
                Width = 2, Height = 2, FitToCurve = false
            }
        };
        clear.Click += (_, _) => _ink.Strokes.Clear();
        gen.Click += (_, _) =>
        {
            double.TryParse(_scale.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var s);
            double.TryParse(_feed.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var f);
            if (s <= 0) s = 0.25; if (f <= 0) f = 1000;
            var g = StrokesToGCode(_ink.Strokes, s, f);
            onGenerated?.Invoke(g);
            Close();
        };

        dock.Children.Add(top);
        dock.Children.Add(new Border { Child = _ink, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) });
        Content = dock;
    }

    private static string StrokesToGCode(StrokeCollection strokes, double scale, double feed)
    {
        var sb = new StringBuilder();
        sb.AppendLine("; tablet sketch");
        sb.AppendLine("G21 G90 G94");
        sb.AppendLine("G0 Z2");
        double maxY = 0;
        foreach (Stroke st in strokes)
            foreach (var p in st.StylusPoints)
                maxY = Math.Max(maxY, p.Y);

        foreach (Stroke st in strokes)
        {
            bool first = true;
            foreach (var p in st.StylusPoints)
            {
                double x = p.X * scale;
                double y = (maxY - p.Y) * scale; // flip Y
                if (first)
                {
                    sb.AppendLine(FormattableString.Invariant($"G0 X{x:0.###} Y{y:0.###}"));
                    sb.AppendLine("G1 Z-0.2 F400");
                    first = false;
                }
                else
                    sb.AppendLine(FormattableString.Invariant($"G1 X{x:0.###} Y{y:0.###} F{feed:0}"));
            }
            sb.AppendLine("G0 Z2");
        }
        sb.AppendLine("G0 X0 Y0");
        sb.AppendLine("M2");
        return sb.ToString();
    }
}
