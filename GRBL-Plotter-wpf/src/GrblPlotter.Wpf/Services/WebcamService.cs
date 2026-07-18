using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using FlashCap;

namespace GrblPlotter.Wpf.Services;

/// <summary>Live webcam frames via FlashCap (Phase 4).</summary>
public sealed class WebcamService : IDisposable
{
    private CaptureDevice? _device;

    public event Action<BitmapSource>? FrameReady;
    public bool IsRunning => _device != null;

    public static IReadOnlyList<string> ListDevices()
    {
        try
        {
            return new CaptureDevices().EnumerateDescriptors().Select(d => d.Name).ToList();
        }
        catch { return Array.Empty<string>(); }
    }

    public async Task StartAsync(int deviceIndex = 0)
    {
        await StopAsync();
        var devices = new CaptureDevices().EnumerateDescriptors().ToList();
        if (devices.Count == 0) throw new InvalidOperationException("No camera found");
        var desc = devices[Math.Clamp(deviceIndex, 0, devices.Count - 1)];
        var chars = desc.Characteristics.OrderByDescending(c => c.Width * c.Height).FirstOrDefault()
                    ?? throw new InvalidOperationException("No capture characteristics");

        _device = await desc.OpenAsync(chars, bufferScope =>
        {
            try
            {
                var image = bufferScope.Buffer.CopyImage();
                Application.Current?.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        using var ms = new MemoryStream(image);
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.StreamSource = ms;
                        bmp.EndInit();
                        bmp.Freeze();
                        FrameReady?.Invoke(bmp);
                    }
                    catch { /* drop frame */ }
                });
            }
            catch { /* drop */ }
        });
        await _device.StartAsync();
    }

    public async Task StopAsync()
    {
        if (_device == null) return;
        try { await _device.StopAsync(); } catch { }
        try { await _device.DisposeAsync(); } catch { }
        _device = null;
    }

    public void Dispose() => _ = StopAsync();
}
