using System.Windows.Threading;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services;

/// <summary>GRBL ok-handshake streamer: one (or a few) lines ahead, never leaves the machine in Hold on Start.</summary>
public sealed class GCodeStreamer
{
    private readonly GrblSerialService _serial;
    private readonly Dispatcher _dispatcher;
    private IReadOnlyList<string> _lines = Array.Empty<string>();
    private int _index;
    private int _pendingOk;
    private bool _running;
    private bool _paused;
    private bool _checkMode;

    public event Action? ProgressChanged;
    public event Action? Completed;

    public int CurrentLine => _index;
    public int TotalLines => _lines.Count;
    public bool IsRunning => _running;
    public bool IsPaused => _paused;
    public double Progress => TotalLines == 0 ? 0 : 100.0 * Math.Min(_index, TotalLines) / TotalLines;

    public GCodeStreamer(GrblSerialService serial)
    {
        _serial = serial;
        _dispatcher = Dispatcher.CurrentDispatcher;
        _serial.OkReceived += OnOk;
    }

    public void Load(GCodeDocument doc)
    {
        // Reset stream state only — do NOT FeedHold here (that was leaving GRBL in Hold
        // so Start queued a burst then stalled until the user clicked Resume / '~').
        _running = false;
        _paused = false;
        _pendingOk = 0;
        _checkMode = false;
        _lines = doc.Lines
            .Select(l => l.Split(';')[0].Trim())
            .Where(l => l.Length > 0 && !l.StartsWith('('))
            .ToList();
        _index = 0;
        ProgressChanged?.Invoke();
    }

    public void Start(bool checkMode = false)
    {
        if (_lines.Count == 0) return;
        _checkMode = checkMode;
        _running = true;
        _paused = false;
        _pendingOk = 0;
        _index = 0;
        _serial.ClearQueue();
        // Clear any Hold left over from a previous Stop / accidental '!'
        _serial.CycleStart();
        if (_checkMode) _serial.SendLine("$C");
        FillWindow();
        ProgressChanged?.Invoke();
    }

    public void Pause()
    {
        if (!_running) return;
        _paused = true;
        _serial.FeedHold();
        ProgressChanged?.Invoke();
    }

    public void Resume()
    {
        if (!_running) return;
        _paused = false;
        _serial.CycleStart();
        FillWindow();
        ProgressChanged?.Invoke();
    }

    public void Stop()
    {
        bool wasRunning = _running;
        _running = false;
        _paused = false;
        _pendingOk = 0;
        _serial.ClearQueue();
        if (wasRunning)
            _serial.FeedHold();
        if (_checkMode)
        {
            _serial.SendLine("$C");
            _checkMode = false;
        }
        ProgressChanged?.Invoke();
    }

    private void OnOk()
    {
        // SerialPort.DataReceived is a worker thread — marshal before mutating stream state.
        if (_dispatcher.CheckAccess())
            HandleOk();
        else
            _dispatcher.BeginInvoke(HandleOk);
    }

    private void HandleOk()
    {
        if (_pendingOk > 0) _pendingOk--;
        if (!_running || _paused) return;
        FillWindow();
        if (_index >= _lines.Count && _pendingOk == 0)
            Finish();
    }

    /// <summary>Keep a small send window filled (serial layer also caps by planner slots).</summary>
    private void FillWindow()
    {
        if (!_running || _paused) return;
        const int window = 5;
        while (_pendingOk < window && _index < _lines.Count)
        {
            _serial.SendLine(_lines[_index]);
            _index++;
            _pendingOk++;
        }
        ProgressChanged?.Invoke();
        if (_index >= _lines.Count && _pendingOk == 0)
            Finish();
    }

    private void Finish()
    {
        if (!_running) return;
        _running = false;
        if (_checkMode)
        {
            _serial.SendLine("$C");
            _checkMode = false;
        }
        Completed?.Invoke();
        ProgressChanged?.Invoke();
    }
}
