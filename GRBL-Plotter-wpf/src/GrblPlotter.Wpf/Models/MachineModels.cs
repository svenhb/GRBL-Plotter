namespace GrblPlotter.Wpf.Models;

public enum GrblMachineState
{
    Unknown,
    Idle,
    Run,
    Hold,
    Jog,
    Alarm,
    Door,
    Check,
    Home,
    Sleep
}

public sealed class AxisPosition
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double A { get; set; }
    public double B { get; set; }
    public double C { get; set; }

    public override string ToString() =>
        $"X:{X:0.000} Y:{Y:0.000} Z:{Z:0.000}";
}

public sealed class GrblStatusSnapshot
{
    public GrblMachineState State { get; set; } = GrblMachineState.Unknown;
    public AxisPosition Machine { get; } = new();
    public AxisPosition Work { get; } = new();
    public AxisPosition Wco { get; } = new();
    public double Feed { get; set; }
    public double Spindle { get; set; }
    public int OvFeed { get; set; } = 100;
    public int OvRapid { get; set; } = 100;
    public int OvSpindle { get; set; } = 100;
    public string Raw { get; set; } = string.Empty;
    public DateTime Utc { get; set; } = DateTime.UtcNow;
}

public sealed class GCodeSegment
{
    public bool Rapid { get; set; }
    public double X0 { get; set; }
    public double Y0 { get; set; }
    public double X1 { get; set; }
    public double Y1 { get; set; }
}

public sealed class GCodeDocument
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName => string.IsNullOrEmpty(FilePath) ? "(unsaved)" : System.IO.Path.GetFileName(FilePath);
    public List<string> Lines { get; } = new();
    public List<GCodeSegment> Segments { get; } = new();
    public double MinX { get; set; }
    public double MinY { get; set; }
    public double MaxX { get; set; }
    public double MaxY { get; set; }
    public bool IsEmpty => Lines.Count == 0;
}
