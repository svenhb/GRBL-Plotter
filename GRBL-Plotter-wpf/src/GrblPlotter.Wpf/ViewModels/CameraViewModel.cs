using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GrblPlotter.Wpf.Models;
using GrblPlotter.Wpf.Services;
using Microsoft.Win32;

namespace GrblPlotter.Wpf.ViewModels;

public partial class CameraViewModel : ObservableObject, IDisposable
{
    private readonly Action<string> _sendLine;
    private readonly Func<AxisPosition>? _getWorkPos;
    private readonly WebcamService _webcam = new();

    [ObservableProperty] private BitmapSource? _stillImage;
    [ObservableProperty] private bool _isRunning;
    [ObservableProperty] private double _offsetX;
    [ObservableProperty] private double _offsetY;
    [ObservableProperty] private double _teachX;
    [ObservableProperty] private double _teachY;
    [ObservableProperty] private string _statusText = "camera stopped";
    [ObservableProperty] private string? _imagePath;
    [ObservableProperty] private int _selectedDeviceIndex;

    public ObservableCollection<string> Devices { get; } = new();

    public CameraViewModel(Action<string> sendLine, Func<AxisPosition>? getWorkPos = null)
    {
        _sendLine = sendLine;
        _getWorkPos = getWorkPos;
        _webcam.FrameReady += bmp =>
        {
            StillImage = bmp;
        };
        RefreshDevices();
    }

    [RelayCommand]
    private void RefreshDevices()
    {
        Devices.Clear();
        foreach (var d in WebcamService.ListDevices()) Devices.Add(d);
        StatusText = Devices.Count == 0 ? "no cameras found — use still image" : $"{Devices.Count} camera(s)";
    }

    [RelayCommand]
    private async Task ToggleCamera()
    {
        try
        {
            if (IsRunning)
            {
                await _webcam.StopAsync();
                IsRunning = false;
                StatusText = "camera stopped";
                return;
            }
            await _webcam.StartAsync(SelectedDeviceIndex);
            IsRunning = true;
            StatusText = "live camera running";
        }
        catch (Exception ex)
        {
            IsRunning = false;
            StatusText = "camera error: " + ex.Message;
        }
    }

    [RelayCommand]
    private void LoadStillImage()
    {
        var dlg = new OpenFileDialog { Filter = "Images (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp" };
        if (dlg.ShowDialog() != true) return;
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.UriSource = new Uri(dlg.FileName);
        bmp.EndInit();
        StillImage = bmp;
        ImagePath = dlg.FileName;
        StatusText = $"loaded still frame {System.IO.Path.GetFileName(dlg.FileName)}";
    }

    [RelayCommand]
    private void TeachOffset()
    {
        var pos = _getWorkPos?.Invoke();
        if (pos != null)
        {
            TeachX = pos.X;
            TeachY = pos.Y;
        }
        OffsetX = TeachX;
        OffsetY = TeachY;
        StatusText = $"taught camera offset X{OffsetX:0.000} Y{OffsetY:0.000}";
    }

    [RelayCommand]
    private void ApplyOffsetAsG54()
    {
        _sendLine("G54");
        _sendLine(FormattableString.Invariant($"G10 L20 P1 X{OffsetX:0.###} Y{OffsetY:0.###}"));
        StatusBannerSafe($"G54 offset applied X{OffsetX:0.###} Y{OffsetY:0.###}");
    }

    [RelayCommand]
    private void SendJogToOffset()
    {
        var x = OffsetX.ToString(CultureInfo.InvariantCulture);
        var y = OffsetY.ToString(CultureInfo.InvariantCulture);
        _sendLine($"G90 G0 X{x} Y{y}");
        StatusText = $"jogged to camera offset X{OffsetX:0.000} Y{OffsetY:0.000}";
    }

    private void StatusBannerSafe(string s) => StatusText = s;

    public void Dispose() => _webcam.Dispose();
}
