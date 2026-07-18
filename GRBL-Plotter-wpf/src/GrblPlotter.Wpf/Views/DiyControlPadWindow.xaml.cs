using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GrblPlotter.Wpf.Services;

namespace GrblPlotter.Wpf.Views;

/// <summary>DIY control pad — independent serial + forward lines/realtime to CNC (WinForms ControlDiyControlPad).</summary>
public class DiyControlPadWindow : Window
{
    private SerialPort? _port;
    private readonly GrblSerialService _cnc;
    private readonly ListBox _log;
    private readonly ComboBox _ports;
    private readonly ComboBox _baud;
    private readonly CheckBox _forward;

    public DiyControlPadWindow(GrblSerialService cnc)
    {
        _cnc = cnc;
        Title = "DIY Control Pad";
        Width = 480; Height = 440;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        try { Background = (Brush)FindResource("BgAppBrush"); } catch { }

        var dock = new DockPanel { Margin = new Thickness(8) };
        var top = new Grid { Margin = new Thickness(0, 0, 0, 8) };
        top.ColumnDefinitions.Add(new ColumnDefinition());
        top.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });
        top.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        top.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        _ports = new ComboBox();
        foreach (var p in SerialPort.GetPortNames()) _ports.Items.Add(p);
        if (_ports.Items.Count > 0) _ports.SelectedIndex = 0;
        _baud = new ComboBox();
        foreach (var b in new[] { 9600, 115200 }) _baud.Items.Add(b);
        _baud.SelectedItem = 115200;
        var open = new Button { Content = "Open", MinWidth = 64, Margin = new Thickness(4, 0, 0, 0) };
        var scan = new Button { Content = "Scan", MinWidth = 56, Margin = new Thickness(4, 0, 0, 0) };
        Grid.SetColumn(_baud, 1); Grid.SetColumn(open, 2); Grid.SetColumn(scan, 3);
        top.Children.Add(_ports); top.Children.Add(_baud); top.Children.Add(open); top.Children.Add(scan);
        DockPanel.SetDock(top, Dock.Top);

        _forward = new CheckBox { Content = "Forward pad commands to CNC", IsChecked = true, Margin = new Thickness(0, 0, 0, 6) };
        DockPanel.SetDock(_forward, Dock.Top);

        _log = new ListBox { FontFamily = new FontFamily("Consolas") };
        var cmdRow = new Grid { Margin = new Thickness(0, 8, 0, 0) };
        cmdRow.ColumnDefinitions.Add(new ColumnDefinition());
        cmdRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var cmd = new TextBox();
        var send = new Button { Content = "Send", MinWidth = 64, Margin = new Thickness(6, 0, 0, 0) };
        Grid.SetColumn(send, 1);
        cmdRow.Children.Add(cmd); cmdRow.Children.Add(send);
        DockPanel.SetDock(cmdRow, Dock.Bottom);

        void Append(string s) => Dispatcher.Invoke(() =>
        {
            _log.Items.Add(s);
            while (_log.Items.Count > 500) _log.Items.RemoveAt(0);
        });

        scan.Click += (_, _) =>
        {
            _ports.Items.Clear();
            foreach (var p in SerialPort.GetPortNames()) _ports.Items.Add(p);
            if (_ports.Items.Count > 0) _ports.SelectedIndex = 0;
        };

        open.Click += (_, _) =>
        {
            try
            {
                if (_port?.IsOpen == true)
                {
                    _port.Close(); _port.Dispose(); _port = null;
                    open.Content = "Open";
                    Append("closed");
                    return;
                }
                if (_ports.SelectedItem is not string name) return;
                _port = new SerialPort(name, (int)(_baud.SelectedItem ?? 115200))
                {
                    Encoding = Encoding.GetEncoding(28591),
                    NewLine = "\n"
                };
                _port.DataReceived += (_, __) =>
                {
                    try
                    {
                        var data = _port.ReadExisting();
                        foreach (var line in data.Replace("\r", "").Split('\n'))
                        {
                            var t = line.Trim();
                            if (t.Length == 0) continue;
                            Append("< " + t);
                            if (_forward.IsChecked == true)
                            {
                                // Realtime single-byte commands
                                if (t.Length == 1 && t[0] < 32)
                                    _cnc.SendRealtime((byte)t[0]);
                                else
                                    _cnc.SendLine(t);
                            }
                        }
                    }
                    catch { /* ignore */ }
                };
                _port.Open();
                open.Content = "Close";
                Append("opened " + name);
            }
            catch (Exception ex) { Append("[ERR] " + ex.Message); }
        };

        send.Click += (_, _) =>
        {
            if (_port?.IsOpen != true || string.IsNullOrWhiteSpace(cmd.Text)) return;
            _port.Write(cmd.Text.Trim() + "\n");
            Append("> " + cmd.Text.Trim());
            cmd.Clear();
        };

        dock.Children.Add(top);
        dock.Children.Add(_forward);
        dock.Children.Add(cmdRow);
        dock.Children.Add(_log);
        Content = dock;
        Closed += (_, _) => { try { _port?.Close(); _port?.Dispose(); } catch { } };
    }
}
