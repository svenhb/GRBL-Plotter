using System.Windows;
using System.Windows.Controls;
using GrblPlotter.Wpf.Services;

namespace GrblPlotter.Wpf.Views;

/// <summary>Basic GRBL $$ settings viewer / editor (WinForms GrblSetup equivalent).</summary>
public class GrblSetupWindow : Window
{
    private readonly GrblSerialService _serial;
    private readonly ListBox _list;
    private readonly TextBox _cmd;

    public GrblSetupWindow(GrblSerialService serial)
    {
        _serial = serial;
        Title = "GRBL Setup ($$)";
        Width = 520;
        Height = 480;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        try { Background = (System.Windows.Media.Brush)FindResource("BgAppBrush"); } catch { /* */ }

        var dock = new DockPanel { Margin = new Thickness(8) };
        var top = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
        var btnRefresh = new Button { Content = "Query $$", MinWidth = 90, Margin = new Thickness(0, 0, 6, 0) };
        var btnInfo = new Button { Content = "$I", MinWidth = 48, Margin = new Thickness(0, 0, 6, 0) };
        var btnG = new Button { Content = "$G", MinWidth = 48, Margin = new Thickness(0, 0, 6, 0) };
        var btnHash = new Button { Content = "$#", MinWidth = 48 };
        top.Children.Add(btnRefresh);
        top.Children.Add(btnInfo);
        top.Children.Add(btnG);
        top.Children.Add(btnHash);
        DockPanel.SetDock(top, Dock.Top);

        _cmd = new TextBox { Margin = new Thickness(0, 8, 0, 0) };
        var send = new Button { Content = "Send", MinWidth = 64, Margin = new Thickness(6, 8, 0, 0) };
        var row = new Grid();
        row.ColumnDefinitions.Add(new ColumnDefinition());
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Grid.SetColumn(send, 1);
        row.Children.Add(_cmd);
        row.Children.Add(send);
        DockPanel.SetDock(row, Dock.Bottom);

        _list = new ListBox { FontFamily = new System.Windows.Media.FontFamily("Consolas") };
        dock.Children.Add(top);
        dock.Children.Add(row);
        dock.Children.Add(_list);
        Content = dock;

        void Append(string s) => Dispatcher.Invoke(() =>
        {
            _list.Items.Add(s);
            while (_list.Items.Count > 600) _list.Items.RemoveAt(0);
            _list.ScrollIntoView(_list.Items[^1]);
        });

        void OnLog(string line) => Append(line);
        _serial.LogReceived += OnLog;
        Closed += (_, _) => _serial.LogReceived -= OnLog;

        btnRefresh.Click += (_, _) => { _list.Items.Clear(); _serial.SendLine("$$"); };
        btnInfo.Click += (_, _) => _serial.SendLine("$I");
        btnG.Click += (_, _) => _serial.SendLine("$G");
        btnHash.Click += (_, _) => _serial.SendLine("$#");
        send.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_cmd.Text)) return;
            _serial.SendLine(_cmd.Text.Trim());
            Append("> " + _cmd.Text.Trim());
            _cmd.Clear();
        };

        _list.MouseDoubleClick += (_, _) =>
        {
            if (_list.SelectedItem is string s && s.StartsWith('$'))
            {
                // $N=value → put in command box for edit
                var eq = s.IndexOf('=');
                _cmd.Text = eq > 0 ? s[..(eq + 1)] : s;
                _cmd.CaretIndex = _cmd.Text.Length;
                _cmd.Focus();
            }
        };
    }
}
