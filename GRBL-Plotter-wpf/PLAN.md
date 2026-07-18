# GRBL-Plotter WPF Port Plan

Target folder: `GRBL-Plotter-wpf`  
Goal: WPF rewrite with matching core functionality + professional industrial theme.

## Scope (parity with WinForms 1.8.x)

| Area | WinForms source | WPF approach |
|------|-----------------|--------------|
| Shell / menus | `GUI/MainForm*` | `MainWindow` + ribbon-style menu |
| Serial / GRBL | `ControlSerialForm*`, `GrblRelated` | `GrblSerialService` + `SerialWindow` |
| Streaming | `UCStreaming` | `StreamingPanel` + VM |
| Overrides / Flow | `UCOverrides`, `UCFlowControl` | Panels + realtime cmds (`! ~ ? $X` Ctrl-X) |
| DRO / Zero | `UCDRO` | `DroPanel` |
| Jog | `UCJogControl*` | `JogPanel` |
| Origin / offsets | `UCSetOffset` | `OriginPanel` |
| Devices | Laser / Plotter / Router UCs | Tabbed `DevicePanel` |
| G-Code editor | FastColoredTextBox area | AvalonEdit or TextBox + line highlight |
| 2D workspace | `MainFormPictureBox` / VisuGCode | WPF `Canvas` / `Path` viewer |
| File import | Load G-code / SVG pipeline (phase) | G-code open first; SVG stub hooks |
| Theme | Pink/Yellow defaults | Industrial slate + teal |

## Phases

1. **Scaffold** — .NET 8 WPF solution, industrial ResourceDictionary, MainWindow chrome
2. **Machine I/O** — Serial ports, GRBL status parser (`<Idle|MPos|WPos|FS|Ov>`), send queue
3. **Operator panels** — Streaming, Flow, Overrides, DRO, Jog, Origin (left/center/right layout)
4. **Workspace** — Load `.nc/.gcode`, parse G0/G1 paths, draw 2D preview, editor sync
5. **Devices + polish** — Laser/Plotter/Router tabs, Home/Reset/Hold, status bar, splash brand image
6. **Build verify** — `dotnet build`, run smoke checklist

## Architecture

```
Views (XAML)  →  ViewModels (INotifyPropertyChanged)
                      ↓
              Services (Serial, Streamer, GCodeParser, Settings)
                      ↓
              Models (GrblState, AxisPosition, GCodeDocument)
```

## Status

- [x] Phase 1 — Scaffold + industrial theme + MainWindow shell
- [x] Phase 2 — Serial / GRBL status + COM window
- [x] Phase 3 — Streaming / DRO / Jog / Overrides / Origin
- [x] Phase 4 — G-code load + 2D preview
- [x] Phase 5 — Device tabs + menus + build verify
- [ ] Later — SVG/DXF import, Camera, HeightMap, full localization

## Industrial theme tokens

- Background: `#1A1F26` / `#232A33`
- Surface: `#2C343C`
- Accent teal: `#3D8B8A`
- Danger: `#D25F5F`
- Warning: `#E6C35C`
- Text: `#E8EEF2` / muted `#9AA7B2`
