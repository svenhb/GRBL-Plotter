using System.Windows;
using GrblPlotter.Wpf.ViewModels;

namespace GrblPlotter.Wpf.Views;

public partial class ShapeCreateWindow : Window
{
    public event Action<string>? GCodeGenerated;

    public ShapeCreateWindow()
    {
        InitializeComponent();
        var vm = new ShapeCreateViewModel();
        vm.GCodeGenerated += g => GCodeGenerated?.Invoke(g);
        DataContext = vm;
    }

    public ShapeCreateWindow(Action<string> onGenerated) : this()
    {
        GCodeGenerated += onGenerated;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
