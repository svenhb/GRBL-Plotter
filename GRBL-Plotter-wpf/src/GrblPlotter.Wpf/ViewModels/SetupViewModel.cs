using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GrblPlotter.Wpf.Services;

namespace GrblPlotter.Wpf.ViewModels;

public partial class SetupViewModel : ObservableObject
{
    [ObservableProperty] private AppSettings _settings;
    [ObservableProperty] private string _statusText = "";

    public string[] DeviceOptions { get; } = { "Laser", "Plotter", "Router" };
    public string[] RotaryAxes { get; } = { "A", "B", "C" };
    public int[] BaudRates { get; } = { 9600, 19200, 38400, 57600, 115200, 230400 };

    public SetupViewModel() : this(AppSettings.Load())
    {
    }

    public SetupViewModel(AppSettings settings)
    {
        _settings = settings;
        EnsureCustomButtons();
    }

    private void EnsureCustomButtons()
    {
        while (Settings.CustomButtons.Count < 16)
            Settings.CustomButtons.Add(new CustomButtonDto
            {
                Label = $"C{Settings.CustomButtons.Count + 1}",
                Code = ""
            });
        if (Settings.CustomButtons.Count > 16)
            Settings.CustomButtons.RemoveRange(16, Settings.CustomButtons.Count - 16);
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            Settings.Save();
            StatusText = $"Saved to {AppSettings.SettingsPath}";
        }
        catch (Exception ex)
        {
            StatusText = $"Save failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ReloadFromDisk()
    {
        Settings = AppSettings.Load();
        StatusText = "Reloaded from disk";
    }

    [RelayCommand]
    private void ResetDefaults()
    {
        Settings = new AppSettings();
        StatusText = "Reset to defaults (not yet saved)";
    }
}
