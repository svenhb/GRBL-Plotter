using System.Windows;
using GrblPlotter.Wpf.Services;
using GrblPlotter.Wpf.ViewModels;

namespace GrblPlotter.Wpf.Views;

public partial class ProbingWindow : Window
{
    public ProbingWindow(GrblSerialService serial)
    {
        InitializeComponent();
        DataContext = new ProbingViewModel(serial);
    }

    public ProbingWindow(Action<string> sendLine)
    {
        InitializeComponent();
        DataContext = new ProbingViewModel(sendLine);
    }
}
