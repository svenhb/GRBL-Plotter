using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GrblPlotter.Wpf.Models;
using GrblPlotter.Wpf.Services;
using GrblPlotter.Wpf.Services.Import;
using GrblPlotter.Wpf.Services.Transform;
// HatchService in Services.Import
using Microsoft.Win32;

namespace GrblPlotter.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly GrblSerialService _serial = new();
    private readonly GCodeStreamer _streamer;
    private readonly PathSimulator _simulator = new();
    private readonly DispatcherTimer _uiTimer;
    private readonly Stack<string> _undoStack = new();
    private readonly AppSettings _settings;

    public GrblSerialService Serial => _serial;
    public AppSettings Settings => _settings;
    public ObservableCollection<string> LogLines { get; } = new();
    public ObservableCollection<string> Ports { get; } = new();
    public ObservableCollection<int> BaudRates { get; } = new() { 9600, 19200, 38400, 57600, 115200, 230400 };
    public ObservableCollection<CustomButtonItem> CustomButtons { get; } = new();

    [ObservableProperty] private string _selectedPort = "";
    [ObservableProperty] private int _selectedBaud = 115200;
    [ObservableProperty] private bool _isConnected;
    [ObservableProperty] private string _connectionText = "not connected";
    [ObservableProperty] private string _machineStateText = "unknown";
    [ObservableProperty] private string _statusBanner = "nothing loaded";
    [ObservableProperty] private string _windowTitle = "GRBL Plotter WPF Ver.:1.8.0.2 | grbl: not connected";

    private static string L(string key, string fallback) => LocalizationService.Get(key, fallback);

    [ObservableProperty] private string _workX = "00000.000";
    [ObservableProperty] private string _workY = "00000.000";
    [ObservableProperty] private string _workZ = "00000.000";
    [ObservableProperty] private string _machineX = "00000.000";
    [ObservableProperty] private string _machineY = "00000.000";
    [ObservableProperty] private string _machineZ = "00000.000";
    [ObservableProperty] private string _coordSystem = "G54";
    [ObservableProperty] private double _feedRate;
    [ObservableProperty] private double _spindleRate;

    [ObservableProperty] private string _gcodeText = "";
    [ObservableProperty] private string _fileLabel = "nothing loaded";
    [ObservableProperty] private double _streamProgress;
    [ObservableProperty] private string _streamInfo = "Prog 0%  Time —";
    [ObservableProperty] private bool _isStreaming;
    [ObservableProperty] private bool _isSimulating;
    [ObservableProperty] private string _dimensionText = "—";

    [ObservableProperty] private int _ovFeed = 100;
    [ObservableProperty] private int _ovRapid = 100;
    [ObservableProperty] private int _ovSpindle = 100;

    [ObservableProperty] private double _jogStep = 1;
    [ObservableProperty] private double _jogFeed = 1000;

    [ObservableProperty] private int _laserPower = 500;
    [ObservableProperty] private int _laserSpeed = 1000;
    [ObservableProperty] private int _laserPasses = 1;
    [ObservableProperty] private bool _airAssist;

    [ObservableProperty] private double _plotterZUp = 2;
    [ObservableProperty] private double _plotterZDown = -2;
    [ObservableProperty] private double _plotterSpeed = 1000;

    [ObservableProperty] private double _routerSpeedXy = 200;
    [ObservableProperty] private double _routerSpeedZ = 100;
    [ObservableProperty] private double _routerDepth = -1;

    [ObservableProperty] private Geometry? _toolpathGeometry;
    [ObservableProperty] private Geometry? _rapidGeometry;
    [ObservableProperty] private Geometry? _selectionGeometry;
    [ObservableProperty] private Geometry? _backgroundGeometry;
    [ObservableProperty] private Visibility _selectionVisibility = Visibility.Collapsed;
    [ObservableProperty] private Visibility _backgroundVisibility = Visibility.Collapsed;
    [ObservableProperty] private Visibility _markerVisibility = Visibility.Collapsed;
    [ObservableProperty] private double _markerCanvasX;
    [ObservableProperty] private double _markerCanvasY;
    [ObservableProperty] private ImageSource? _placeholderImage;
    [ObservableProperty] private bool _showPlaceholder = true;
    [ObservableProperty] private double _simX;
    [ObservableProperty] private double _simY;
    [ObservableProperty] private Visibility _simMarkerVisibility = Visibility.Collapsed;
    [ObservableProperty] private double _previewWidth = 400;
    [ObservableProperty] private double _previewHeight = 300;

    // World → canvas mapping (updated in BuildPreview)
    private double _mapMinX, _mapMaxY, _mapScale = 1;
    private double PreviewPad => ShowRuler ? 36 : 10;
    private const double PreviewTarget = 400;

    [ObservableProperty] private double _transformScale = 1.0;
    [ObservableProperty] private double _transformRotate = 90;
    [ObservableProperty] private double _transformDx;
    [ObservableProperty] private double _transformDy;
    [ObservableProperty] private double _transformTargetWidth = 100;
    [ObservableProperty] private double _transformTargetHeight = 100;

    [ObservableProperty] private bool _addImportedToView;
    [ObservableProperty] private bool _showRapidMoves = true;
    [ObservableProperty] private bool _showToolpath = true;
    [ObservableProperty] private bool _showDimensionOverlay = true;
    [ObservableProperty] private string _canvasMode = "Edit"; // Edit | JogFigure | JogClick
    [ObservableProperty] private string _urlPaste = "";
    [ObservableProperty] private Visibility _rapidVisibility = Visibility.Visible;
    [ObservableProperty] private Visibility _toolpathVisibility = Visibility.Visible;
    [ObservableProperty] private Visibility _dimensionVisibility = Visibility.Visible;
    [ObservableProperty] private bool _modeEdit = true;
    [ObservableProperty] private bool _modeJogFigure;
    [ObservableProperty] private bool _modeJogClick;

    [ObservableProperty] private bool _useAbsoluteOrigin; // transform pivot at 0,0
    [ObservableProperty] private double _transformScaleX = 1.0;
    [ObservableProperty] private double _transformScaleY = 1.0;
    [ObservableProperty] private double _rotaryDiameter = 50;
    [ObservableProperty] private double _radiusComp = 0.5;
    [ObservableProperty] private bool _moveUseG0 = true;
    [ObservableProperty] private ToolEntry? _selectedTool;

    public ObservableCollection<string> RecentFiles { get; } = new();
    public ObservableCollection<ToolEntry> Tools { get; } = new();
    public GCodeDocument Document { get; private set; } = new();
    public AxisPosition CurrentWorkPos { get; } = new();

    private readonly List<int> _selectedIndices = new();
    private double _markerWorldX, _markerWorldY;
    private bool _hasMarker;
    private List<GCodeSegment>? _backgroundSegments;

    public MainViewModel()
    {
        _settings = AppSettings.Load();
        SelectedBaud = _settings.Connection.LastBaud;
        SelectedPort = _settings.Connection.LastPort ?? "";
        LaserPower = _settings.Devices.Laser.Power;
        LaserSpeed = _settings.Devices.Laser.Speed;
        LaserPasses = _settings.Devices.Laser.Passes;
        AirAssist = _settings.Devices.Laser.AirAssist;
        PlotterZUp = _settings.Devices.Plotter.ZUp;
        PlotterZDown = _settings.Devices.Plotter.ZDown;
        PlotterSpeed = _settings.Devices.Plotter.Speed;
        RouterSpeedXy = _settings.Devices.Router.SpeedXy;
        RouterSpeedZ = _settings.Devices.Router.SpeedZ;
        RouterDepth = _settings.Devices.Router.Depth;
        JogStep = _settings.JogStep;
        JogFeed = _settings.JogFeed;
        AddImportedToView = _settings.AddImportedToView;
        ShowRapidMoves = _settings.ShowRapidMoves;
        ShowToolpath = _settings.ShowToolpath;
        ShowDimensionOverlay = _settings.ShowDimensionOverlay;
        CanvasMode = string.IsNullOrEmpty(_settings.CanvasMode) ? "Edit" : _settings.CanvasMode;
        ApplyCanvasModeFlags();
        ApplyViewVisibilities();
        foreach (var f in _settings.RecentFiles.Where(File.Exists).Take(12))
            RecentFiles.Add(f);

        _streamer = new GCodeStreamer(_serial);
        _serial.LogReceived += OnLog;
        _serial.StatusUpdated += OnStatus;
        _serial.ConnectionChanged += c =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsConnected = c;
                ConnectionText = c
                    ? $"{L("Str.Connected", "connected")} {_serial.PortName}"
                    : L("Str.NotConnected", "not connected");
                RefreshTitle();
                if (!c && !IsSimulating)
                    SimMarkerVisibility = Visibility.Collapsed;
            });
        };
        _streamer.ProgressChanged += () =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StreamProgress = _streamer.Progress;
                StreamInfo = $"{L("Str.Prog", "Prog")} {_streamer.Progress:0}%  {L("Str.Line", "Line")} {_streamer.CurrentLine}/{_streamer.TotalLines}";
                IsStreaming = _streamer.IsRunning;
            });
        };
        _streamer.Completed += () =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StatusBanner = L("Str.StreamingComplete", "streaming complete");
                IsStreaming = false;
            });
        };

        _simulator.PositionChanged += p =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Map machine/world coords into preview canvas pixels
                SimX = (p.X - _mapMinX) * _mapScale + PreviewPad;
                SimY = (_mapMaxY - p.Y) * _mapScale + PreviewPad;
                SimMarkerVisibility = Visibility.Visible;
            });
        };
        _simulator.ProgressChanged += pct =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StreamProgress = pct * 100;
                StreamInfo = $"Sim {pct * 100:0}%";
            });
        };
        _simulator.Completed += () =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsSimulating = false;
                StatusBanner = L("Str.SimulationComplete", "simulation complete");
                SimMarkerVisibility = Visibility.Collapsed;
            });
        };

        ConnectionText = L("Str.NotConnected", "not connected");
        MachineStateText = L("Str.Unknown", "unknown");
        StatusBanner = L("Str.NothingLoaded", "nothing loaded");
        FileLabel = L("Str.NothingLoaded", "nothing loaded");
        StreamInfo = $"{L("Str.Prog", "Prog")} 0%  {L("Str.Time", "Time")} —";
        RefreshTitle();

        try
        {
            PlaceholderImage = new System.Windows.Media.Imaging.BitmapImage(
                new Uri("pack://application:,,,/Assets/modell.png"));
        }
        catch { /* optional */ }

        LoadCustomButtons();
        InitParityFromSettings();
        BuildPreview(); // establish map + ruler for empty workspace
        RefreshPorts();
        if (!string.IsNullOrEmpty(_settings.Connection.LastPort) && Ports.Contains(_settings.Connection.LastPort))
            SelectedPort = _settings.Connection.LastPort;

        _uiTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _uiTimer.Tick += (_, _) => { if (!IsConnected) RefreshPorts(); };
        _uiTimer.Start();
    }

    partial void InitParityFromSettings();

    private void LoadCustomButtons()
    {
        if (_settings.CustomButtons.Count == 0)
        {
            var defaults = new (string Label, string Code)[]
            {
                ("Home", "$H"), ("Unlock", "$X"), ("Sleep", "$SLP"), ("$$", "$$"),
                ("$G", "$G"), ("M5", "M5"), ("M8", "M8"), ("M9", "M9"),
            };
            for (int i = 0; i < 16; i++)
            {
                _settings.CustomButtons.Add(i < defaults.Length
                    ? new CustomButtonDto { Label = defaults[i].Label, Code = defaults[i].Code }
                    : new CustomButtonDto { Label = $"C{i + 1}", Code = "" });
            }
        }
        RefreshCustomButtonsFromSettings();
    }

    private void RefreshTitle() =>
        WindowTitle = $"GRBL Plotter WPF Ver.:1.8.0.2 | grbl: {ConnectionText}";

    private void PushUndo()
    {
        _undoStack.Push(GcodeText);
        while (_undoStack.Count > 30) { var tmp = _undoStack.Reverse().Take(30).Reverse().ToList(); _undoStack.Clear(); foreach (var t in tmp) _undoStack.Push(t); }
    }

    public void ApplyDocument(GCodeDocument doc, bool pushUndo = true)
    {
        if (pushUndo) PushUndo();
        Document = doc;
        GcodeText = string.Join(Environment.NewLine, doc.Lines);
        FileLabel = doc.FileName;
        _streamer.Load(doc);
        _simulator.Load(doc);
        BuildPreview();
        ShowPlaceholder = doc.Segments.Count == 0;
        DimensionText = doc.Segments.Count == 0
            ? "—"
            : $"X:{doc.MinX:0.###}…{doc.MaxX:0.###}  Y:{doc.MinY:0.###}…{doc.MaxY:0.###}";
        StatusBanner = $"{L("Str.Loaded", "loaded")} {doc.FileName}  ({doc.Lines.Count} {L("Str.Lines", "lines")}, {doc.Segments.Count} {L("Str.Segs", "segs")})";
    }

    public void ApplyGeneratedGCode(string gcode, string name = "generated")
    {
        var doc = GCodeParser.Parse(gcode, name);
        ApplyDocument(doc);
    }

    public void SetStatus(string msg) => StatusBanner = msg;

    [RelayCommand]
    private void RefreshPorts()
    {
        var ports = GrblSerialService.GetPortNames();
        Ports.Clear();
        foreach (var p in ports) Ports.Add(p);
        if (Ports.Count > 0 && (string.IsNullOrEmpty(SelectedPort) || !Ports.Contains(SelectedPort)))
            SelectedPort = Ports[0];
        if (Ports.Count == 0)
            StatusBanner = L("Str.NoPorts", "no serial ports found");
    }

    [RelayCommand]
    private void ConnectToggle()
    {
        try
        {
            if (IsConnected) _serial.Disconnect();
            else
            {
                if (string.IsNullOrEmpty(SelectedPort))
                {
                    StatusBanner = L("Str.SelectPort", "select a COM port");
                    return;
                }
                _serial.Connect(SelectedPort, SelectedBaud);
                _settings.Connection.LastPort = SelectedPort;
                _settings.Connection.LastBaud = SelectedBaud;
                _settings.Save();
            }
        }
        catch (Exception ex)
        {
            StatusBanner = $"COM Port {SelectedPort} {L("Str.ComFailed", "failed")}: {ex.Message}";
            OnLog($"[ERROR] {ex.Message}");
        }
    }

    [RelayCommand]
    private void OpenFile()
    {
        var dlg = new OpenFileDialog
        {
            Filter =
                "All supported|*.nc;*.gcode;*.ngc;*.tap;*.svg;*.dxf;*.hpgl;*.plt;*.gbr;*.ger;*.csv;*.txt;*.png;*.jpg;*.jpeg;*.bmp|" +
                "G-Code|*.nc;*.gcode;*.ngc;*.tap|" +
                "Vector|*.svg;*.dxf;*.hpgl;*.plt|" +
                "Gerber/Drill|*.gbr;*.ger;*.csv|" +
                "Images|*.png;*.jpg;*.jpeg;*.bmp|" +
                "All files|*.*"
        };
        if (dlg.ShowDialog() != true) return;
        LoadPath(dlg.FileName);
    }

    public void LoadPath(string path)
    {
        try
        {
            var doc = ImportFacade.OpenAny(path);
            doc = PostProcessImport(doc);
            if (AddImportedToView && Document.Segments.Count > 0 && doc.Segments.Count > 0)
            {
                PushUndo();
                var merged = new GCodeDocument { FilePath = doc.FilePath };
                merged.Lines.AddRange(Document.Lines);
                if (merged.Lines.Count > 0 && !string.IsNullOrWhiteSpace(merged.Lines[^1]))
                    merged.Lines.Add("");
                merged.Lines.Add($"; --- append {Path.GetFileName(path)} ---");
                merged.Lines.AddRange(doc.Lines);
                merged.Segments.AddRange(Document.Segments);
                merged.Segments.AddRange(doc.Segments);
                merged.MinX = Math.Min(Document.MinX, doc.MinX);
                merged.MaxX = Math.Max(Document.MaxX, doc.MaxX);
                merged.MinY = Math.Min(Document.MinY, doc.MinY);
                merged.MaxY = Math.Max(Document.MaxY, doc.MaxY);
                ApplyDocument(merged, pushUndo: false);
            }
            else
            {
                ApplyDocument(doc);
            }
            RememberRecent(path);
            TrackLoadedPath(path);
        }
        catch (Exception ex)
        {
            StatusBanner = $"{L("Str.LoadFailed", "Load failed")}: {ex.Message}";
            OnLog($"[ERROR] {ex.Message}");
        }
    }

    private GCodeDocument PostProcessImport(GCodeDocument doc)
    {
        if (doc.Segments.Count == 0) return doc;
        if (_settings.ImportHatchFill)
            doc = HatchService.HatchDocument(doc, _settings.ImportHatchSpacing, _settings.ImportHatchAngle);
        if (_settings.ImportTangential)
            doc = TangentialService.Apply(doc, _settings.RotaryAxisName, _settings.ImportTangentialAngle);
        return doc;
    }

    private void RememberRecent(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;
        RecentFiles.Remove(path);
        RecentFiles.Insert(0, path);
        while (RecentFiles.Count > 12) RecentFiles.RemoveAt(RecentFiles.Count - 1);
        _settings.RecentFiles = RecentFiles.ToList();
        _settings.Save();
    }

    [RelayCommand]
    private void OpenRecent(string? path)
    {
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
            LoadPath(path);
        else
            StatusBanner = L("Str.RecentMissing", "recent file missing");
    }

    [RelayCommand]
    private void LoadFromUrl()
    {
        var url = (UrlPaste ?? "").Trim();
        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                using var http = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                var bytes = http.GetByteArrayAsync(url).GetAwaiter().GetResult();
                var ext = Path.GetExtension(new Uri(url).AbsolutePath);
                if (string.IsNullOrEmpty(ext)) ext = ".svg";
                var tmp = Path.Combine(Path.GetTempPath(), "grbl-url-" + Guid.NewGuid().ToString("N") + ext);
                File.WriteAllBytes(tmp, bytes);
                LoadPath(tmp);
                StatusBanner = $"loaded URL → {Path.GetFileName(tmp)}";
            }
            catch (Exception ex)
            {
                StatusBanner = $"URL load failed: {ex.Message}";
            }
            return;
        }
        if (File.Exists(url))
        {
            LoadPath(url);
            return;
        }
        StatusBanner = L("Str.PastePathHint", "paste a file path or http(s) URL");
    }

    partial void OnShowRapidMovesChanged(bool value)
    {
        RapidVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        _settings.ShowRapidMoves = value; _settings.Save();
    }
    partial void OnShowToolpathChanged(bool value)
    {
        ToolpathVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        _settings.ShowToolpath = value; _settings.Save();
    }
    partial void OnShowDimensionOverlayChanged(bool value)
    {
        DimensionVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        _settings.ShowDimensionOverlay = value; _settings.Save();
    }
    partial void OnAddImportedToViewChanged(bool value)
    {
        _settings.AddImportedToView = value; _settings.Save();
    }

    private void ApplyViewVisibilities()
    {
        RapidVisibility = ShowRapidMoves ? Visibility.Visible : Visibility.Collapsed;
        ToolpathVisibility = ShowToolpath ? Visibility.Visible : Visibility.Collapsed;
        DimensionVisibility = ShowDimensionOverlay ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ApplyCanvasModeFlags()
    {
        ModeEdit = CanvasMode == "Edit";
        ModeJogFigure = CanvasMode == "JogFigure";
        ModeJogClick = CanvasMode == "JogClick";
    }

    partial void OnModeEditChanged(bool value) { if (value) SetCanvasMode("Edit"); }
    partial void OnModeJogFigureChanged(bool value) { if (value) SetCanvasMode("JogFigure"); }
    partial void OnModeJogClickChanged(bool value) { if (value) SetCanvasMode("JogClick"); }

    private void SetCanvasMode(string mode)
    {
        if (CanvasMode == mode) return;
        CanvasMode = mode;
        _settings.CanvasMode = mode;
        _settings.Save();
        ApplyCanvasModeFlags();
        StatusBanner = mode switch
        {
            "JogFigure" => L("Str.StatusModeJogFigure", "mode: jog to figure position"),
            "JogClick" => L("Str.StatusModeJogClick", "mode: jog to clicked position"),
            _ => L("Str.ModeEditBanner", "mode: edit figure")
        };
    }

    public (double X, double Y) CanvasToWorld(double canvasX, double canvasY)
    {
        if (_mapScale <= 0) return (0, 0);
        double worldX = (canvasX - PreviewPad) / _mapScale + _mapMinX;
        double worldY = _mapMaxY - (canvasY - PreviewPad) / _mapScale;
        return (worldX, worldY);
    }

    /// <summary>Map a click in preview canvas pixels: edit=select path, jog modes=G0.</summary>
    public void CanvasClick(double canvasX, double canvasY)
    {
        var (worldX, worldY) = CanvasToWorld(canvasX, canvasY);
        if (CanvasMode is "JogFigure" or "JogClick")
        {
            _serial.SendLine(FormattableString.Invariant($"G90 G0 X{worldX:0.###} Y{worldY:0.###}"));
            StatusBanner = $"move to X{worldX:0.###} Y{worldY:0.###}";
            return;
        }
        // Edit mode: select nearest path
        double span = Math.Max(Document.MaxX - Document.MinX, Document.MaxY - Document.MinY);
        double tol = Math.Max(span * 0.02, 8.0 / Math.Max(_mapScale, 1e-6));
        int hit = PathEditService.HitTestSegment(Document, worldX, worldY, tol);
        _selectedIndices.Clear();
        if (hit >= 0)
        {
            _selectedIndices.AddRange(PathEditService.ExpandToPath(Document, hit));
            StatusBanner = $"selected path ({_selectedIndices.Count} segs)";
        }
        else
            StatusBanner = L("Str.NoPathHit", "no path hit");
        RebuildSelectionGeometry();
    }

    public void CanvasSetMarker(double canvasX, double canvasY)
    {
        var (wx, wy) = CanvasToWorld(canvasX, canvasY);
        _markerWorldX = wx;
        _markerWorldY = wy;
        _hasMarker = true;
        MarkerCanvasX = canvasX;
        MarkerCanvasY = canvasY;
        MarkerVisibility = Visibility.Visible;
        StatusBanner = $"marker X{wx:0.###} Y{wy:0.###}";
    }

    private void RebuildSelectionGeometry()
    {
        if (_selectedIndices.Count == 0 || _mapScale <= 0)
        {
            SelectionGeometry = null;
            SelectionVisibility = Visibility.Collapsed;
            return;
        }
        Point Map(double x, double y) =>
            new((x - _mapMinX) * _mapScale + PreviewPad, (_mapMaxY - y) * _mapScale + PreviewPad);
        var g = new StreamGeometry();
        using (var ctx = g.Open())
        {
            foreach (var i in _selectedIndices)
            {
                if (i < 0 || i >= Document.Segments.Count) continue;
                var s = Document.Segments[i];
                ctx.BeginFigure(Map(s.X0, s.Y0), false, false);
                ctx.LineTo(Map(s.X1, s.Y1), true, false);
            }
        }
        g.Freeze();
        SelectionGeometry = g;
        SelectionVisibility = Visibility.Visible;
    }

    private void RebuildBackgroundGeometry()
    {
        if (_backgroundSegments == null || _backgroundSegments.Count == 0 || _mapScale <= 0)
        {
            BackgroundGeometry = null;
            BackgroundVisibility = Visibility.Collapsed;
            return;
        }
        Point Map(double x, double y) =>
            new((x - _mapMinX) * _mapScale + PreviewPad, (_mapMaxY - y) * _mapScale + PreviewPad);
        var g = new StreamGeometry();
        using (var ctx = g.Open())
        {
            foreach (var s in _backgroundSegments)
            {
                ctx.BeginFigure(Map(s.X0, s.Y0), false, false);
                ctx.LineTo(Map(s.X1, s.Y1), true, false);
            }
        }
        g.Freeze();
        BackgroundGeometry = g;
        BackgroundVisibility = Visibility.Visible;
    }

    [RelayCommand]
    private void SaveFile()
    {
        var dlg = new SaveFileDialog { Filter = "G-Code (*.nc)|*.nc|All files (*.*)|*.*", FileName = Document.FileName };
        if (dlg.ShowDialog() != true) return;
        File.WriteAllText(dlg.FileName, GcodeText);
        Document.FilePath = dlg.FileName;
        FileLabel = Document.FileName;
        StatusBanner = $"{L("Str.Saved", "saved")} {Document.FileName}";
    }

    [RelayCommand]
    private void ApplyEditor()
    {
        PushUndo();
        Document = GCodeParser.Parse(GcodeText, Document.FilePath);
        _streamer.Load(Document);
        _simulator.Load(Document);
        BuildPreview();
        ShowPlaceholder = Document.Segments.Count == 0;
        DimensionText = Document.Segments.Count == 0
            ? "—"
            : $"X:{Document.MinX:0.###}…{Document.MaxX:0.###}  Y:{Document.MinY:0.###}…{Document.MaxY:0.###}";
        StatusBanner = $"parsed {Document.Lines.Count} lines / {Document.Segments.Count} segments";
    }

    [RelayCommand]
    private void Undo()
    {
        if (_undoStack.Count == 0) { StatusBanner = "nothing to undo"; return; }
        GcodeText = _undoStack.Pop();
        Document = GCodeParser.Parse(GcodeText, Document.FilePath);
        _streamer.Load(Document);
        _simulator.Load(Document);
        BuildPreview();
        ShowPlaceholder = Document.Segments.Count == 0;
        StatusBanner = "undo";
    }

    [RelayCommand]
    private void ClearWorkspace()
    {
        PushUndo();
        Document = new GCodeDocument();
        GcodeText = "";
        FileLabel = "nothing loaded";
        ToolpathGeometry = null;
        RapidGeometry = null;
        ShowPlaceholder = true;
        DimensionText = "—";
        StatusBanner = "workspace cleared";
    }

    private void BuildPreview()
    {
        if (Document.Segments.Count == 0)
        {
            ToolpathGeometry = null;
            RapidGeometry = null;
            PreviewWidth = 400;
            PreviewHeight = 300;
            _mapScale = 1;
            _mapMinX = _settings.MachineMinX;
            _mapMaxY = _settings.MachineMaxY;
            if (ShowFixedMachineArea || ShowMachineLimits || ShowRuler)
            {
                double mSpanX = Math.Max(_settings.MachineMaxX - _settings.MachineMinX, 1);
                double mSpanY = Math.Max(_settings.MachineMaxY - _settings.MachineMinY, 1);
                _mapScale = PreviewTarget / Math.Max(mSpanX, mSpanY);
                PreviewWidth = mSpanX * _mapScale + PreviewPad * 2;
                PreviewHeight = mSpanY * _mapScale + PreviewPad * 2;
            }
            RebuildOverlays();
            return;
        }

        double minX = Document.MinX, maxX = Document.MaxX;
        double minY = Document.MinY, maxY = Document.MaxY;
        // Also scan segments in case bounds were stale
        foreach (var s in Document.Segments)
        {
            minX = Math.Min(minX, Math.Min(s.X0, s.X1));
            maxX = Math.Max(maxX, Math.Max(s.X0, s.X1));
            minY = Math.Min(minY, Math.Min(s.Y0, s.Y1));
            maxY = Math.Max(maxY, Math.Max(s.Y0, s.Y1));
        }

        double spanX = Math.Max(maxX - minX, 1e-6);
        double spanY = Math.Max(maxY - minY, 1e-6);
        // Fit bbox into a ~PreviewTarget box (CNC: Y up → screen Y down via maxY - y)
        double scale = PreviewTarget / Math.Max(spanX, spanY);
        _mapScale = scale;
        _mapMinX = minX;
        _mapMaxY = maxY;

        PreviewWidth = spanX * scale + PreviewPad * 2;
        PreviewHeight = spanY * scale + PreviewPad * 2;

        Point Map(double x, double y) =>
            new((x - minX) * scale + PreviewPad, (maxY - y) * scale + PreviewPad);

        var cut = new StreamGeometry();
        var rapid = new StreamGeometry();
        using (var c = cut.Open())
        using (var r = rapid.Open())
        {
            foreach (var s in Document.Segments)
            {
                var g = s.Rapid ? r : c;
                var p0 = Map(s.X0, s.Y0);
                var p1 = Map(s.X1, s.Y1);
                g.BeginFigure(p0, false, false);
                g.LineTo(p1, true, false);
            }
        }
        cut.Freeze();
        rapid.Freeze();
        ToolpathGeometry = cut;
        RapidGeometry = rapid;
        RebuildSelectionGeometry();
        RebuildBackgroundGeometry();
        if (_hasMarker)
        {
            MarkerCanvasX = (_markerWorldX - _mapMinX) * _mapScale + PreviewPad;
            MarkerCanvasY = (_mapMaxY - _markerWorldY) * _mapScale + PreviewPad;
            MarkerVisibility = Visibility.Visible;
        }
        if (!IsSimulating && IsConnected)
            UpdateLiveToolMarker(CurrentWorkPos.X, CurrentWorkPos.Y);
        RebuildOverlays();
    }

    private void AfterTransform()
    {
        GcodeText = string.Join(Environment.NewLine, Document.Lines);
        _streamer.Load(Document);
        _simulator.Load(Document);
        BuildPreview();
        ShowPlaceholder = Document.Segments.Count == 0;
        DimensionText = Document.Segments.Count == 0
            ? "—"
            : $"X:{Document.MinX:0.###}…{Document.MaxX:0.###}  Y:{Document.MinY:0.###}…{Document.MaxY:0.###}";
    }

    [RelayCommand]
    private void TransformMirrorX()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.MirrorX(Document);
        AfterTransform();
        StatusBanner = "mirrored X";
    }

    [RelayCommand]
    private void TransformMirrorY()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.MirrorY(Document);
        AfterTransform();
        StatusBanner = "mirrored Y";
    }

    [RelayCommand]
    private void ApplyRotateTransform()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.Rotate(Document, TransformRotate, aroundBboxCenter: !UseAbsoluteOrigin);
        AfterTransform();
        StatusBanner = $"rotated {TransformRotate}°";
    }

    [RelayCommand]
    private void ApplyScaleTransform()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.Scale(Document, TransformScale, aroundBboxCenter: !UseAbsoluteOrigin);
        AfterTransform();
        StatusBanner = $"scaled ×{TransformScale}";
    }

    [RelayCommand]
    private void ApplyTranslateTransform()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.Translate(Document, TransformDx, TransformDy);
        AfterTransform();
        StatusBanner = $"translated {TransformDx},{TransformDy}";
    }

    [RelayCommand]
    private void TransformOriginCenter()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.SetOriginToCenter(Document);
        AfterTransform();
        StatusBanner = "origin → center";
    }

    [RelayCommand]
    private void TransformOriginMin()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.SetOriginToMinXY(Document);
        AfterTransform();
        StatusBanner = "origin → min XY";
    }

    [RelayCommand]
    private void TransformReverse()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.Reverse(Document);
        AfterTransform();
        StatusBanner = "paths reversed";
    }

    [RelayCommand]
    private void TransformRotate90Cw()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.Rotate(Document, -90, aroundBboxCenter: true);
        AfterTransform();
        StatusBanner = "rotated 90° CW";
    }

    [RelayCommand]
    private void TransformRotate90Ccw()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.Rotate(Document, 90, aroundBboxCenter: true);
        AfterTransform();
        StatusBanner = "rotated 90° CCW";
    }

    [RelayCommand]
    private void TransformRotate180()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.Rotate(Document, 180, aroundBboxCenter: true);
        AfterTransform();
        StatusBanner = "rotated 180°";
    }

    [RelayCommand]
    private void TransformScaleToWidth()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.ScaleToWidth(Document, TransformTargetWidth);
        AfterTransform();
        StatusBanner = $"scaled to width {TransformTargetWidth}";
    }

    [RelayCommand]
    private void TransformScaleToHeight()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.ScaleToHeight(Document, TransformTargetHeight);
        AfterTransform();
        StatusBanner = $"scaled to height {TransformTargetHeight}";
    }

    [RelayCommand]
    private void TransformRemoveZ()
    {
        if (Document.Lines.Count == 0) return;
        PushUndo();
        GCodeTransformService.RemoveZMoves(Document);
        AfterTransform();
        StatusBanner = "Z moves removed";
    }

    [RelayCommand]
    private void TransformArcsToLines()
    {
        if (Document.Lines.Count == 0) return;
        PushUndo();
        GCodeTransformService.ReplaceArcsWithLines(Document);
        AfterTransform();
        StatusBanner = "G2/G3 → G1";
    }

    [RelayCommand]
    private void TransformToPolar()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.ConvertToPolar(Document);
        AfterTransform();
        StatusBanner = "converted to polar";
    }

    [RelayCommand]
    private void TransformZToSpindle()
    {
        if (Document.Lines.Count == 0) return;
        PushUndo();
        GCodeTransformService.ConvertZToSpindle(Document);
        AfterTransform();
        StatusBanner = "Z → S spindle";
    }

    [RelayCommand]
    private void StreamStartFromLine()
    {
        ApplyEditor();
        if (!TryPromptInt("Start streaming at line number (1-based):", "Stream from line", 1, out var line) || line < 1)
        {
            StatusBanner = "stream from line cancelled";
            return;
        }
        var all = Document.Lines.ToList();
        if (line > all.Count) { StatusBanner = "line beyond end"; return; }
        var sliced = string.Join(Environment.NewLine, all.Skip(line - 1));
        var tmp = GCodeParser.Parse(sliced, Document.FilePath);
        _streamer.Load(tmp);
        ShowPlaceholder = false;
        _streamer.Start(false);
        if (!IsSimulating)
            UpdateLiveToolMarker(CurrentWorkPos.X, CurrentWorkPos.Y);
        StatusBanner = $"streaming from line {line}…";
    }

    private static bool TryPromptInt(string message, string title, int defaultValue, out int value)
    {
        value = defaultValue;
        var w = new Window
        {
            Title = title,
            Width = 360,
            Height = 160,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow,
            ResizeMode = ResizeMode.NoResize
        };
        var box = new System.Windows.Controls.TextBox { Text = defaultValue.ToString(), Margin = new Thickness(12, 8, 12, 8) };
        var ok = new System.Windows.Controls.Button { Content = "OK", Width = 80, IsDefault = true, Margin = new Thickness(0, 0, 8, 0) };
        var cancel = new System.Windows.Controls.Button { Content = "Cancel", Width = 80, IsCancel = true };
        bool accepted = false;
        ok.Click += (_, _) => { accepted = true; w.DialogResult = true; w.Close(); };
        var buttons = new System.Windows.Controls.StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(12) };
        buttons.Children.Add(ok); buttons.Children.Add(cancel);
        var panel = new System.Windows.Controls.StackPanel();
        panel.Children.Add(new System.Windows.Controls.TextBlock { Text = message, Margin = new Thickness(12, 12, 12, 0), TextWrapping = TextWrapping.Wrap });
        panel.Children.Add(box);
        panel.Children.Add(buttons);
        w.Content = panel;
        w.ShowDialog();
        return accepted && int.TryParse(box.Text, out value);
    }

    [RelayCommand]
    private void StreamStart()
    {
        ApplyEditor();
        ShowPlaceholder = Document.Segments.Count == 0;
        _streamer.Start(false);
        // Show tool tip immediately from last known work position (status polls continue to update).
        if (!IsSimulating)
            UpdateLiveToolMarker(CurrentWorkPos.X, CurrentWorkPos.Y);
        StatusBanner = "streaming…";
    }
    [RelayCommand] private void StreamCheck() { ApplyEditor(); _streamer.Start(true); StatusBanner = "check mode…"; }
    [RelayCommand] private void StreamPause() => _streamer.Pause();
    [RelayCommand] private void StreamResume() => _streamer.Resume();
    [RelayCommand] private void StreamStop() { _streamer.Stop(); StatusBanner = "stopped"; }

    [RelayCommand]
    private void SimStart()
    {
        ApplyEditor();
        _simulator.Load(Document);
        IsSimulating = true;
        _simulator.Start();
        StatusBanner = "simulating…";
    }

    [RelayCommand] private void SimPause() => _simulator.Pause();
    [RelayCommand] private void SimResume() => _simulator.Resume();
    [RelayCommand]
    private void SimStop()
    {
        _simulator.Stop();
        IsSimulating = false;
        SimMarkerVisibility = Visibility.Collapsed;
        StatusBanner = "simulation stopped";
    }

    [RelayCommand]
    private void SimFaster()
    {
        _simulator.SpeedUnitsPerSecond = Math.Min(400, _simulator.SpeedUnitsPerSecond * 2);
        StatusBanner = $"sim speed {_simulator.SpeedUnitsPerSecond:0}/s";
    }

    [RelayCommand]
    private void SimSlower()
    {
        _simulator.SpeedUnitsPerSecond = Math.Max(5, _simulator.SpeedUnitsPerSecond / 2);
        StatusBanner = $"sim speed {_simulator.SpeedUnitsPerSecond:0}/s";
    }

    [RelayCommand] private void FeedHold() => _serial.FeedHold();
    [RelayCommand] private void CycleStart() => _serial.CycleStart();
    [RelayCommand] private void SoftReset() => _serial.SoftReset();
    [RelayCommand] private void KillAlarm() => _serial.KillAlarm();
    [RelayCommand] private void Door() => _serial.SendRealtime(0x84);
    [RelayCommand] private void HomeMachine() => _serial.Home();

    [RelayCommand] private void ZeroX() => _serial.SendLine("G10 L20 P0 X0");
    [RelayCommand] private void ZeroY() => _serial.SendLine("G10 L20 P0 Y0");
    [RelayCommand] private void ZeroZ() => _serial.SendLine("G10 L20 P0 Z0");
    [RelayCommand] private void ZeroXy() => _serial.SendLine("G10 L20 P0 X0 Y0");
    [RelayCommand] private void ZeroXyz() => _serial.SendLine("G10 L20 P0 X0 Y0 Z0");

    /// <summary>Jog axes. Parameter examples: "X+", "Y-", "X+Y+", "X-Y-", "Z+".</summary>
    [RelayCommand]
    private void Jog(string axisDir)
    {
        if (string.IsNullOrWhiteSpace(axisDir)) return;
        var moves = new List<string>();
        for (int i = 0; i < axisDir.Length - 1; i++)
        {
            char a = char.ToUpperInvariant(axisDir[i]);
            char s = axisDir[i + 1];
            if ((a is 'X' or 'Y' or 'Z' or 'A' or 'B' or 'C') && (s is '+' or '-'))
            {
                var d = JogStep * (s == '-' ? -1 : 1);
                moves.Add($"{a}{d.ToString(CultureInfo.InvariantCulture)}");
                i++; // skip sign
            }
        }
        if (moves.Count == 0) return;
        _serial.SendLine($"$J=G91 G21 {string.Join(" ", moves)} F{JogFeed.ToString(CultureInfo.InvariantCulture)}");
    }

    [RelayCommand] private void JogCancel() => _serial.SendRealtime(0x85);

    [RelayCommand] private void OvFeedPlus() => _serial.SendRealtime(0x91);
    [RelayCommand] private void OvFeedMinus() => _serial.SendRealtime(0x92);
    [RelayCommand] private void OvFeedReset() => _serial.SendRealtime(0x90);
    [RelayCommand] private void OvRapid25() => _serial.SendRealtime(0x97);
    [RelayCommand] private void OvRapid50() => _serial.SendRealtime(0x96);
    [RelayCommand] private void OvRapid100() => _serial.SendRealtime(0x95);
    [RelayCommand] private void OvSpindlePlus() => _serial.SendRealtime(0x9A);
    [RelayCommand] private void OvSpindleMinus() => _serial.SendRealtime(0x9B);
    [RelayCommand] private void OvSpindleReset() => _serial.SendRealtime(0x99);

    [RelayCommand] private void OvFeedPlusFine() => _serial.SendRealtime(0x93); // +1%
    [RelayCommand] private void OvFeedMinusFine() => _serial.SendRealtime(0x94); // -1%
    [RelayCommand] private void OvSpindlePlusFine() => _serial.SendRealtime(0x9C);
    [RelayCommand] private void OvSpindleMinusFine() => _serial.SendRealtime(0x9D);

    [RelayCommand] private void CoolantMist() => _serial.SendLine("M7");
    [RelayCommand] private void CoolantFlood() => _serial.SendLine("M8");
    [RelayCommand] private void CoolantOff() => _serial.SendLine("M9");
    [RelayCommand] private void SpindleCw() => _serial.SendLine($"M3 S{LaserPower}");
    [RelayCommand] private void SpindleStop() => _serial.SendLine("M5");

    [RelayCommand]
    private void ExportSettings()
    {
        var dlg = new SaveFileDialog { Filter = "JSON settings|*.json", FileName = "grbl-plotter-wpf-settings.json" };
        if (dlg.ShowDialog() != true) return;
        _settings.JogStep = JogStep;
        _settings.JogFeed = JogFeed;
        _settings.Save();
        File.Copy(AppSettings.SettingsPath, dlg.FileName, overwrite: true);
        StatusBanner = "settings exported";
    }

    [RelayCommand]
    private void ImportSettings()
    {
        var dlg = new OpenFileDialog { Filter = "JSON settings|*.json|All|*.*" };
        if (dlg.ShowDialog() != true) return;
        try
        {
            File.Copy(dlg.FileName, AppSettings.SettingsPath, overwrite: true);
            StatusBanner = "settings imported — restart app to apply fully";
            MessageBox.Show("Settings imported. Some values apply after restart.", "Import settings");
        }
        catch (Exception ex)
        {
            StatusBanner = "import failed: " + ex.Message;
        }
    }

    [RelayCommand]
    private void SendManual(string? cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd)) return;
        _serial.SendLine(cmd);
    }

    [RelayCommand]
    private void RunCustom(CustomButtonItem? item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Code)) return;
        foreach (var line in item.Code.Replace("\r\n", "\n").Split('\n'))
        {
            var t = line.Trim();
            if (t.Length > 0) _serial.SendLine(t);
        }
    }

    [RelayCommand]
    private void LaserOn()
    {
        _serial.SendLine($"M3 S{LaserPower}");
        if (AirAssist) _serial.SendLine("M8");
    }

    [RelayCommand] private void LaserOff() { _serial.SendLine("M5"); _serial.SendLine("M9"); }

    [RelayCommand]
    private void ApplyLaserDefaults()
    {
        _settings.Devices.Laser.Power = LaserPower;
        _settings.Devices.Laser.Speed = LaserSpeed;
        _settings.Devices.Laser.Passes = LaserPasses;
        _settings.Devices.Laser.AirAssist = AirAssist;
        _settings.Save();
        StatusBanner = $"Laser defaults saved: S{LaserPower} F{LaserSpeed} passes={LaserPasses}";
    }

    [RelayCommand]
    private void ApplyPlotterDefaults()
    {
        _settings.Devices.Plotter.ZUp = PlotterZUp;
        _settings.Devices.Plotter.ZDown = PlotterZDown;
        _settings.Devices.Plotter.Speed = PlotterSpeed;
        _settings.Save();
        StatusBanner = $"Plotter defaults saved";
    }

    [RelayCommand]
    private void ApplyRouterDefaults()
    {
        _settings.Devices.Router.SpeedXy = RouterSpeedXy;
        _settings.Devices.Router.SpeedZ = RouterSpeedZ;
        _settings.Devices.Router.Depth = RouterDepth;
        _settings.Save();
        StatusBanner = $"Router defaults saved";
    }

    [RelayCommand]
    private void SetOriginCorner(string corner)
    {
        if (Document.Segments.Count == 0) { StatusBanner = "no graphic"; return; }
        PushUndo();
        double cx = (Document.MinX + Document.MaxX) / 2;
        double cy = (Document.MinY + Document.MaxY) / 2;
        double dx = 0, dy = 0;
        switch (corner)
        {
            case "TopLeft": dx = -Document.MinX; dy = -Document.MaxY; break;
            case "TopCenter": dx = -cx; dy = -Document.MaxY; break;
            case "TopRight": dx = -Document.MaxX; dy = -Document.MaxY; break;
            case "MidLeft": dx = -Document.MinX; dy = -cy; break;
            case "Center": dx = -cx; dy = -cy; break;
            case "MidRight": dx = -Document.MaxX; dy = -cy; break;
            case "BotLeft": dx = -Document.MinX; dy = -Document.MinY; break;
            case "BotCenter": dx = -cx; dy = -Document.MinY; break;
            case "BotRight": dx = -Document.MaxX; dy = -Document.MinY; break;
        }
        GCodeTransformService.Translate(Document, dx, dy);
        AfterTransform();
        StatusBanner = $"G-Code origin: {corner}";
    }

    [RelayCommand]
    private void SelectCoord(string code)
    {
        CoordSystem = code;
        _serial.SendLine(code);
    }

    private bool HasSelection() => _selectedIndices.Count > 0;

    [RelayCommand]
    private void PathDeleteSelected()
    {
        if (!HasSelection()) { StatusBanner = "no selection"; return; }
        PushUndo();
        PathEditService.DeleteIndices(Document, _selectedIndices.ToList());
        _selectedIndices.Clear();
        AfterTransform();
        StatusBanner = "deleted selected path";
    }

    [RelayCommand]
    private void PathDuplicateSelected()
    {
        if (!HasSelection()) { StatusBanner = "no selection"; return; }
        PushUndo();
        PathEditService.DuplicateIndices(Document, _selectedIndices.ToList());
        AfterTransform();
        StatusBanner = "duplicated selected path";
    }

    [RelayCommand]
    private void PathCropSelected()
    {
        if (!HasSelection()) { StatusBanner = "no selection"; return; }
        PushUndo();
        PathEditService.CropToIndices(Document, _selectedIndices.ToList());
        _selectedIndices.Clear();
        AfterTransform();
        StatusBanner = "cropped to selection";
    }

    [RelayCommand]
    private void PathReverseSelected()
    {
        if (!HasSelection()) { StatusBanner = "no selection"; return; }
        PushUndo();
        PathEditService.ReverseIndices(Document, _selectedIndices.ToList());
        AfterTransform();
        StatusBanner = "reversed selected path";
    }

    [RelayCommand]
    private void PathRotateSelected()
    {
        if (!HasSelection()) { StatusBanner = "no selection"; return; }
        PushUndo();
        PathEditService.RotateIndices(Document, _selectedIndices.ToList(), 90);
        AfterTransform();
        StatusBanner = "rotated selected path 90°";
    }

    [RelayCommand]
    private void MarkerMoveG0()
    {
        if (!_hasMarker) { StatusBanner = "no marker — right-click canvas"; return; }
        _serial.SendLine(FormattableString.Invariant($"G90 G0 X{_markerWorldX:0.###} Y{_markerWorldY:0.###}"));
        StatusBanner = "G0 to marker";
    }

    [RelayCommand]
    private void MarkerZeroXy()
    {
        if (!_hasMarker) { StatusBanner = "no marker — right-click canvas"; return; }
        _serial.SendLine(FormattableString.Invariant($"G90 G0 X{_markerWorldX:0.###} Y{_markerWorldY:0.###}"));
        _serial.SendLine("G10 L20 P0 X0 Y0");
        StatusBanner = "moved to marker + G10 XY zero";
    }

    [RelayCommand]
    private void SetGCodeAsBackground()
    {
        _backgroundSegments = Document.Segments.Select(s => new GCodeSegment
        {
            Rapid = s.Rapid, X0 = s.X0, Y0 = s.Y0, X1 = s.X1, Y1 = s.Y1
        }).ToList();
        RebuildBackgroundGeometry();
        StatusBanner = "G-Code set as background";
    }

    [RelayCommand]
    private void ClearBackground()
    {
        _backgroundSegments = null;
        RebuildBackgroundGeometry();
        StatusBanner = "background cleared";
    }

    [RelayCommand]
    private void SendEditorLine(string? lineText)
    {
        var line = (lineText ?? "").Split('\n').FirstOrDefault()?.Trim() ?? "";
        if (line.Length == 0) { StatusBanner = "no line to send"; return; }
        if (line.StartsWith(';') || line.StartsWith('(')) { StatusBanner = "comment line skipped"; return; }
        _serial.SendLine(line);
        StatusBanner = "sent: " + line;
    }

    [RelayCommand]
    private void FoldEditorComments()
    {
        // Collapse consecutive comment-only lines into a single "; … (N comments)"
        var lines = GcodeText.Replace("\r\n", "\n").Split('\n').ToList();
        var outLines = new List<string>();
        int run = 0;
        void Flush()
        {
            if (run == 1) outLines.Add("; (1 comment)");
            else if (run > 1) outLines.Add($"; … ({run} comments folded)");
            run = 0;
        }
        foreach (var l in lines)
        {
            var t = l.TrimStart();
            if (t.StartsWith(';') || t.StartsWith('(')) run++;
            else { Flush(); outLines.Add(l); }
        }
        Flush();
        PushUndo();
        GcodeText = string.Join(Environment.NewLine, outLines);
        StatusBanner = "folded comment blocks";
    }

    [RelayCommand]
    private void SortEditorByLineLength()
    {
        PushUndo();
        var lines = GcodeText.Replace("\r\n", "\n").Split('\n')
            .Select((l, i) => (l, i))
            .OrderBy(x => x.l.Length).ThenBy(x => x.i)
            .Select(x => x.l);
        GcodeText = string.Join(Environment.NewLine, lines);
        StatusBanner = "sorted editor lines by length";
    }

    // ——— Phase 2: machine panels ———

    private void MoveAbs(double x, double y)
    {
        if (MoveUseG0)
            _serial.SendLine(FormattableString.Invariant($"G90 G0 X{x:0.###} Y{y:0.###}"));
        else
            _serial.SendLine(FormattableString.Invariant(
                $"$J=G90 G21 X{x:0.###} Y{y:0.###} F{JogFeed.ToString(CultureInfo.InvariantCulture)}"));
    }

    [RelayCommand]
    private void MoveToGraphicCorner(string corner)
    {
        if (Document.Segments.Count == 0) { StatusBanner = "no graphic"; return; }
        double minX = Document.MinX, maxX = Document.MaxX, minY = Document.MinY, maxY = Document.MaxY;
        double cx = (minX + maxX) / 2, cy = (minY + maxY) / 2;
        double x = cx, y = cy;
        switch (corner)
        {
            case "TL": x = minX; y = maxY; break;
            case "TC": x = cx; y = maxY; break;
            case "TR": x = maxX; y = maxY; break;
            case "ML": x = minX; y = cy; break;
            case "C": x = cx; y = cy; break;
            case "MR": x = maxX; y = cy; break;
            case "BL": x = minX; y = minY; break;
            case "BC": x = cx; y = minY; break;
            case "BR": x = maxX; y = minY; break;
        }
        MoveAbs(x, y);
        StatusBanner = $"move to graphic {corner}";
    }

    [RelayCommand]
    private void FrameGraphic()
    {
        if (Document.Segments.Count == 0) { StatusBanner = "no graphic"; return; }
        double minX = Document.MinX, maxX = Document.MaxX, minY = Document.MinY, maxY = Document.MaxY;
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("; framing");
        sb.AppendLine(FormattableString.Invariant($"G90 G0 X{minX:0.###} Y{minY:0.###}"));
        sb.AppendLine(FormattableString.Invariant($"G1 X{maxX:0.###} Y{minY:0.###} F{JogFeed:0}"));
        sb.AppendLine(FormattableString.Invariant($"G1 X{maxX:0.###} Y{maxY:0.###}"));
        sb.AppendLine(FormattableString.Invariant($"G1 X{minX:0.###} Y{maxY:0.###}"));
        sb.AppendLine(FormattableString.Invariant($"G1 X{minX:0.###} Y{minY:0.###}"));
        foreach (var line in sb.ToString().Split('\n'))
        {
            var t = line.Trim();
            if (t.Length > 0 && !t.StartsWith(';')) _serial.SendLine(t);
        }
        StatusBanner = "framing graphic";
    }

    [RelayCommand]
    private void MoveAxisToZero(string axis)
    {
        axis = (axis ?? "XY").ToUpperInvariant();
        if (MoveUseG0)
        {
            if (axis == "XY") _serial.SendLine("G90 G0 X0 Y0");
            else _serial.SendLine($"G90 G0 {axis}0");
        }
        else
        {
            if (axis == "XY")
                _serial.SendLine($"$J=G90 G21 X0 Y0 F{JogFeed.ToString(CultureInfo.InvariantCulture)}");
            else
                _serial.SendLine($"$J=G90 G21 {axis}0 F{JogFeed.ToString(CultureInfo.InvariantCulture)}");
        }
        StatusBanner = $"move {axis} to zero";
    }

    [RelayCommand] private void DigitalOutOn(string n) => _serial.SendLine($"M64 P{n}");
    [RelayCommand] private void DigitalOutOff(string n) => _serial.SendLine($"M65 P{n}");

    [RelayCommand]
    private void PenUp() => _serial.SendLine(FormattableString.Invariant($"G90 G0 Z{PlotterZUp:0.###}"));
    [RelayCommand]
    private void PenDown() => _serial.SendLine(FormattableString.Invariant($"G90 G1 Z{PlotterZDown:0.###} F{PlotterSpeed:0}"));
    [RelayCommand]
    private void PenZero() => _serial.SendLine("G10 L20 P0 Z0");
    [RelayCommand]
    private void PenDot()
    {
        PenDown();
        _serial.SendLine("G4 P0.1");
        PenUp();
    }

    [RelayCommand]
    private void LoadToolList()
    {
        var dlg = new OpenFileDialog { Filter = "Tool CSV|*.csv;*.txt|All|*.*" };
        if (dlg.ShowDialog() != true) return;
        try
        {
            Tools.Clear();
            foreach (var t in ToolListService.LoadCsv(dlg.FileName)) Tools.Add(t);
            StatusBanner = $"loaded {Tools.Count} tools";
        }
        catch (Exception ex) { StatusBanner = "tool load failed: " + ex.Message; }
    }

    [RelayCommand]
    private void SaveToolList()
    {
        var dlg = new SaveFileDialog { Filter = "Tool CSV|*.csv", FileName = "tools.csv" };
        if (dlg.ShowDialog() != true) return;
        ToolListService.SaveCsv(dlg.FileName, Tools);
        StatusBanner = "tools saved";
    }

    [RelayCommand]
    private void GroupToolsByColor()
    {
        var sorted = ToolListService.GroupByColor(Tools);
        Tools.Clear();
        foreach (var t in sorted) Tools.Add(t);
        StatusBanner = "tools grouped by color";
    }

    [RelayCommand]
    private void ApplySelectedTool()
    {
        if (SelectedTool == null) { StatusBanner = "select a tool"; return; }
        _serial.SendLine($"T{SelectedTool.Number}");
        _serial.SendLine("M6");
        StatusBanner = $"tool {SelectedTool.Number} {SelectedTool.Name}";
    }

    // ——— Phase 3: deeper transforms ———

    [RelayCommand]
    private void TransformScaleXyIndep()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.Scale(Document, TransformScaleX, TransformScaleY, aroundBboxCenter: !UseAbsoluteOrigin);
        AfterTransform();
        StatusBanner = $"scaled X×{TransformScaleX} Y×{TransformScaleY}";
    }

    [RelayCommand]
    private void TransformScaleToRotary()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.ScaleAxisToRotaryDegrees(Document, RotaryDiameter, useX: true);
        AfterTransform();
        StatusBanner = $"scaled X to rotary ° (Ø{RotaryDiameter})";
    }

    [RelayCommand]
    private void TransformRadiusComp()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.RadiusCompensation(Document, RadiusComp);
        RememberLastTransform("radius comp", d => GCodeTransformService.RadiusCompensation(d, RadiusComp));
        AfterTransform();
        StatusBanner = $"radius compensation {RadiusComp}";
    }

    [RelayCommand]
    private void ApplyHatch()
    {
        if (Document.Segments.Count == 0) { StatusBanner = "no graphic for hatch"; return; }
        PushUndo();
        Document = HatchService.HatchDocument(Document,
            spacing: _settings.ImportHatchSpacing > 0 ? _settings.ImportHatchSpacing : 1.0,
            angleDeg: _settings.ImportHatchAngle);
        GcodeText = string.Join(Environment.NewLine, Document.Lines);
        _streamer.Load(Document);
        _simulator.Load(Document);
        BuildPreview();
        ShowPlaceholder = false;
        StatusBanner = "hatch fill added";
    }

    // override rotate/scale/mirror to honor UseAbsoluteOrigin
    partial void OnUseAbsoluteOriginChanged(bool value) =>
        StatusBanner = value ? "transform pivot: origin 0;0" : "transform pivot: bbox center";

    private void OnLog(string line)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            LogLines.Add(line);
            while (LogLines.Count > 800) LogLines.RemoveAt(0);
        });
    }

    private void OnStatus(GrblStatusSnapshot s)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MachineStateText = s.State.ToString();
            WorkX = $"{s.Work.X:00000.000}";
            WorkY = $"{s.Work.Y:00000.000}";
            WorkZ = $"{s.Work.Z:00000.000}";
            MachineX = $"{s.Machine.X:00000.000}";
            MachineY = $"{s.Machine.Y:00000.000}";
            MachineZ = $"{s.Machine.Z:00000.000}";
            CurrentWorkPos.X = s.Work.X;
            CurrentWorkPos.Y = s.Work.Y;
            CurrentWorkPos.Z = s.Work.Z;
            UpdateAbcFromStatus(s);
            OvFeed = s.OvFeed;
            OvRapid = s.OvRapid;
            OvSpindle = s.OvSpindle;
            FeedRate = s.Feed;
            SpindleRate = s.Spindle;
            // Run/jog: drive the same preview marker Simulation uses (was sim-only before).
            if (!IsSimulating)
                UpdateLiveToolMarker(s.Work.X, s.Work.Y);
        });
    }

    /// <summary>Map work coordinates onto the 2D preview tool tip (visible during Run &amp; idle while connected).</summary>
    private void UpdateLiveToolMarker(double workX, double workY)
    {
        if (!IsConnected || Document.Segments.Count == 0 || _mapScale <= 1e-12)
        {
            if (!IsSimulating) SimMarkerVisibility = Visibility.Collapsed;
            return;
        }
        SimX = (workX - _mapMinX) * _mapScale + PreviewPad;
        SimY = (_mapMaxY - workY) * _mapScale + PreviewPad;
        SimMarkerVisibility = Visibility.Visible;
    }
}

public sealed class CustomButtonItem
{
    public string Label { get; set; }
    public string Code { get; set; }
    public CustomButtonItem() { Label = ""; Code = ""; }
    public CustomButtonItem(string label, string code) { Label = label; Code = code; }
}
