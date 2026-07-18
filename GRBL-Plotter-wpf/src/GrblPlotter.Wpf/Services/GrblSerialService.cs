using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text;
using System.Windows.Threading;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services;

public sealed class GrblSerialService : IDisposable
{
    private readonly object _sync = new();
    private SerialPort? _port;
    private readonly StringBuilder _rx = new();
    private readonly ConcurrentQueue<string> _sendQueue = new();
    private readonly DispatcherTimer _pollTimer;
    private readonly DispatcherTimer _sendTimer;
    private int _plannerSlots = 12;
    private int _okPending;

    public event Action<string>? LogReceived;
    public event Action<GrblStatusSnapshot>? StatusUpdated;
    public event Action<bool>? ConnectionChanged;
    public event Action? OkReceived;

    public bool IsConnected
    {
        get { lock (_sync) return _port?.IsOpen == true; }
    }

    public string PortName { get; private set; } = "";
    public int BaudRate { get; private set; } = 115200;
    public GrblStatusSnapshot LastStatus { get; } = new();

    public GrblSerialService()
    {
        _pollTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _pollTimer.Tick += (_, _) => { if (IsConnected) SendRealtime((byte)'?'); };
        _sendTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
        _sendTimer.Tick += (_, _) => PumpSendQueue();
    }

    public static string[] GetPortNames() => SerialPort.GetPortNames().OrderBy(x => x).ToArray();

    public void Connect(string portName, int baud)
    {
        Disconnect();
        var sp = new SerialPort(portName, baud)
        {
            NewLine = "\n",
            Encoding = Encoding.ASCII,
            ReadTimeout = 500,
            WriteTimeout = 500,
            DtrEnable = false,
            RtsEnable = false
        };
        sp.DataReceived += OnDataReceived;
        sp.ErrorReceived += (_, e) => AppendLog($"[PORT ERROR] {e.EventType}");
        sp.Open();
        lock (_sync) _port = sp;
        PortName = portName;
        BaudRate = baud;
        _okPending = 0;
        _pollTimer.Start();
        _sendTimer.Start();
        ConnectionChanged?.Invoke(true);
        AppendLog($"Opened {portName} @ {baud}");
        // soft wake
        SendLine("");
        SendLine("$I");
    }

    public void Disconnect()
    {
        _pollTimer.Stop();
        _sendTimer.Stop();
        while (_sendQueue.TryDequeue(out _)) { }
        SerialPort? sp;
        lock (_sync)
        {
            sp = _port;
            _port = null;
        }
        if (sp != null)
        {
            try
            {
                sp.DataReceived -= OnDataReceived;
                if (sp.IsOpen) sp.Close();
                sp.Dispose();
            }
            catch { /* ignore */ }
            AppendLog("Port closed");
        }
        ConnectionChanged?.Invoke(false);
    }

    public void SendLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line) && line != "")
        {
            SendRaw("\r\n");
            return;
        }
        var cleaned = line.Split(';')[0].Trim();
        if (cleaned.Length == 0) return;
        _sendQueue.Enqueue(cleaned);
    }

    public void SendRealtime(byte b)
    {
        try
        {
            lock (_sync)
            {
                if (_port?.IsOpen == true)
                    _port.BaseStream.WriteByte(b);
            }
        }
        catch (Exception ex) { AppendLog($"[TX realtime fail] {ex.Message}"); }
    }

    public void SoftReset() => SendRealtime(0x18);
    public void FeedHold() => SendRealtime((byte)'!');
    public void CycleStart() => SendRealtime((byte)'~');
    public void KillAlarm() => SendLine("$X");
    public void Home() => SendLine("$H");
    public void ClearQueue() { while (_sendQueue.TryDequeue(out _)) { } _okPending = 0; }

    private void PumpSendQueue()
    {
        if (!IsConnected) return;
        while (_okPending < _plannerSlots && _sendQueue.TryDequeue(out var line))
        {
            SendRaw(line + "\n");
            _okPending++;
            AppendLog($"> {line}");
        }
    }

    private void SendRaw(string data)
    {
        try
        {
            lock (_sync)
            {
                if (_port?.IsOpen == true)
                    _port.Write(data);
            }
        }
        catch (Exception ex) { AppendLog($"[TX fail] {ex.Message}"); }
    }

    private void OnDataReceived(object? sender, SerialDataReceivedEventArgs e)
    {
        string chunk;
        try
        {
            lock (_sync)
            {
                if (_port?.IsOpen != true) return;
                chunk = _port.ReadExisting();
            }
        }
        catch { return; }

        if (string.IsNullOrEmpty(chunk)) return;
        lock (_rx) _rx.Append(chunk);
        ProcessBuffer();
    }

    private void ProcessBuffer()
    {
        string buf;
        lock (_rx)
        {
            buf = _rx.ToString();
            _rx.Clear();
        }
        var lines = buf.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        // keep incomplete last fragment
        if (!buf.EndsWith('\n') && lines.Length > 0)
        {
            lock (_rx) _rx.Append(lines[^1]);
            lines = lines.Take(lines.Length - 1).ToArray();
        }

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0) continue;
            AppendLog($"< {line}");

            if (line.Equals("ok", StringComparison.OrdinalIgnoreCase))
            {
                if (_okPending > 0) _okPending--;
                OkReceived?.Invoke();
                continue;
            }
            if (line.StartsWith("error", StringComparison.OrdinalIgnoreCase))
            {
                if (_okPending > 0) _okPending--;
                continue;
            }
            if (GrblStatusParser.TryParse(line, LastStatus))
                StatusUpdated?.Invoke(LastStatus);
        }
    }

    private void AppendLog(string msg) => LogReceived?.Invoke(msg);

    public void Dispose() => Disconnect();
}
