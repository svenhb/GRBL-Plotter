using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GrblPlotter.Wpf.Models;
using GrblPlotter.Wpf.Services;
using GrblPlotter.Wpf.Services.Import;
using GrblPlotter.Wpf.Services.Preview;
using GrblPlotter.Wpf.Services.Transform;

namespace GrblPlotter.Wpf.ViewModels;

/// <summary>Wave A–D parity commands and overlay state (partial).</summary>
public partial class MainViewModel
{
    [ObservableProperty] private string _workA = "00000.000";
    [ObservableProperty] private string _workB = "00000.000";
    [ObservableProperty] private string _workC = "00000.000";
    [ObservableProperty] private string _machineA = "00000.000";
    [ObservableProperty] private string _machineB = "00000.000";
    [ObservableProperty] private string _machineC = "00000.000";
    [ObservableProperty] private Visibility _axisAVisibility = Visibility.Collapsed;
    [ObservableProperty] private Visibility _axisBVisibility = Visibility.Collapsed;
    [ObservableProperty] private Visibility _axisCVisibility = Visibility.Collapsed;

    [ObservableProperty] private bool _showRuler = true;
    [ObservableProperty] private bool _showInfoHud = true;
    [ObservableProperty] private bool _showMachineLimits;
    [ObservableProperty] private bool _showFixedMachineArea;
    [ObservableProperty] private bool _showToolTableOverlay;
    [ObservableProperty] private Geometry? _rulerGeometry;
    [ObservableProperty] private Geometry? _rulerGridGeometry;
    [ObservableProperty] private Geometry? _limitsGeometry;
    [ObservableProperty] private Geometry? _toolTableGeometry;
    [ObservableProperty] private Visibility _rulerVisibility = Visibility.Visible;
    [ObservableProperty] private Visibility _limitsVisibility = Visibility.Collapsed;
    [ObservableProperty] private Visibility _toolTableVisibility = Visibility.Collapsed;

    public ObservableCollection<RulerLabelItem> RulerLabels { get; } = new();
    [ObservableProperty] private string _infoHudText = "";
    [ObservableProperty] private Visibility _infoHudVisibility = Visibility.Visible;

    [ObservableProperty] private double _viewZoom = 1.0;
    [ObservableProperty] private double _viewPanX;
    [ObservableProperty] private double _viewPanY;

    private string? _lastTransformName;
    private Action<GCodeDocument>? _lastTransform;
    private string _lastLoadedPath = "";

    partial void InitParityFromSettings() => ReloadParityFromSettings();

    public void ReloadParityFromSettings()
    {
        ShowRuler = _settings.ShowRuler;
        ShowInfoHud = _settings.ShowInfoHud;
        ShowMachineLimits = _settings.ShowMachineLimits;
        ShowFixedMachineArea = _settings.ShowFixedMachineArea;
        ShowToolTableOverlay = _settings.ShowToolTableOverlay;
        _lastLoadedPath = _settings.LastLoadedPath ?? "";
        ApplyAxisVisibility();
        RebuildOverlays();
    }

    public void ApplyAxisVisibility()
    {
        int n = Math.Clamp(_settings.AxisCount, 3, 6);
        AxisAVisibility = n >= 4 ? Visibility.Visible : Visibility.Collapsed;
        AxisBVisibility = n >= 5 ? Visibility.Visible : Visibility.Collapsed;
        AxisCVisibility = n >= 6 ? Visibility.Visible : Visibility.Collapsed;
    }

    public void RefreshCustomButtonsFromSettings()
    {
        while (_settings.CustomButtons.Count < 16)
            _settings.CustomButtons.Add(new CustomButtonDto
            {
                Label = $"C{_settings.CustomButtons.Count + 1}",
                Code = ""
            });
        if (_settings.CustomButtons.Count > 16)
            _settings.CustomButtons.RemoveRange(16, _settings.CustomButtons.Count - 16);
        CustomButtons.Clear();
        for (int i = 0; i < 16; i++)
        {
            var dto = _settings.CustomButtons[i];
            CustomButtons.Add(new CustomButtonItem(
                string.IsNullOrWhiteSpace(dto.Label) ? $"C{i + 1}" : dto.Label,
                dto.Code ?? ""));
        }
    }

