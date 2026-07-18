using System.Windows;
using System.Windows.Threading;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services;

/// <summary>Simulates toolpath progress along a <see cref="GCodeDocument"/>'s segments using a UI timer,
/// independent of any real controller connection (used for preview / dry-run animation).</summary>
public sealed class PathSimulator
{
    private readonly DispatcherTimer _timer;
    private List<(GCodeSegment Seg, double Length, double CumulativeStart)> _plan = new();
    private double _totalLength;
    private double _travelled;
    private bool _running;
    private bool _paused;

    /// <summary>Simulated feed rate in drawing units per second.</summary>
    public double SpeedUnitsPerSecond { get; set; } = 40;

    public event Action<Point>? PositionChanged;
    public event Action<double>? ProgressChanged;
    public event Action? Completed;

    public bool IsRunning => _running;
    public bool IsPaused => _paused;
    public Point CurrentPosition { get; private set; }

    public PathSimulator()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };
        _timer.Tick += (_, _) => Tick();
    }

    public void Load(GCodeDocument doc)
    {
        Stop();
        _plan = new List<(GCodeSegment, double, double)>();
        double cumulative = 0;
        foreach (var seg in doc.Segments)
        {
            var len = Math.Sqrt(Math.Pow(seg.X1 - seg.X0, 2) + Math.Pow(seg.Y1 - seg.Y0, 2));
            _plan.Add((seg, len, cumulative));
            cumulative += len;
        }
        _totalLength = cumulative;
        _travelled = 0;
        CurrentPosition = doc.Segments.Count > 0 ? new Point(doc.Segments[0].X0, doc.Segments[0].Y0) : new Point();
    }

    public void Start()
    {
        if (_plan.Count == 0) return;
        _running = true;
        _paused = false;
        _timer.Start();
    }

    public void Pause()
    {
        _paused = true;
        _timer.Stop();
    }

    public void Resume()
    {
        if (!_running) return;
        _paused = false;
        _timer.Start();
    }

    public void Stop()
    {
        _running = false;
        _paused = false;
        _timer.Stop();
        _travelled = 0;
    }

    private void Tick()
    {
        if (!_running || _paused) return;
        _travelled += SpeedUnitsPerSecond * (_timer.Interval.TotalSeconds);
        if (_travelled >= _totalLength)
        {
            _travelled = _totalLength;
            var last = _plan.Count > 0 ? _plan[^1].Seg : null;
            if (last != null) CurrentPosition = new Point(last.X1, last.Y1);
            PositionChanged?.Invoke(CurrentPosition);
            ProgressChanged?.Invoke(100);
            Stop();
            Completed?.Invoke();
            return;
        }

        var entry = FindSegmentAt(_travelled);
        if (entry != null)
        {
            var (seg, len, start) = entry.Value;
            var localT = len <= 0 ? 1 : (_travelled - start) / len;
            var x = seg.X0 + (seg.X1 - seg.X0) * localT;
            var y = seg.Y0 + (seg.Y1 - seg.Y0) * localT;
            CurrentPosition = new Point(x, y);
            PositionChanged?.Invoke(CurrentPosition);
        }
        ProgressChanged?.Invoke(_totalLength <= 0 ? 0 : 100.0 * _travelled / _totalLength);
    }

    private (GCodeSegment Seg, double Length, double CumulativeStart)? FindSegmentAt(double distance)
    {
        foreach (var entry in _plan)
        {
            if (distance <= entry.CumulativeStart + entry.Length) return entry;
        }
        return _plan.Count > 0 ? _plan[^1] : null;
    }
}
