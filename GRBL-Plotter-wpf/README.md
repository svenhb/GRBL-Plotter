# GRBL-Plotter WPF

Industrial-themed WPF port of GRBL-Plotter (.NET 8).

## Run

```bat
dotnet run --project src\GrblPlotter.Wpf\GrblPlotter.Wpf.csproj -c Debug
```

EXE: `src\GrblPlotter.Wpf\bin\Debug\net8.0-windows\GRBL-Plotter-Wpf.exe`

## Capabilities (aligned with WinForms)

- **Machine:** COM CNC, 2nd serial, soft reset, hold/resume, overrides, jog, home, coord systems G54–G59
- **Streaming:** Start / Check ($C) / Pause / Resume / Stop + path simulation
- **Import:** SVG, DXF, HPGL/PLT, Gerber, CSV/Excellon, images, G-code (drag-drop supported)
- **Create:** Text, Shape, Image engraving, Barcode, Wire-cutter paths
- **Transform:** Mirror, scale, rotate, translate, reverse, origin presets
- **Workpiece tools:** Probing, Height map (probe + apply Z), Camera teach offsets, Projector overlay
- **Automation:** Step list (Send / Wait / WaitIdle / Probe / LoadFile / Message)
- **Setup / About:** JSON settings under `%AppData%\GRBL-Plotter-Wpf\`

See `FEATURE_PARITY.md` and `PLAN.md` for details and remaining gaps (live webcam SDK, full hatch/tangential graphic pipeline, GamePad, full i18n).
