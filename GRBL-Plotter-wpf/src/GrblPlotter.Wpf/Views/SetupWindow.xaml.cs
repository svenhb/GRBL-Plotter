using System.Windows;
using GrblPlotter.Wpf.Services;
using GrblPlotter.Wpf.ViewModels;

namespace GrblPlotter.Wpf.Views;

public partial class SetupWindow : Window
{
    public SetupWindow() : this(AppSettings.Load())
    {
    }

    public SetupWindow(AppSettings settings)
    {
        InitializeComponent();
        DataContext = new SetupViewModel(settings);
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
