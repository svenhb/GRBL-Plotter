using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace GrblPlotter.Wpf.Views;

/// <summary>Projector overlay showing live toolpath geometry from the main document.</summary>
public class ProjectorWindow : Window
{
    private readonly Path _tool = new() { Stroke = Brushes.Lime, StrokeThickness = 1.5 };
    private readonly Path _rapid = new() { Stroke = Brushes.DodgerBlue, StrokeThickness = 1, StrokeDashArray = new DoubleCollection { 2, 2 } };
    private readonly Viewbox _view;

    public ProjectorWindow(Geometry? toolpath = null, Geometry? rapid = null)
    {
        Title = "Projector Overlay";
        Width = 900; Height = 700;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Background = Brushes.Black;
        WindowStyle = WindowStyle.None;
        ResizeMode = ResizeMode.CanResizeWithGrip;

        var canvas = new Canvas { Width = 800, Height = 600, Background = Brushes.Black };
        if (toolpath != null) _tool.Data = toolpath;
        if (rapid != null) _rapid.Data = rapid;
        canvas.Children.Add(_rapid);
        canvas.Children.Add(_tool);

        _view = new Viewbox { Child = canvas, Stretch = Stretch.Uniform };
        var root = new Grid();
        root.Children.Add(_view);
        root.Children.Add(new TextBlock
        {
            Text = "Projector — Esc close · F11 fullscreen · shows current toolpath",
            Foreground = Brushes.White,
            Margin = new Thickness(12),
            VerticalAlignment = VerticalAlignment.Bottom,
            Opacity = 0.7
        });
        Content = root;

        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape) Close();
            if (e.Key == Key.F11)
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        };
    }

    public void UpdateGeometry(Geometry? toolpath, Geometry? rapid)
    {
        _tool.Data = toolpath;
        _rapid.Data = rapid;
    }
}
