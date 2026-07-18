using System.Windows;
using GrblPlotter.Wpf.ViewModels;

namespace GrblPlotter.Wpf.Views;

public partial class TextCreateWindow : Window
{
    public event Action<string>? GCodeGenerated;

    public TextCreateWindow()
    {
        InitializeComponent();
        var vm = new TextCreateViewModel();
        vm.GCodeGenerated += g => GCodeGenerated?.Invoke(g);
        DataContext = vm;
    }

    public TextCreateWindow(Action<string> onGenerated) : this()
    {
        GCodeGenerated += onGenerated;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
