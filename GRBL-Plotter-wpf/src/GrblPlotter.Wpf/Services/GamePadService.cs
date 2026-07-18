using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace GrblPlotter.Wpf.Services;

/// <summary>XInput gamepad → jog / hold / cycle (Phase 5).</summary>
public sealed class GamePadService : IDisposable
{
    private readonly DispatcherTimer _timer;
    private readonly Action<string> _sendLine;
    private readonly Action _feedHold;
    private readonly Action _cycleStart;
    private readonly Func<double> _jogStep;
    private readonly Func<double> _jogFeed;
    private bool _enabled;

    public event Action<string>? StatusChanged;
    public bool IsEnabled => _enabled;

    public GamePadService(Action<string> sendLine, Action feedHold, Action cycleStart, Func<double> jogStep, Func<double> jogFeed)
    {
        _sendLine = sendLine;
        _feedHold = feedHold;
        _cycleStart = cycleStart;
        _jogStep = jogStep;
        _jogFeed = jogFeed;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(80) };
        _timer.Tick += (_, _) => Poll();
    }

    public void Start()
    {
        _enabled = true;
        _timer.Start();
        StatusChanged?.Invoke("gamepad polling");
    }

    public void Stop()
    {
        _enabled = false;
        _timer.Stop();
        StatusChanged?.Invoke("gamepad stopped");
    }

    private ushort _prevButtons;

    private void Poll()
    {
        if (!_enabled) return;
        var state = new XINPUT_STATE();
        if (XInputGetState(0, ref state) != 0)
        {
            StatusChanged?.Invoke("no gamepad on port 0");
            return;
        }
        var b = state.Gamepad.wButtons;
        short lx = state.Gamepad.sThumbLX;
        short ly = state.Gamepad.sThumbLY;
        const short dead = 8000;

        // Edge-triggered buttons
        if (Pressed(b, _prevButtons, 0x1000)) _feedHold();      // A
        if (Pressed(b, _prevButtons, 0x2000)) _cycleStart();    // B
        if (Pressed(b, _prevButtons, 0x4000)) _sendLine("$X"); // X unlock
        if (Pressed(b, _prevButtons, 0x8000)) _sendLine("$H"); // Y home
        _prevButtons = b;

        double dx = 0, dy = 0;
        if (lx > dead) dx = _jogStep();
        if (lx < -dead) dx = -_jogStep();
        if (ly > dead) dy = _jogStep();
        if (ly < -dead) dy = -_jogStep();
        if (Math.Abs(dx) > 0 || Math.Abs(dy) > 0)
        {
            var parts = new List<string>();
            if (Math.Abs(dx) > 0) parts.Add($"X{dx.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            if (Math.Abs(dy) > 0) parts.Add($"Y{dy.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            _sendLine($"$J=G91 G21 {string.Join(" ", parts)} F{_jogFeed().ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        }
    }

    private static bool Pressed(ushort now, ushort prev, ushort mask) =>
        (now & mask) != 0 && (prev & mask) == 0;

    public void Dispose() => Stop();

    [StructLayout(LayoutKind.Sequential)]
    private struct XINPUT_GAMEPAD
    {
        public ushort wButtons;
        public byte bLeftTrigger, bRightTrigger;
        public short sThumbLX, sThumbLY, sThumbRX, sThumbRY;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XINPUT_STATE
    {
        public uint dwPacketNumber;
        public XINPUT_GAMEPAD Gamepad;
    }

    [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
    private static extern uint XInputGetState(uint dwUserIndex, ref XINPUT_STATE pState);
}
