using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GrblPlotter.Wpf.Services;
using Microsoft.Win32;

namespace GrblPlotter.Wpf.ViewModels;

public partial class AutomationStepVm : ObservableObject
{
    [ObservableProperty] private bool _enabled = true;
    [ObservableProperty] private AutomationActionType _action = AutomationActionType.SendCode;
    [ObservableProperty] private string _parameter = "";
    [ObservableProperty] private string _note = "";

    public AutomationStep ToModel() => new() { Enabled = Enabled, Action = Action, Parameter = Parameter, Note = Note };

    public static AutomationStepVm FromModel(AutomationStep s) =>
        new() { Enabled = s.Enabled, Action = s.Action, Parameter = s.Parameter, Note = s.Note };
}

/// <summary>DataGrid-backed editor + runner for a sequence of <see cref="AutomationStep"/>s,
/// driven by <see cref="ProcessAutomationService"/>.</summary>
public partial class AutomationViewModel : ObservableObject
{
    private readonly ProcessAutomationService _engine;

    public ObservableCollection<AutomationStepVm> Steps { get; } = new();
    public ObservableCollection<string> Log { get; } = new();

    [ObservableProperty] private bool _isRunning;
    [ObservableProperty] private string _statusText = "idle";
    [ObservableProperty] private double _progress;

    public AutomationViewModel(GrblSerialService serial, Action<string>? loadFile = null, Action<string>? showMessage = null)
    {
        _engine = new ProcessAutomationService(serial, loadFile, showMessage ?? (msg => AppendLog($"[MSG] {msg}")));
        _engine.LogMessage += AppendLog;
        _engine.StepProgress += (i, total) => Application.Current?.Dispatcher.Invoke(() =>
        {
            Progress = total == 0 ? 0 : 100.0 * i / total;
            StatusText = $"running step {i + 1}/{total}";
        });
        _engine.Completed += () => Application.Current?.Dispatcher.Invoke(() =>
        {
            IsRunning = false;
            StatusText = "completed";
            Progress = 100;
        });
        _engine.Stopped += () => Application.Current?.Dispatcher.Invoke(() =>
        {
            IsRunning = false;
            StatusText = "stopped";
        });

        Steps.Add(new AutomationStepVm { Action = AutomationActionType.Message, Parameter = "Starting job", Note = "example step" });
    }

    private void AppendLog(string msg) => Application.Current?.Dispatcher.Invoke(() =>
    {
        Log.Add(msg);
        while (Log.Count > 500) Log.RemoveAt(0);
    });

    [RelayCommand] private void AddStep() => Steps.Add(new AutomationStepVm());

    [RelayCommand]
    private void RemoveStep(AutomationStepVm? step)
    {
        if (step != null) Steps.Remove(step);
    }

    [RelayCommand] private void ClearSteps() => Steps.Clear();

    [RelayCommand]
    private void SaveSteps()
    {
        var dlg = new SaveFileDialog { Filter = "Automation steps (*.json)|*.json", FileName = "automation.json" };
        if (dlg.ShowDialog() != true) return;
        var models = Steps.Select(s => s.ToModel()).ToList();
        var json = JsonSerializer.Serialize(models, new JsonSerializerOptions { WriteIndented = true });
        System.IO.File.WriteAllText(dlg.FileName, json);
        StatusText = $"saved {dlg.FileName}";
    }

    [RelayCommand]
    private void LoadSteps()
    {
        var dlg = new OpenFileDialog { Filter = "Automation steps (*.json)|*.json" };
        if (dlg.ShowDialog() != true) return;
        var json = System.IO.File.ReadAllText(dlg.FileName);
        var models = JsonSerializer.Deserialize<List<AutomationStep>>(json) ?? new();
        Steps.Clear();
        foreach (var m in models) Steps.Add(AutomationStepVm.FromModel(m));
        StatusText = $"loaded {Steps.Count} steps";
    }

    [RelayCommand]
    private async Task RunAsync()
    {
        if (IsRunning) return;
        IsRunning = true;
        StatusText = "running…";
        Log.Clear();
        var models = Steps.Select(s => s.ToModel()).ToList();
        await _engine.RunAsync(models);
    }

    [RelayCommand]
    private void StopRun() => _engine.Stop();
}
