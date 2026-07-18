using System.Windows;
using GrblPlotter.Wpf.Models;
using GrblPlotter.Wpf.ViewModels;

namespace GrblPlotter.Wpf.Views;

public partial class CameraWindow : Window
{
    public CameraWindow() : this(_ => { })
    {
    }

    public CameraWindow(Action<string> sendLine, Func<AxisPosition>? getWorkPos = null)
    {
        InitializeComponent();
        DataContext = new CameraViewModel(sendLine, getWorkPos);
        Closed += (_, _) =>
        {
            if (DataContext is CameraViewModel vm) vm.Dispose();
        };
    }
}
