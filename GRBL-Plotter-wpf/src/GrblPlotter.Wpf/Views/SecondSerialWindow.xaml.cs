using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GrblPlotter.Wpf.Views;

/// <summary>Secondary serial (tool changer / DIY pad) â€?WinForms Control2ndGRBL / DIYControlPad equivalent.</summary>
public class SecondSerialWindow : Window
{
    private SerialPort? _port;

    public SecondSerialWindow()
    {
        Title = "2nd Serial / DIY Control";
        Width = 480; Height = 420;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        try { Background = (Brush)FindResource("BgAppBrush"); } catch { }

        var dock = new DockPanel { Margin = new Thickness(8) };
        var top = new Grid { Margin = new Thickness(0, 0, 0, 8) };
        top.ColumnDefinitions.Add(new ColumnDefinition());
        top.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
        top.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        top.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var ports = new ComboBox();
        foreach (var p in SerialPort.GetPortNames()) ports.Items.Add(p);
        if (ports.Items.Count > 0) ports.SelectedIndex = 0;
        var baud = new ComboBox();
        foreach (var b in new[] { 9600, 115200 }) baud.Items.Add(b);
        baud.SelectedItem = 115200;
        var openBtn = new Button { Content = "Open", MinWidth = 64, Margin = new Thickness(4, 0, 0, 0) };
        var scanBtn = new Button { Content = "Scan", MinWidth = 56, Margin = new Thickness(4, 0, 0, 0) };
        Grid.SetColumn(baud, 1); Grid.SetColumn(openBtn, 2); Grid.SetColumn(scanBtn, 3);
        top.Children.Add(ports); top.Children.Add(baud); top.Children.Add(openBtn); top.Children.Add(scanBtn);
        DockPanel.SetDock(top, Dock.Top);

        var log = new ListBox { FontFamily = new System.Windows.Media.FontFamily("Consolas") };
        var cmdRow = new Grid { Margin = new Thickness(0, 8, 0, 0) };
        cmdRow.ColumnDefinitions.Add(new ColumnDefinition());
        cmdRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var cmd = new TextBox();
        var send = new Button { Content = "Send", Margin = new Thickness(6, 0, 0, 0), MinWidth = 64 };
        Grid.SetColumn(send, 1);
        cmdRow.Children.Add(cmd); cmdRow.Children.Add(send);
        DockPanel.SetDock(cmdRow, Dock.Bottom);

        void Append(string s) => Dispatcher.Invoke(() => { log.Items.Add(s); while (log.Items.Count > 400) log.Items.RemoveAt(0); });

        scanBtn.Click += (_, _) =>
        {
            ports.Items.Clear();
            foreach (var p in SerialPort.GetPortNames()) ports.Items.Add(p);
            if (ports.Items.Count > 0) ports.SelectedIndex = 0;
        };

        openBtn.Click += (_, _) =>
        {
            try
            {
                if (_port?.IsOpen == true)
                {
                    _port.Close(); _port.Dispose(); _port = null;
                    openBtn.Content = "Open";
                    Append("closed");
                    return;
                }
                if (ports.SelectedItem is not string name) return;
                _port = new SerialPort(name, (int)(baud.SelectedItem ?? 115200)) { Encoding = Encoding.ASCII, NewLine = "\n" };
                _port.DataReceived += (_, __) =>
                {
                    try { Append("< " + _port.ReadExisting().Trim()); } catch { }
                };
                _port.Open();
                openBtn.Content = "Close";
                Append($"opened {name}");
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
        dock.Children.Add(cmdRow);
        dock.Children.Add(log);
        Content = dock;
        Closed += (_, _) => { try { _port?.Close(); _port?.Dispose(); } catch { } };
    }
}
