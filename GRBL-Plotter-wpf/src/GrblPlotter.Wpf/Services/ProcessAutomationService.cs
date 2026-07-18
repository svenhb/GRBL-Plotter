using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services;

public enum AutomationActionType
{
    SendCode,
    Wait,
    WaitIdle,
    Probe,
    LoadFile,
    Message
}

public sealed class AutomationStep
{
    public bool Enabled { get; set; } = true;
    public AutomationActionType Action { get; set; } = AutomationActionType.SendCode;
    public string Parameter { get; set; } = "";
    public string Note { get; set; } = "";
}

/// <summary>Runs a list of <see cref="AutomationStep"/> against a <see cref="GrblSerialService"/>, one step at a time.</summary>
public sealed class ProcessAutomationService
{
    private readonly GrblSerialService _serial;
    private readonly Action<string>? _loadFile;
    private readonly Action<string>? _showMessage;
    private CancellationTokenSource? _cts;

    public event Action<string>? LogMessage;
    public event Action<int, int>? StepProgress;
    public event Action? Completed;
    public event Action? Stopped;

    public bool IsRunning { get; private set; }

    public ProcessAutomationService(GrblSerialService serial, Action<string>? loadFile = null, Action<string>? showMessage = null)
    {
        _serial = serial;
        _loadFile = loadFile;
        _showMessage = showMessage;
    }

    public void Stop() => _cts?.Cancel();

    public async Task RunAsync(IReadOnlyList<AutomationStep> steps)
    {
        if (IsRunning) return;
        IsRunning = true;
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        try
        {
            for (var i = 0; i < steps.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                var step = steps[i];
                if (!step.Enabled) continue;
                StepProgress?.Invoke(i, steps.Count);
                Log($"[{i + 1}/{steps.Count}] {step.Action}: {step.Parameter}");
                await RunStepAsync(step, ct);
            }
            Completed?.Invoke();
        }
        catch (OperationCanceledException)
        {
            Stopped?.Invoke();
        }
        catch (Exception ex)
        {
            Log($"[ERROR] {ex.Message}");
            Stopped?.Invoke();
        }
        finally
        {
            IsRunning = false;
        }
    }

    private async Task RunStepAsync(AutomationStep step, CancellationToken ct)
    {
        switch (step.Action)
        {
            case AutomationActionType.SendCode:
                foreach (var line in step.Parameter.Replace("\r\n", "\n").Split('\n'))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    _serial.SendLine(line);
                    await WaitForOkAsync(ct, TimeSpan.FromSeconds(30));
                }
                break;

            case AutomationActionType.Wait:
                var ms = int.TryParse(step.Parameter, out var v) ? v : 1000;
                await Task.Delay(ms, ct);
                break;

            case AutomationActionType.WaitIdle:
                await WaitIdleAsync(ct);
                break;

            case AutomationActionType.Probe:
                var cmd = string.IsNullOrWhiteSpace(step.Parameter) ? "G38.2 G91 Z-10 F100" : step.Parameter;
                _serial.SendLine(cmd);
                await WaitForOkAsync(ct, TimeSpan.FromSeconds(30));
                break;

            case AutomationActionType.LoadFile:
                _loadFile?.Invoke(step.Parameter);
                break;

            case AutomationActionType.Message:
                _showMessage?.Invoke(step.Parameter);
                Log($"[MESSAGE] {step.Parameter}");
                break;
        }
    }

    private async Task WaitForOkAsync(CancellationToken ct, TimeSpan timeout)
    {
        var tcs = new TaskCompletionSource();
        void Handler() => tcs.TrySetResult();
        _serial.OkReceived += Handler;
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(timeout);
        using var reg = timeoutCts.Token.Register(() => tcs.TrySetCanceled());
        try
        {
            await tcs.Task.ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            // timed out waiting for "ok" - continue the automation anyway
        }
        finally
        {
            _serial.OkReceived -= Handler;
        }
    }

    private async Task WaitIdleAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (_serial.LastStatus.State == GrblMachineState.Idle) return;
            _serial.SendRealtime((byte)'?');
            await Task.Delay(300, ct);
        }
    }

    private void Log(string msg) => LogMessage?.Invoke(msg);
}
