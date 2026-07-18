using System.Windows;
using GrblPlotter.Wpf.ViewModels;

namespace GrblPlotter.Wpf.Views;

public partial class ImageCreateWindow : Window
{
    public event Action<string>? GCodeGenerated;

    public ImageCreateWindow()
    {
        InitializeComponent();
        var vm = new ImageCreateViewModel();
        vm.GCodeGenerated += g => GCodeGenerated?.Invoke(g);
        DataContext = vm;
    }

    public ImageCreateWindow(Action<string> onGenerated) : this()
    {
        GCodeGenerated += onGenerated;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
