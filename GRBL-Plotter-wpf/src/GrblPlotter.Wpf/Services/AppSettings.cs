using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GrblPlotter.Wpf.Services;

public sealed class ConnectionSettings
{
    public string LastPort { get; set; } = "";
    public int LastBaud { get; set; } = 115200;
    public bool AutoConnectOnStartup { get; set; }
    public bool SendSoftResetOnConnect { get; set; }
    public int PollIntervalMs { get; set; } = 250;
}

public sealed class StreamingSettings
{
    public int PlannerBufferSlots { get; set; } = 12;
    public bool UseCheckModeBeforeRun { get; set; }
    public bool PauseOnToolChangeM6 { get; set; }
    public int SendIntervalMs { get; set; } = 20;
}

public sealed class ViewColorSettings
{
    public string BackgroundColor { get; set; } = "#FF14181E";
    public string ToolpathColor { get; set; } = "#FF6FBF8A";
    public string RapidColor { get; set; } = "#FF4A90A4";
    public string GridColor { get; set; } = "#FF3A4652";
    public string ToolPositionColor { get; set; } = "#FFE6C35C";
    public bool ShowGrid { get; set; } = true;
    public bool ShowRapidMoves { get; set; } = true;
}

public sealed class LaserDefaults
{
    public int Power { get; set; } = 500;
    public int Speed { get; set; } = 1000;
    public int Passes { get; set; } = 1;
    public bool AirAssist { get; set; }
}

public sealed class PlotterDefaults
{
    public double ZUp { get; set; } = 2;
    public double ZDown { get; set; } = -2;
    public double Speed { get; set; } = 1000;
}

public sealed class RouterDefaults
{
    public double SpeedXy { get; set; } = 200;
    public double SpeedZ { get; set; } = 100;
    public double Depth { get; set; } = -1;
}

public sealed class DeviceSettings
{
    public LaserDefaults Laser { get; set; } = new();
    public PlotterDefaults Plotter { get; set; } = new();
    public RouterDefaults Router { get; set; } = new();
    public string ActiveDevice { get; set; } = "Plotter";
}

/// <summary>Root settings object, persisted as JSON under %AppData%/GRBL-Plotter-Wpf/settings.json.</summary>
public sealed class AppSettings
{
    public ConnectionSettings Connection { get; set; } = new();
    public StreamingSettings Streaming { get; set; } = new();
    public ViewColorSettings Colors { get; set; } = new();
    public DeviceSettings Devices { get; set; } = new();
    public double JogStep { get; set; } = 1;
    public double JogFeed { get; set; } = 1000;
    public List<CustomButtonDto> CustomButtons { get; set; } = new();
    public List<string> RecentFiles { get; set; } = new();
    public bool AddImportedToView { get; set; }
    public bool ShowRapidMoves { get; set; } = true;
    public bool ShowToolpath { get; set; } = true;
    public bool ShowDimensionOverlay { get; set; } = true;
    public bool ShowGrid { get; set; } = true;
    public string CanvasMode { get; set; } = "Edit"; // Edit | JogFigure | JogClick

    // Axes / machine (Wave A–B)
    public int AxisCount { get; set; } = 3; // 3=XYZ, 4=+A, 5=+B, 6=+C
    public string RotaryAxisName { get; set; } = "A"; // A|B|C for mirror rotary
    public double MachineMinX { get; set; }
    public double MachineMinY { get; set; }
    public double MachineMaxX { get; set; } = 300;
    public double MachineMaxY { get; set; } = 200;
    public bool ShowRuler { get; set; } = true;
    public bool ShowInfoHud { get; set; } = true;
    public bool ShowMachineLimits { get; set; }
    public bool ShowFixedMachineArea { get; set; }
    public bool ShowToolTableOverlay { get; set; }

    // Import Graphic options
    public bool ImportHatchFill { get; set; }
    public double ImportHatchSpacing { get; set; } = 1.0;
    public double ImportHatchAngle { get; set; } = 45;
    public bool ImportTangential { get; set; }
    public double ImportTangentialAngle { get; set; }

    public string LastLoadedPath { get; set; } = "";
    public string Language { get; set; } = "en";

    [JsonIgnore]
    public static string SettingsDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GRBL-Plotter-Wpf");

    [JsonIgnore]
    public static string SettingsPath => Path.Combine(SettingsDirectory, "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
                if (loaded != null) return loaded;
            }
        }
        catch
        {
            // corrupt or unreadable file - fall back to defaults
        }
        return new AppSettings();
    }

    public void Save()
    {
        Directory.CreateDirectory(SettingsDirectory);
        var json = JsonSerializer.Serialize(this, JsonOptions);
        File.WriteAllText(SettingsPath, json);
    }
}

public sealed class CustomButtonDto
{
    public string Label { get; set; } = "";
    public string Code { get; set; } = "";
}
