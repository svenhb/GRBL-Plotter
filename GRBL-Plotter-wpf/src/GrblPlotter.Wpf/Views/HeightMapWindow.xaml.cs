using System.Windows;
using GrblPlotter.Wpf.Models;
using GrblPlotter.Wpf.Services;
using GrblPlotter.Wpf.ViewModels;

namespace GrblPlotter.Wpf.Views;

public partial class HeightMapWindow : Window
{
    public HeightMapWindow(GrblSerialService serial, Func<GCodeDocument> getDoc, Action<GCodeDocument> setDoc)
    {
        InitializeComponent();
        DataContext = new HeightMapViewModel(serial, getDoc, setDoc);
    }

    public HeightMapWindow(Action<string> sendLine, Func<GCodeDocument>? getDoc = null, Action<GCodeDocument>? setDoc = null)
    {
        InitializeComponent();
        DataContext = new HeightMapViewModel(sendLine, getDoc, setDoc);
    }
}
