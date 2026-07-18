using System.Windows;
using GrblPlotter.Wpf.Services;
using GrblPlotter.Wpf.ViewModels;

namespace GrblPlotter.Wpf.Views;

public partial class CoordSystemWindow : Window
{
    public CoordSystemWindow(GrblSerialService serial)
    {
        InitializeComponent();
        DataContext = new CoordSystemViewModel(serial);
    }

    public CoordSystemWindow(Action<string> sendLine)
    {
        InitializeComponent();
        DataContext = new CoordSystemViewModel(sendLine);
    }
}
