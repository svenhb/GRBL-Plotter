using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GrblPlotter.Wpf.Models;
using GrblPlotter.Wpf.Services;

namespace GrblPlotter.Wpf.ViewModels;

public partial class CoordSystemRow : ObservableObject
{
    public string Name { get; init; } = "G54";
    public int PValue { get; init; }

    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;
    [ObservableProperty] private double _z;
}

/// <summary>G54-G59 work coordinate system list: activate, zero at current position (G10 L20), or
/// save absolute machine-space offsets (G10 L2). Shows the live WCO reported by the controller.</summary>
public partial class CoordSystemViewModel : ObservableObject
{
    private readonly Action<string> _sendLine;
    private readonly GrblSerialService? _serial;

    public ObservableCollection<CoordSystemRow> Systems { get; } = new();

    [ObservableProperty] private CoordSystemRow? _selectedSystem;
    [ObservableProperty] private string _currentWco = "X:0.000 Y:0.000 Z:0.000";
    [ObservableProperty] private string _activeCoordSystem = "G54";
    [ObservableProperty] private string _statusText = "";

    public CoordSystemViewModel(GrblSerialService serial)
        : this((Action<string>)serial.SendLine)
    {
        _serial = serial;
        _serial.StatusUpdated += OnStatus;
    }

    public CoordSystemViewModel(Action<string> sendLine)
    {
        _sendLine = sendLine;
        Systems.Add(new CoordSystemRow { Name = "G54", PValue = 1 });
        Systems.Add(new CoordSystemRow { Name = "G55", PValue = 2 });
        Systems.Add(new CoordSystemRow { Name = "G56", PValue = 3 });
        Systems.Add(new CoordSystemRow { Name = "G57", PValue = 4 });
        Systems.Add(new CoordSystemRow { Name = "G58", PValue = 5 });
        Systems.Add(new CoordSystemRow { Name = "G59", PValue = 6 });
        SelectedSystem = Systems[0];
    }

    private void OnStatus(GrblStatusSnapshot s) => Application.Current?.Dispatcher.Invoke(() =>
    {
        CurrentWco = $"X:{s.Wco.X:0.000} Y:{s.Wco.Y:0.000} Z:{s.Wco.Z:0.000}";
    });

    [RelayCommand]
    private void ActivateSystem(CoordSystemRow? row)
    {
        row ??= SelectedSystem;
        if (row == null) return;
        _sendLine(row.Name);
        ActiveCoordSystem = row.Name;
        StatusText = $"activated {row.Name}";
    }

    [RelayCommand]
    private void ZeroHere(CoordSystemRow? row)
    {
        row ??= SelectedSystem;
        if (row == null) return;
        _sendLine($"G10 L20 P{row.PValue} X0 Y0 Z0");
        StatusText = $"{row.Name} zeroed at current position (G10 L20 P{row.PValue})";
    }

    [RelayCommand]
    private void SaveOffset(CoordSystemRow? row)
    {
        row ??= SelectedSystem;
        if (row == null) return;
        var cmd = FormattableString.Invariant(
            $"G10 L2 P{row.PValue} X{row.X:0.###} Y{row.Y:0.###} Z{row.Z:0.###}");
        _sendLine(cmd);
        StatusText = $"saved offsets for {row.Name}: {cmd}";
    }

    [RelayCommand]
    private void QueryOffsets() => _sendLine("$#");
}