    public void RememberLastTransform(string name, Action<GCodeDocument> action)
    {
        _lastTransformName = name;
        _lastTransform = action;
    }

    private void TrackLoadedPath(string path)
    {
        _lastLoadedPath = path;
        _settings.LastLoadedPath = path;
        _settings.Save();
    }

    [RelayCommand]
    private void ReloadLastFile()
    {
        if (string.IsNullOrEmpty(_lastLoadedPath) || !File.Exists(_lastLoadedPath))
        {
            StatusBanner = "no last file to reload";
            return;
        }
        LoadPath(_lastLoadedPath);
        StatusBanner = "reloaded " + Path.GetFileName(_lastLoadedPath);
    }

    [RelayCommand]
    private void PasteFromClipboard()
    {
        try
        {
            if (Clipboard.ContainsFileDropList())
            {
                var files = Clipboard.GetFileDropList();
                if (files.Count > 0) { LoadPath(files[0]!); return; }
            }
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                if (text.Contains('G') || text.Contains('g') || text.Contains('<'))
                {
                    ApplyGeneratedGCode(text, "clipboard.nc");
                    return;
                }
            }
            if (Clipboard.ContainsImage())
            {
                var img = Clipboard.GetImage();
                if (img != null)
                {
                    var tmp = Path.Combine(Path.GetTempPath(), "grbl-clip.png");
                    using (var fs = File.Create(tmp))
                    {
                        var enc = new PngBitmapEncoder();
                        enc.Frames.Add(BitmapFrame.Create(img));
                        enc.Save(fs);
                    }
                    LoadPath(tmp);
                    return;
                }
            }
            StatusBanner = "clipboard has no usable graphic/G-code";
        }
        catch (Exception ex) { StatusBanner = "paste failed: " + ex.Message; }
    }

    [RelayCommand]
    private void ResetZoom()
    {
        ViewZoom = 1;
        ViewPanX = 0;
        ViewPanY = 0;
        StatusBanner = "zoom reset";
    }

    [RelayCommand]
    private void ShowPathProperties()
    {
        if (_selectedIndices.Count == 0) { StatusBanner = "no selection"; return; }
        double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue, len = 0;
        foreach (var i in _selectedIndices)
        {
            var s = Document.Segments[i];
            minX = Math.Min(minX, Math.Min(s.X0, s.X1));
            maxX = Math.Max(maxX, Math.Max(s.X0, s.X1));
            minY = Math.Min(minY, Math.Min(s.Y0, s.Y1));
            maxY = Math.Max(maxY, Math.Max(s.Y0, s.Y1));
            len += Math.Sqrt(Math.Pow(s.X1 - s.X0, 2) + Math.Pow(s.Y1 - s.Y0, 2));
        }
        MessageBox.Show(
            $"Segments: {_selectedIndices.Count}\n" +
            $"X: {minX:0.###} … {maxX:0.###}\nY: {minY:0.###} … {maxY:0.###}\n" +
            $"Size: {maxX - minX:0.###} × {maxY - minY:0.###}\nLength: {len:0.###}",
            "Path properties");
    }

    [RelayCommand]
    private void OffsetGraphicsMarkerToZero()
    {
        if (!_hasMarker) { StatusBanner = "no marker"; return; }
        PushUndo();
        GCodeTransformService.Translate(Document, -_markerWorldX, -_markerWorldY);
        RememberLastTransform("offset marker→0", d => GCodeTransformService.Translate(d, -_markerWorldX, -_markerWorldY));
        AfterTransform();
        StatusBanner = "offset graphics: marker → 0;0";
    }

    [RelayCommand]
    private void OffsetGraphicsMarkerToTool()
    {
        if (!_hasMarker) { StatusBanner = "no marker"; return; }
        PushUndo();
        double dx = CurrentWorkPos.X - _markerWorldX;
        double dy = CurrentWorkPos.Y - _markerWorldY;
        GCodeTransformService.Translate(Document, dx, dy);
        RememberLastTransform("offset marker→tool", d => GCodeTransformService.Translate(d, dx, dy));
        AfterTransform();
        StatusBanner = "offset graphics: marker → tool";
    }

    [RelayCommand]
    private void ApplyLastTransform()
    {
        if (_lastTransform == null) { StatusBanner = "no last transform"; return; }
        PushUndo();
        _lastTransform(Document);
        AfterTransform();
        StatusBanner = "applied last transform: " + (_lastTransformName ?? "");
    }

    [RelayCommand]
    private void CopyPreviewToClipboard()
    {
        try
        {
            var dpi = 96.0;
            var w = Math.Max(PreviewWidth, 1);
            var h = Math.Max(PreviewHeight, 1);
            var rtb = new RenderTargetBitmap((int)w, (int)h, dpi, dpi, PixelFormats.Pbgra32);
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0x1A, 0x1F, 0x26)), null, new Rect(0, 0, w, h));
                if (ToolpathGeometry != null)
                    dc.DrawGeometry(null, new Pen(new SolidColorBrush(Color.FromRgb(0x6F, 0xBF, 0x8A)), 1.2), ToolpathGeometry);
                if (RapidGeometry != null)
                    dc.DrawGeometry(null, new Pen(new SolidColorBrush(Color.FromRgb(0x4A, 0x90, 0xA4)), 0.8), RapidGeometry);
            }
            rtb.Render(dv);
            Clipboard.SetImage(rtb);
            StatusBanner = "preview copied to clipboard";
        }
        catch (Exception ex) { StatusBanner = "copy failed: " + ex.Message; }
    }

    [RelayCommand]
    private void MarkFirstPointOfSelection()
    {
        if (_selectedIndices.Count == 0) { StatusBanner = "no selection"; return; }
        var s = Document.Segments[_selectedIndices[0]];
        _markerWorldX = s.X0; _markerWorldY = s.Y0; _hasMarker = true;
        MarkerCanvasX = (s.X0 - _mapMinX) * _mapScale + PreviewPad;
        MarkerCanvasY = (_mapMaxY - s.Y0) * _mapScale + PreviewPad;
        MarkerVisibility = Visibility.Visible;
        StatusBanner = $"marked first point X{s.X0:0.###} Y{s.Y0:0.###}";
    }

    [RelayCommand] private void ZeroA() => _serial.SendLine("G10 L20 P0 A0");
    [RelayCommand] private void ZeroB() => _serial.SendLine("G10 L20 P0 B0");
    [RelayCommand] private void ZeroC() => _serial.SendLine("G10 L20 P0 C0");

    [RelayCommand]
    private void TransformMirrorRotary()
    {
        if (Document.Lines.Count == 0) return;
        PushUndo();
        GCodeTransformService.MirrorRotary(Document, _settings.RotaryAxisName);
        RememberLastTransform("mirror rotary", d => GCodeTransformService.MirrorRotary(d, _settings.RotaryAxisName));
        AfterTransform();
        StatusBanner = "mirrored rotary " + _settings.RotaryAxisName;
    }

    [RelayCommand]
    private void TransformScaleYToRotary()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.ScaleYToRotaryDegrees(Document, RotaryDiameter);
        RememberLastTransform("scale Y rotary", d => GCodeTransformService.ScaleYToRotaryDegrees(d, RotaryDiameter));
        AfterTransform();
        StatusBanner = $"scaled Y to rotary ° (Ø{RotaryDiameter})";
    }

    [RelayCommand]
    private void TransformRadiusCompFull()
    {
        if (Document.Segments.Count == 0) return;
        PushUndo();
        GCodeTransformService.RadiusCompensation(Document, RadiusComp);
        RememberLastTransform("radius comp", d => GCodeTransformService.RadiusCompensation(d, RadiusComp));
        AfterTransform();
        StatusBanner = $"radius compensation {RadiusComp}";
    }

    [RelayCommand]
    private void ApplyTangential()
    {
        if (Document.Segments.Count == 0) return;
        var doc = TangentialService.Apply(Document, _settings.RotaryAxisName, _settings.ImportTangentialAngle);
        ApplyDocument(doc);
        StatusBanner = "tangential axis applied";
    }

    [RelayCommand]
    private void FindReplaceEditor(string? args)
    {
        // args format: find|replace  (empty replace = find-only highlight via status count)
        if (string.IsNullOrEmpty(args)) return;
        var parts = args.Split('|', 2);
        var find = parts[0];
        var replace = parts.Length > 1 ? parts[1] : null;
        if (string.IsNullOrEmpty(find)) return;
        if (replace == null)
        {
            int n = 0, idx = 0;
            while ((idx = GcodeText.IndexOf(find, idx, StringComparison.OrdinalIgnoreCase)) >= 0)
            { n++; idx += find.Length; }
            StatusBanner = $"find '{find}': {n} match(es)";
            return;
        }
        PushUndo();
        GcodeText = GcodeText.Replace(find, replace, StringComparison.OrdinalIgnoreCase);
        StatusBanner = $"replaced '{find}' → '{replace}'";
    }

    [RelayCommand]
    private void SortBlocksBy(string key)
    {
        PushUndo();
        var lines = GcodeText.Replace("\r\n", "\n").Split('\n');
        GcodeText = XmlMarkerService.SortBy(lines, key);
        ApplyEditor();
        StatusBanner = "sorted blocks by " + key;
    }

    [RelayCommand]
    private void RemoveXmlTags(string mode)
    {
        PushUndo();
        var lines = GcodeText.Replace("\r\n", "\n").Split('\n');
        GcodeText = XmlMarkerService.RemoveXmlTags(lines, groupsOnly: mode == "group");
        ApplyEditor();
        StatusBanner = mode == "group" ? "removed Group tags" : "removed XML tags";
    }

    [RelayCommand]
    private void InsertPenUp()
    {
        var insert = FormattableString.Invariant($"G0 Z{_settings.Devices.Plotter.ZUp:0.###}");
        GcodeText += Environment.NewLine + insert;
        StatusBanner = "inserted PenUp";
    }

    [RelayCommand]
    private void InsertPenDown()
    {
        var insert = FormattableString.Invariant($"G1 Z{_settings.Devices.Plotter.ZDown:0.###} F{_settings.Devices.Plotter.Speed:0}");
        GcodeText += Environment.NewLine + insert;
        StatusBanner = "inserted PenDown";
    }

    [RelayCommand]
    private void CommentUnknownGCode()
    {
        PushUndo();
        var known = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "G0","G00","G1","G01","G2","G02","G3","G03","G4","G17","G18","G19","G20","G21","G28","G53","G54","G55","G56","G57","G58","G59","G90","G91","G92","G94","M0","M1","M2","M3","M4","M5","M6","M7","M8","M9","M30" };
        var sb = new StringBuilder();
        foreach (var raw in GcodeText.Replace("\r\n", "\n").Split('\n'))
        {
            var t = raw.Trim();
            if (t.Length == 0 || t.StartsWith(';') || t.StartsWith('(')) { sb.AppendLine(raw); continue; }
            var word = t.Split(' ', '\t')[0];
            if (word.Length >= 2 && (word[0] is 'G' or 'g' or 'M' or 'm') && !known.Contains(word) && !known.Contains(word.ToUpperInvariant()))
                sb.AppendLine("; ??? " + raw);
            else sb.AppendLine(raw);
        }
        GcodeText = sb.ToString();
        StatusBanner = "commented unknown G/M codes";
    }

    public void RebuildOverlays()
    {
        Point Map(double x, double y) =>
            new((x - _mapMinX) * _mapScale + PreviewPad, (_mapMaxY - y) * _mapScale + PreviewPad);

        double minX = ShowFixedMachineArea ? _settings.MachineMinX : Document.MinX;
        double minY = ShowFixedMachineArea ? _settings.MachineMinY : Document.MinY;
        double maxX = ShowFixedMachineArea ? _settings.MachineMaxX : Document.MaxX;
        double maxY = ShowFixedMachineArea ? _settings.MachineMaxY : Document.MaxY;
        if (Document.Segments.Count == 0 || (maxX - minX) < 1e-9 || (maxY - minY) < 1e-9)
        {
            minX = _settings.MachineMinX; maxX = _settings.MachineMaxX;
            minY = _settings.MachineMinY; maxY = _settings.MachineMaxY;
        }

        RulerLabels.Clear();
        if (ShowRuler)
        {
            var overlay = PreviewOverlayBuilder.BuildRuler(minX, minY, maxX, maxY, Map, _mapScale, rulerBandPx: 26);
            RulerGeometry = overlay.Ticks;
            RulerGridGeometry = overlay.Grid;
            foreach (var lb in overlay.Labels)
                RulerLabels.Add(new RulerLabelItem(lb.X, lb.Y, lb.Text));
            RulerVisibility = Visibility.Visible;
        }
        else
        {
            RulerGeometry = null;
            RulerGridGeometry = null;
            RulerVisibility = Visibility.Collapsed;
        }

        if (ShowMachineLimits || ShowFixedMachineArea)
        {
            LimitsGeometry = PreviewOverlayBuilder.BuildMachineLimits(
                _settings.MachineMinX, _settings.MachineMinY, _settings.MachineMaxX, _settings.MachineMaxY, Map);
            LimitsVisibility = Visibility.Visible;
        }
        else LimitsVisibility = Visibility.Collapsed;

        if (ShowToolTableOverlay && Tools.Count > 0)
        {
            ToolTableGeometry = PreviewOverlayBuilder.BuildToolMarkers(Tools.Select(t => (t.X, t.Y)), Map);
            ToolTableVisibility = Visibility.Visible;
        }
        else ToolTableVisibility = Visibility.Collapsed;

        InfoHudVisibility = ShowInfoHud ? Visibility.Visible : Visibility.Collapsed;
        InfoHudText = Document.Segments.Count == 0
            ? "no graphic"
            : $"{Document.FileName}  segs:{Document.Segments.Count}  {DimensionText}  zoom:{ViewZoom:0.##}";
    }

    partial void OnShowRulerChanged(bool value)
    {
        _settings.ShowRuler = value;
        _settings.Save();
        BuildPreview(); // padding changes with ruler band
    }
    partial void OnShowInfoHudChanged(bool value) { _settings.ShowInfoHud = value; _settings.Save(); RebuildOverlays(); }
    partial void OnShowMachineLimitsChanged(bool value) { _settings.ShowMachineLimits = value; _settings.Save(); RebuildOverlays(); }
    partial void OnShowFixedMachineAreaChanged(bool value) { _settings.ShowFixedMachineArea = value; _settings.Save(); RebuildOverlays(); }
    partial void OnShowToolTableOverlayChanged(bool value) { _settings.ShowToolTableOverlay = value; _settings.Save(); RebuildOverlays(); }

    public void UpdateAbcFromStatus(GrblStatusSnapshot s)
    {
        WorkA = $"{s.Work.A:00000.000}";
        WorkB = $"{s.Work.B:00000.000}";
        WorkC = $"{s.Work.C:00000.000}";
        MachineA = $"{s.Machine.A:00000.000}";
        MachineB = $"{s.Machine.B:00000.000}";
        MachineC = $"{s.Machine.C:00000.000}";
    }
}

public sealed class RulerLabelItem
{
    public double X { get; }
    public double Y { get; }
    public string Text { get; }
    public RulerLabelItem(double x, double y, string text) { X = x; Y = y; Text = text; }
}
