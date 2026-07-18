using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using GrblPlotter.Wpf.Services;
using GrblPlotter.Wpf.Services.Import;
using GrblPlotter.Wpf.ViewModels;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Search;

namespace GrblPlotter.Wpf.Views;

public partial class MainWindow : Window
{
    private readonly Dictionary<string, Window> _tools = new();
    private GamePadService? _gamePad;
    private FoldingManager? _foldingManager;
    private bool _syncingEditor;
    private bool _panning;
    private Point _panStart;
    private double _panOriginX, _panOriginY;
    private System.Windows.Point _lastCanvasClick;

    public MainWindow()
    {
        InitializeComponent();
        PreviewKeyDown += OnPreviewKeyDown;
    }

    private MainViewModel Vm => (MainViewModel)DataContext;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        SearchPanel.Install(CodeEditor);
        _foldingManager = FoldingManager.Install(CodeEditor.TextArea);
        CodeEditor.Text = Vm.GcodeText ?? "";
        CodeEditor.TextChanged += CodeEditor_TextChanged;
        Vm.PropertyChanged += Vm_PropertyChanged;
        ApplyXmlFolding(1);
        if (!string.IsNullOrEmpty(Vm.Settings.Language))
            LocalizationService.Apply(Vm.Settings.Language);
    }

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MainViewModel.GcodeText) || _syncingEditor) return;
        Dispatcher.BeginInvoke(() =>
        {
            if (_syncingEditor) return;
            _syncingEditor = true;
            try
            {
                var caret = CodeEditor.CaretOffset;
                CodeEditor.Text = Vm.GcodeText ?? "";
                CodeEditor.CaretOffset = Math.Clamp(caret, 0, CodeEditor.Text.Length);
                ApplyXmlFolding(1);
            }
            finally { _syncingEditor = false; }
        }, DispatcherPriority.Background);
    }

    private void CodeEditor_TextChanged(object? sender, EventArgs e)
    {
        if (_syncingEditor) return;
        _syncingEditor = true;
        try { Vm.GcodeText = CodeEditor.Text; }
        finally { _syncingEditor = false; }
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if ((Keyboard.FocusedElement is TextBox || CodeEditor.TextArea.IsKeyboardFocusWithin) &&
            e.Key is not (Key.Escape or Key.F5 or Key.F6 or Key.F7))
            return;

        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O) { Vm.OpenFileCommand.Execute(null); e.Handled = true; }
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S) { Vm.SaveFileCommand.Execute(null); e.Handled = true; }
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z) { Vm.UndoCommand.Execute(null); e.Handled = true; }
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.R) { Vm.SoftResetCommand.Execute(null); e.Handled = true; }
        if (e.Key == Key.Escape) { Vm.JogCancelCommand.Execute(null); e.Handled = true; }
        // Space activates focused buttons — don't also FeedHold or Start+Hold race.
        if (e.Key == Key.Space && Keyboard.Modifiers == ModifierKeys.None
            && Keyboard.FocusedElement is not System.Windows.Controls.Button
            && Keyboard.FocusedElement is not System.Windows.Controls.Primitives.ButtonBase)
        {
            Vm.FeedHoldCommand.Execute(null);
            e.Handled = true;
        }
        if (e.Key == Key.F5) { Vm.StreamStartCommand.Execute(null); e.Handled = true; }
        if (e.Key == Key.F6) { Vm.StreamPauseCommand.Execute(null); e.Handled = true; }
        if (e.Key == Key.F7) { Vm.StreamStopCommand.Execute(null); e.Handled = true; }
        if (e.Key == Key.PageUp) { Vm.JogCommand.Execute("Z+"); e.Handled = true; }
        if (e.Key == Key.PageDown) { Vm.JogCommand.Execute("Z-"); e.Handled = true; }
        if (Keyboard.Modifiers == ModifierKeys.None)
        {
            switch (e.Key)
            {
                case Key.Left: Vm.JogCommand.Execute("X-"); e.Handled = true; break;
                case Key.Right: Vm.JogCommand.Execute("X+"); e.Handled = true; break;
                case Key.Up: Vm.JogCommand.Execute("Y+"); e.Handled = true; break;
                case Key.Down: Vm.JogCommand.Execute("Y-"); e.Handled = true; break;
            }
        }
    }

    private void ShowTool(string key, Func<Window> factory)
    {
        if (_tools.TryGetValue(key, out var existing) && existing.IsLoaded)
        {
            existing.Activate();
            return;
        }
        var w = factory();
        w.Owner = this;
        w.Closed += (_, _) => _tools.Remove(key);
        _tools[key] = w;
        w.Show();
    }

    private void OpenSerial_Click(object sender, RoutedEventArgs e) =>
        ShowTool("serial", () => new SerialWindow { DataContext = DataContext });

    private void OpenSetup_Click(object sender, RoutedEventArgs e)
    {
        ShowTool("setup", () =>
        {
            var w = new SetupWindow(Vm.Settings);
            w.Closed += (_, _) =>
            {
                Vm.RefreshCustomButtonsFromSettings();
                Vm.ReloadParityFromSettings();
            };
            return w;
        });
    }

    private void OpenAbout_Click(object sender, RoutedEventArgs e) =>
        ShowTool("about", () => new AboutWindow());

    private void OpenProbing_Click(object sender, RoutedEventArgs e) =>
        ShowTool("probe", () => new ProbingWindow(Vm.Serial));

    private void OpenHeightMap_Click(object sender, RoutedEventArgs e) =>
        ShowTool("hmap", () => new HeightMapWindow(Vm.Serial, () => Vm.Document, d => Vm.ApplyDocument(d)));

    private void OpenCamera_Click(object sender, RoutedEventArgs e) =>
        ShowTool("cam", () => new CameraWindow(c => Vm.Serial.SendLine(c), () => Vm.CurrentWorkPos));

    private void OpenAutomation_Click(object sender, RoutedEventArgs e) =>
        ShowTool("auto", () => new AutomationWindow(Vm.Serial, path => Vm.LoadPath(path), msg => MessageBox.Show(msg, "Automation")));

    private void OpenCoord_Click(object sender, RoutedEventArgs e) =>
        ShowTool("coord", () => new CoordSystemWindow(Vm.Serial));

    private void OpenText_Click(object sender, RoutedEventArgs e) =>
        ShowTool("text", () => new TextCreateWindow(g => Vm.ApplyGeneratedGCode(g, "text.nc")));

    private void OpenShape_Click(object sender, RoutedEventArgs e) =>
        ShowTool("shape", () => new ShapeCreateWindow(g => Vm.ApplyGeneratedGCode(g, "shape.nc")));

    private void OpenImage_Click(object sender, RoutedEventArgs e) =>
        ShowTool("image", () => new ImageCreateWindow(g => Vm.ApplyGeneratedGCode(g, "image.nc")));

    private void OpenBarcode_Click(object sender, RoutedEventArgs e) =>
        ShowTool("barcode", () => new BarcodeCreateWindow(g => Vm.ApplyGeneratedGCode(g, "barcode.nc")));

    private void OpenWire_Click(object sender, RoutedEventArgs e) =>
        ShowTool("wire", () => new WireCutterWindow(g => Vm.ApplyGeneratedGCode(g, "wire.nc")));

    private void OpenProjector_Click(object sender, RoutedEventArgs e) =>
        ShowTool("proj", () => new ProjectorWindow(Vm.ToolpathGeometry, Vm.RapidGeometry));

    private void OpenSecondSerial_Click(object sender, RoutedEventArgs e) =>
        ShowTool("serial2", () => new SecondSerialWindow());

    private void OpenDiyPad_Click(object sender, RoutedEventArgs e) =>
        ShowTool("diypad", () => new DiyControlPadWindow(Vm.Serial));

    private void OpenTablet_Click(object sender, RoutedEventArgs e) =>
        ShowTool("tablet", () => new TabletCreateWindow(g => Vm.ApplyGeneratedGCode(g, "tablet.nc")));

    private void OpenHershey_Click(object sender, RoutedEventArgs e)
    {
        var input = new Window
        {
            Title = "Hershey / stroke text", Width = 400, Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this
        };
        var box = new TextBox { Text = "GRBL", Margin = new Thickness(12) };
        var fonts = LffFontLoader.ListFonts();
        var fontBox = new ComboBox { Margin = new Thickness(12, 0, 12, 0) };
        fontBox.Items.Add("(built-in Hershey)");
        foreach (var f in fonts) fontBox.Items.Add(f);
        fontBox.SelectedIndex = 0;
        var ok = new Button { Content = "Generate", Width = 90, Margin = new Thickness(12), IsDefault = true };
        ok.Click += (_, _) =>
        {
            var text = box.Text ?? "GRBL";
            if (fontBox.SelectedIndex <= 0 || fontBox.SelectedItem is not string fn || fn.StartsWith('('))
                Vm.ApplyDocument(HersheyStrokeFont.Generate(text, heightMm: 12));
            else
                Vm.ApplyDocument(LffFontLoader.Render(text, fn, 12));
            input.Close();
        };
        var sp = new StackPanel();
        sp.Children.Add(new TextBlock { Text = "Text:", Margin = new Thickness(12, 12, 12, 0) });
        sp.Children.Add(box);
        sp.Children.Add(new TextBlock { Text = "Font:", Margin = new Thickness(12, 8, 12, 0) });
        sp.Children.Add(fontBox);
        sp.Children.Add(ok);
        input.Content = sp;
        input.ShowDialog();
    }

    private void OpenLff_Click(object sender, RoutedEventArgs e) => OpenHershey_Click(sender, e);

    private void LangEn_Click(object sender, RoutedEventArgs e) { LocalizationService.Apply("en"); Vm.Settings.Language = "en"; Vm.Settings.Save(); }
    private void LangZh_Click(object sender, RoutedEventArgs e) { LocalizationService.Apply("zh"); Vm.Settings.Language = "zh"; Vm.Settings.Save(); }
    private void LangDe_Click(object sender, RoutedEventArgs e) { LocalizationService.Apply("de"); Vm.Settings.Language = "de"; Vm.Settings.Save(); }

    private void GamePadToggle_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem mi) return;
        if (mi.IsChecked)
        {
            _gamePad?.Dispose();
            _gamePad = new GamePadService(
                line => Vm.Serial.SendLine(line),
                () => Vm.FeedHoldCommand.Execute(null),
                () => Vm.CycleStartCommand.Execute(null),
                () => Vm.JogStep,
                () => Vm.JogFeed);
            _gamePad.StatusChanged += s => Dispatcher.Invoke(() => Vm.SetStatus(s));
            _gamePad.Start();
            Vm.SetStatus("GamePad enabled (A=Hold B=Resume stick=jog)");
        }
        else
        {
            _gamePad?.Dispose();
            _gamePad = null;
            Vm.SetStatus("GamePad disabled");
        }
    }

    private void ExtensionsMenu_SubmenuOpened(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menu) return;
        menu.Items.Clear();
        var dirs = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Extensions"),
            Path.Combine(AppSettings.SettingsDirectory, "Extensions")
        };
        var files = new List<string>();
        foreach (var d in dirs)
        {
            if (!Directory.Exists(d)) continue;
            files.AddRange(Directory.GetFiles(d, "*.*"));
        }
        if (files.Count == 0)
        {
            menu.Items.Add(new MenuItem { Header = "(no Extensions folder files)", IsEnabled = false });
            return;
        }
        foreach (var f in files.OrderBy(x => x))
        {
            var item = new MenuItem { Header = Path.GetFileName(f), Tag = f };
            item.Click += (_, _) =>
            {
                try
                {
                    var ext = Path.GetExtension(f).ToLowerInvariant();
                    if (ext is ".nc" or ".gcode" or ".svg" or ".dxf" or ".ngc" or ".hpgl" or ".plt")
                        Vm.LoadPath(f);
                    else
                        Process.Start(new ProcessStartInfo(f) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Extension");
                }
            };
            menu.Items.Add(item);
        }
    }

    private void OpenThirdSerial_Click(object sender, RoutedEventArgs e) =>
        ShowTool("serial3", () => new SecondSerialWindow { Title = "3rd Serial COM" });

    private void OpenGrblSetup_Click(object sender, RoutedEventArgs e) =>
        ShowTool("grblsetup", () => new GrblSetupWindow(Vm.Serial));

    private void OpenLaserTools_Click(object sender, RoutedEventArgs e) =>
        ShowTool("laser", () => new LaserToolsWindow(Vm.Serial, g => Vm.ApplyGeneratedGCode(g, "laser-test.nc")));

    private void OpenJogPath_Click(object sender, RoutedEventArgs e) =>
        ShowTool("jogpath", () => new JogPathCreateWindow(g => Vm.ApplyGeneratedGCode(g, "jog-path.nc")));

    private void BringFormsToFront_Click(object sender, RoutedEventArgs e)
    {
        Activate();
        foreach (var w in _tools.Values.Where(x => x.IsLoaded))
        {
            w.Activate();
            w.Topmost = true;
            w.Topmost = false;
        }
    }

    private void RecentMenu_SubmenuOpened(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menu) return;
        menu.Items.Clear();
        if (Vm.RecentFiles.Count == 0)
        {
            menu.Items.Add(new MenuItem { Header = "(empty)", IsEnabled = false });
            return;
        }
        foreach (var path in Vm.RecentFiles.ToList())
        {
            var item = new MenuItem { Header = path, Tag = path };
            item.Click += (_, _) => Vm.OpenRecentCommand.Execute(path);
            menu.Items.Add(item);
        }
    }

    private void PreviewCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_panning || Keyboard.Modifiers == ModifierKeys.Shift) return;
        if (sender is not Canvas canvas) return;
        var p = e.GetPosition(canvas);
        Vm.CanvasClick(p.X, p.Y);
    }

    private void PreviewCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Canvas canvas) return;
        var p = e.GetPosition(canvas);
        Vm.CanvasClick(p.X, p.Y);
        _lastCanvasClick = p;
    }

    private void PreviewHost_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        double factor = e.Delta > 0 ? 1.1 : 1 / 1.1;
        Vm.ViewZoom = Math.Clamp(Vm.ViewZoom * factor, 0.2, 8);
        e.Handled = true;
    }

    private void PreviewHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (Keyboard.Modifiers != ModifierKeys.Shift) return;
        _panning = true;
        _panStart = e.GetPosition((IInputElement)sender);
        _panOriginX = Vm.ViewPanX;
        _panOriginY = Vm.ViewPanY;
        ((UIElement)sender).CaptureMouse();
        e.Handled = true;
    }

    private void PreviewHost_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_panning) return;
        _panning = false;
        ((UIElement)sender).ReleaseMouseCapture();
    }

    private void PreviewHost_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_panning) return;
        var p = e.GetPosition((IInputElement)sender);
        Vm.ViewPanX = _panOriginX + (p.X - _panStart.X);
        Vm.ViewPanY = _panOriginY + (p.Y - _panStart.Y);
    }

    private void SetMarkerHere_Click(object sender, RoutedEventArgs e) =>
        Vm.CanvasSetMarker(_lastCanvasClick.X, _lastCanvasClick.Y);

    private void SendEditorLine_Click(object sender, RoutedEventArgs e)
    {
        string line;
        if (CodeEditor.TextArea.Selection.Length > 0)
            line = CodeEditor.SelectedText;
        else
        {
            int lineNo = CodeEditor.TextArea.Caret.Line;
            var docLine = CodeEditor.Document.GetLineByNumber(lineNo);
            line = CodeEditor.Document.GetText(docLine.Offset, docLine.Length);
        }
        Vm.SendEditorLineCommand.Execute(line);
    }

    private void FindReplace_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new Window
        {
            Title = "Find / Replace", Width = 360, Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this
        };
        var find = new TextBox { Margin = new Thickness(8) };
        var replace = new TextBox { Margin = new Thickness(8, 0, 8, 8) };
        var findBtn = new Button { Content = "Find", Width = 80, Margin = new Thickness(4) };
        var replBtn = new Button { Content = "Replace all", Width = 100, Margin = new Thickness(4) };
        findBtn.Click += (_, _) => Vm.FindReplaceEditorCommand.Execute(find.Text);
        replBtn.Click += (_, _) =>
        {
            Vm.FindReplaceEditorCommand.Execute(find.Text + "|" + (replace.Text ?? ""));
            dlg.Close();
        };
        var sp = new StackPanel();
        sp.Children.Add(new TextBlock { Text = "Find", Margin = new Thickness(8, 8, 8, 0) });
        sp.Children.Add(find);
        sp.Children.Add(new TextBlock { Text = "Replace", Margin = new Thickness(8, 0, 8, 0) });
        sp.Children.Add(replace);
        var row = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        row.Children.Add(findBtn);
        row.Children.Add(replBtn);
        sp.Children.Add(row);
        dlg.Content = sp;
        dlg.ShowDialog();
    }

    private void FoldLevel1_Click(object sender, RoutedEventArgs e) => ApplyXmlFolding(1);
    private void FoldLevel2_Click(object sender, RoutedEventArgs e) => ApplyXmlFolding(2);
    private void FoldLevel3_Click(object sender, RoutedEventArgs e) => ApplyXmlFolding(3);

    private void FoldExpand_Click(object sender, RoutedEventArgs e)
    {
        if (_foldingManager == null) return;
        foreach (var f in _foldingManager.AllFoldings)
            f.IsFolded = false;
    }

    private void ApplyXmlFolding(int level)
    {
        if (_foldingManager == null) return;
        var lines = (CodeEditor.Text ?? "").Replace("\r\n", "\n").Split('\n');
        var foldings = new List<NewFolding>();
        foreach (var (start, end) in XmlMarkerService.FoldRanges(lines, level))
        {
            if (end <= start) continue;
            try
            {
                var startOff = CodeEditor.Document.GetLineByNumber(start + 1).Offset;
                var endLine = CodeEditor.Document.GetLineByNumber(Math.Min(end + 1, CodeEditor.Document.LineCount));
                var endOff = endLine.Offset + endLine.Length;
                if (endOff > startOff)
                    foldings.Add(new NewFolding(startOff, endOff) { Name = $"…L{start + 1}-{end + 1}" });
            }
            catch { /* ignore bad ranges */ }
        }
        _foldingManager.UpdateFoldings(foldings, -1);
        foreach (var f in _foldingManager.AllFoldings)
            f.IsFolded = true;
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            Vm.LoadPath(files[0]);
    }

    protected override void OnClosed(EventArgs e)
    {
        Vm.PropertyChanged -= Vm_PropertyChanged;
        _gamePad?.Dispose();
        Vm.Settings.JogStep = Vm.JogStep;
        Vm.Settings.JogFeed = Vm.JogFeed;
        Vm.Settings.Save();
        Vm.Serial.Disconnect();
        foreach (var w in _tools.Values.ToList())
        {
            try { w.Close(); } catch { /* ignore */ }
        }
        base.OnClosed(e);
    }
}
