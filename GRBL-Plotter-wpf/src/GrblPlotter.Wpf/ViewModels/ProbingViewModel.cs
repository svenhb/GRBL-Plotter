using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GrblPlotter.Wpf.Models;
using GrblPlotter.Wpf.Services;

namespace GrblPlotter.Wpf.ViewModels;

/// <summary>Edge / center / XYZ probing helper. Generates and sends G38.2 probe moves and,
/// when wired to a live <see cref="GrblSerialService"/>, parses "[PRB:...]" responses from the log.</summary>
public partial class ProbingViewModel : ObservableObject
{
    private static readonly Regex PrbRx = new(@"\[PRB:(?<vals>[-\d.,]+):(?<ok>\d)\]", RegexOptions.Compiled);

    private readonly Action<string> _sendLine;
    private readonly GrblSerialService? _serial;

    public ObservableCollection<string> Log { get; } = new();

    [ObservableProperty] private double _feed = 100;
    [ObservableProperty] private double _probeDistance = 15;
    [ObservableProperty] private double _retract = 2;
    [ObservableProperty] private double _edgeSize = 10;
    [ObservableProperty] private string _lastResultText = "no probe yet";
    [ObservableProperty] private AxisPosition? _lastProbeResult;
    [ObservableProperty] private bool _setWcoAfterProbe = true;
    [ObservableProperty] private string _statusText = "";

    public bool HasLiveFeedback => _serial != null;

    public ProbingViewModel(GrblSerialService serial)
        : this((Action<string>)serial.SendLine)
    {
        _serial = serial;
        _serial.LogReceived += OnLog;
        StatusText = "connected to live serial log - PRB results will populate automatically";
    }

    public ProbingViewModel(Action<string> sendLine)
    {
        _sendLine = sendLine;
        StatusText = HasLiveFeedback ? "" : "no live serial attached - watch the COM CNC log for [PRB:...] results";
    }

    private void OnLog(string line)
    {
        var m = PrbRx.Match(line);
        if (!m.Success) return;
        Application.Current?.Dispatcher.Invoke(() =>
        {
            var nums = m.Groups["vals"].Value.Split(',');
            var pos = new AxisPosition
            {
                X = nums.Length > 0 ? ParseD(nums[0]) : 0,
                Y = nums.Length > 1 ? ParseD(nums[1]) : 0,
                Z = nums.Length > 2 ? ParseD(nums[2]) : 0
            };
            LastProbeResult = pos;
            LastResultText = $"PRB X:{pos.X:0.000} Y:{pos.Y:0.000} Z:{pos.Z:0.000} (contact={m.Groups["ok"].Value})";
            Log.Add(line);
            while (Log.Count > 200) Log.RemoveAt(0);
        });
    }

    private static double ParseD(string s) =>
        double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : 0;

    private void SendProbe(char axis, int dir)
    {
        var dist = (ProbeDistance * dir).ToString(CultureInfo.InvariantCulture);
        var feed = Feed.ToString(CultureInfo.InvariantCulture);
        _sendLine($"G91 G38.2 {axis}{dist} F{feed}");
        var back = (-dir * Retract).ToString(CultureInfo.InvariantCulture);
        _sendLine($"G91 G0 {axis}{back}");
        _sendLine("G90");
    }

    [RelayCommand] private void ProbeZDown() => SendProbe('Z', -1);
    [RelayCommand] private void ProbeXPos() => SendProbe('X', 1);
    [RelayCommand] private void ProbeXNeg() => SendProbe('X', -1);
    [RelayCommand] private void ProbeYPos() => SendProbe('Y', 1);
    [RelayCommand] private void ProbeYNeg() => SendProbe('Y', -1);

    [RelayCommand]
    private void ProbeCenterXy()
    {
        SendProbe('X', 1);
        SendProbe('X', -1);
        SendProbe('Y', 1);
        SendProbe('Y', -1);
        StatusText = "center-finding sequence sent (X+, X-, Y+, Y-)";
    }

    [RelayCommand]
    private void ProbeCornerXyz()
    {
        SendProbe('Z', -1);
        SendProbe('X', 1);
        SendProbe('Y', 1);
        StatusText = "corner XYZ probe sequence sent (Z, X+, Y+)";
    }

    [RelayCommand]
    private void ZeroAtLastResult()
    {
        _sendLine("G10 L20 P0 Z0");
        StatusText = "Z zeroed at current position (G10 L20 P0 Z0)";
    }

    [RelayCommand]
    private void ClearLog() => Log.Clear();
}
