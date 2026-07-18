using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GrblPlotter.Wpf.Models;
using GrblPlotter.Wpf.Services;
using Microsoft.Win32;

namespace GrblPlotter.Wpf.ViewModels;

/// <summary>Defines a probe grid, generates/sends the probe pattern, captures live results into a
/// <see cref="HeightMap"/>, and can apply the map's Z compensation onto the currently loaded document.</summary>
public partial class HeightMapViewModel : ObservableObject
{
    private static readonly Regex PrbRx = new(@"\[PRB:(?<vals>[-\d.,]+):(?<ok>\d)\]", RegexOptions.Compiled);

    private readonly Action<string> _sendLine;
    private readonly GrblSerialService? _serial;
    private readonly Func<GCodeDocument>? _getDoc;
    private readonly Action<GCodeDocument>? _setDoc;

    [ObservableProperty] private double _minX;
    [ObservableProperty] private double _minY;
    [ObservableProperty] private double _maxX = 100;
    [ObservableProperty] private double _maxY = 100;
    [ObservableProperty] private int _gridX = 5;
    [ObservableProperty] private int _gridY = 5;
    [ObservableProperty] private double _safeZ = 5;
    [ObservableProperty] private double _probeDepth = -10;
    [ObservableProperty] private double _probeFeed = 100;
    [ObservableProperty] private double _travelFeed = 1500;
    [ObservableProperty] private double _retract = 2;
    [ObservableProperty] private string _statusText = "no map loaded";
    [ObservableProperty] private bool _isProbing;
    [ObservableProperty] private int _probedCount;

    public ObservableCollection<string> GeneratedGCode { get; } = new();
    public HeightMap? Map { get; private set; }
    public bool CanLiveProbe => _serial != null;

    public HeightMapViewModel(GrblSerialService serial, Func<GCodeDocument> getDoc, Action<GCodeDocument> setDoc)
        : this((Action<string>)serial.SendLine, getDoc, setDoc)
    {
        _serial = serial;
    }

    public HeightMapViewModel(Action<string> sendLine, Func<GCodeDocument>? getDoc = null, Action<GCodeDocument>? setDoc = null)
    {
        _sendLine = sendLine;
        _getDoc = getDoc;
        _setDoc = setDoc;
    }

    [RelayCommand]
    private void GeneratePattern()
    {
        var grid = BuildGridDefinition();
        var lines = HeightMapService.GenerateProbePattern(grid, SafeZ, ProbeDepth, ProbeFeed, TravelFeed, Retract);
        GeneratedGCode.Clear();
        foreach (var l in lines) GeneratedGCode.Add(l);
        StatusText = $"generated {lines.Count} lines for a {grid.GridX}x{grid.GridY} grid";
    }

    [RelayCommand]
    private void SendPattern()
    {
        if (GeneratedGCode.Count == 0) GeneratePattern();
        foreach (var l in GeneratedGCode)
            if (!l.StartsWith(';')) _sendLine(l);
        StatusText = "pattern sent to controller";
    }

    [RelayCommand]
    private async Task RunProbingAsync()
    {
        if (_serial == null)
        {
            StatusText = "live probing requires this window to be wired to a GrblSerialService";
            return;
        }
        if (!_serial.IsConnected)
        {
            StatusText = "connect to the controller first";
            return;
        }

        IsProbing = true;
        ProbedCount = 0;
        var grid = BuildGridDefinition();
        var newMap = new HeightMap { GridX = grid.GridX, GridY = grid.GridY, MinX = MinX, MinY = MinY, MaxX = MaxX, MaxY = MaxY };
        for (var i = 0; i < grid.GridX * grid.GridY; i++) newMap.Points.Add(new HeightMapPoint());

        _sendLine(FormattableString.Invariant($"G0 Z{SafeZ:0.###}"));
        for (var iy = 0; iy < grid.GridY; iy++)
        {
            var y = grid.MinY + iy * grid.CellHeight;
            var order = Enumerable.Range(0, grid.GridX);
            if (iy % 2 == 1) order = order.Reverse();

            foreach (var ix in order)
            {
                var x = grid.MinX + ix * grid.CellWidth;
                _sendLine(FormattableString.Invariant($"G0 X{x:0.###} Y{y:0.###} F{TravelFeed:0.###}"));
                var z = await ProbeOnceAsync();
                newMap.Points[iy * grid.GridX + ix] = new HeightMapPoint { X = x, Y = y, Z = z };
                _sendLine(FormattableString.Invariant($"G91 G0 Z{Retract:0.###}"));
                _sendLine("G90");
                ProbedCount++;
                StatusText = $"probing… {ProbedCount}/{grid.GridX * grid.GridY}";
            }
        }
        _sendLine(FormattableString.Invariant($"G0 Z{SafeZ:0.###}"));

        Map = newMap;
        IsProbing = false;
        StatusText = $"probing complete: {newMap.Points.Count} points captured";
    }

    private Task<double> ProbeOnceAsync()
    {
        var tcs = new TaskCompletionSource<double>();
        void Handler(string line)
        {
            var m = PrbRx.Match(line);
            if (!m.Success) return;
            _serial!.LogReceived -= Handler;
            var nums = m.Groups["vals"].Value.Split(',');
            var z = nums.Length > 2 ? ParseD(nums[2]) : 0;
            tcs.TrySetResult(z);
        }
        _serial!.LogReceived += Handler;
        _sendLine(FormattableString.Invariant($"G91 G38.2 Z{ProbeDepth:0.###} F{ProbeFeed:0.###}"));
        _sendLine("G90");
        return tcs.Task;
    }

    private static double ParseD(string s) =>
        double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : 0;

    private HeightMap BuildGridDefinition() => new()
    {
        GridX = Math.Max(2, GridX),
        GridY = Math.Max(2, GridY),
        MinX = MinX,
        MinY = MinY,
        MaxX = MaxX,
        MaxY = MaxY
    };

    [RelayCommand]
    private void SaveCsv()
    {
        if (Map == null) { StatusText = "no probed map to save yet"; return; }
        var dlg = new SaveFileDialog { Filter = "Height map CSV (*.csv)|*.csv", FileName = "heightmap.csv" };
        if (dlg.ShowDialog() != true) return;
        HeightMapService.SaveCsv(dlg.FileName, Map);
        StatusText = $"saved {dlg.FileName}";
    }

    [RelayCommand]
    private void LoadCsv()
    {
        var dlg = new OpenFileDialog { Filter = "Height map CSV (*.csv)|*.csv" };
        if (dlg.ShowDialog() != true) return;
        Map = HeightMapService.LoadCsv(dlg.FileName);
        MinX = Map.MinX; MinY = Map.MinY; MaxX = Map.MaxX; MaxY = Map.MaxY;
        GridX = Map.GridX; GridY = Map.GridY;
        StatusText = $"loaded {Map.Points.Count} points from {System.IO.Path.GetFileName(dlg.FileName)}";
    }

    [RelayCommand]
    private void ApplyToLoadedGCode()
    {
        if (Map == null) { StatusText = "load or probe a height map first"; return; }
        if (_getDoc == null || _setDoc == null) { StatusText = "no document bound to apply to"; return; }
        var doc = _getDoc();
        if (doc.IsEmpty) { StatusText = "no g-code loaded"; return; }
        var compensated = HeightMapService.ApplyToGCode(doc.Lines, Map);
        var newDoc = GCodeParser.Parse(string.Join(Environment.NewLine, compensated), doc.FilePath);
        _setDoc(newDoc);
        StatusText = $"applied height map compensation to {compensated.Count} lines";
    }
}
