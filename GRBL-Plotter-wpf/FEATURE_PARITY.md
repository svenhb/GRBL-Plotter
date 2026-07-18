# GRBL-Plotter WPF — Feature Parity Map

复检日期：2026-07-18（Full Parity Restore Waves A–D）

## 一致性总览

| 等级 | 含义 | 估计覆盖 |
|------|------|----------|
| **对齐** | 主路径可用，入口与行为对应 | ~90% 日常工作流 |
| **技术替代** | 同功能，实现栈不同（允许） | AvalonEdit、StreamGeometry、JSON 设置 |
| **刻意简化** | 计划保留差异 | Control Streaming 旧窗、Setup 像素布局、INI |

日常 CNC（打开→变换→预览→串口→流送→jog/zero/A–C）已可走通；编辑器 XML 折叠/排序与 View 叠加已接上。

---

## Wave A–D 交付对照

| 区域 | 状态 | 备注 |
|------|------|------|
| 2D 右键：Reload / Paste / Reset zoom / Path props / Offset marker→0·tool / Apply last / Copy preview / Mark first | **对齐** | `MainViewModel.Parity` + ContextMenu |
| Mirror Rotary / Scale Y→rotary / RadiusCompensation | **对齐** | `GCodeTransformService` |
| A/B/C DRO · Zero · Jog（按 AxisCount） | **对齐** | Setup → Axes |
| DIY Control Pad 独立窗 | **对齐** | `DiyControlPadWindow` |
| Custom 1–16 Setup + 主面板 | **对齐** | Setup「Custom 1–16」 |
| Hershey + LFF 字库 | **对齐** | `data/fonts` + `LffFontLoader` |
| Hatch / Tangential | **对齐** | 菜单 + Setup Import 自动应用 |
| View：Ruler / Info HUD / Limits / Fixed area / Tool overlay | **对齐** | `PreviewOverlayBuilder` |
| Zoom / Pan / Reset zoom | **对齐** | 滚轮 + Shift 拖拽 |
| AvalonEdit + XmlMarker 折叠/排序/Remove tags | **技术替代** | AvalonEdit FoldingManager |
| Find/Replace · PenUp/Down · Comment unknown · Send line | **对齐** | 编辑器右键 |
| Setup 深度（Axes/Import/Custom/Streaming/2D/Devices） | **对齐**（WPF 布局） | 非 WinForms 像素搬家 |
| i18n en / zh-CN / de-DE | **对齐**（首批） | `Themes/Strings.*.xaml` |
| Control Streaming 独立窗 | **刻意简化** | 主面板覆盖 |
| 设置 INI | **刻意简化** | JSON `AppSettings` |

## 技术替代项（计划允许）

| WinForms | WPF |
|----------|-----|
| PictureBox + GDI `GraphicsPath` | `StreamGeometry` / Canvas `Path` |
| FastColoredTextBox 折叠 | AvalonEdit + `XmlMarkerService` |
| Designer `.resx` 坐标 | 仅字符串资源 en/zh/de |

## Run

```bat
dotnet run --project src\GrblPlotter.Wpf\GrblPlotter.Wpf.csproj -c Debug
```
