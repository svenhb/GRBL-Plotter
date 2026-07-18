using System.Windows;
using GrblPlotter.Wpf.Services;
using GrblPlotter.Wpf.ViewModels;

namespace GrblPlotter.Wpf.Views;

public partial class AutomationWindow : Window
{
    public AutomationWindow(GrblSerialService serial, Action<string>? loadFile = null, Action<string>? showMessage = null)
    {
        InitializeComponent();
        DataContext = new AutomationViewModel(serial, loadFile, showMessage);
    }
}
