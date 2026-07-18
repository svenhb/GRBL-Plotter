using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>
/// Very small SVG importer: flattens &lt;path&gt;, &lt;polyline&gt;, &lt;polygon&gt;,
/// &lt;rect&gt;, &lt;circle&gt;, &lt;ellipse&gt; and &lt;line&gt; elements into straight
/// line segments (curves are approximated by sampling several points along them)
/// and emits absolute, metric G-code. Unsupported/unknown elements are skipped.
/// </summary>
public static class SvgImporter
{
    private const double PxToMm = 25.4 / 96.0;
    private static readonly Regex NumRx = new(@"[-+]?(?:\d+\.?\d*|\.\d+)(?:[eE][-+]?\d+)?", RegexOptions.Compiled);

    public static GCodeDocument Load(string path) => Parse(File.ReadAllText(path), path);

    public static GCodeDocument Parse(string svgContent, string? path = null)
    {
        var writer = new GCodeWriter();
        writer.Header("SVG import");

        try
        {
            var xdoc = XDocument.Parse(svgContent);
            var root = xdoc.Root;
            if (root != null)
            {
                var view = ViewBoxOf(root);
                WalkChildren(root, writer, view, AffineTransform.Identity);
            }
        }
        catch (Exception ex)
        {
            writer.Comment($"SVG parse error: {ex.Message}");
        }

        writer.Footer();
        return GCodeParser.Parse(writer.ToText(), path);
    }

    private readonly record struct ViewBoxInfo(double MinX, double MinY, double ScaleX, double ScaleY, double HeightMm);

    private static ViewBoxInfo ViewBoxOf(XElement svg)
    {
        double vbMinX = 0, vbMinY = 0, vbW = 0, vbH = 0;
        var viewBox = (string?)svg.Attribute("viewBox");
        bool hasViewBox = false;
        if (!string.IsNullOrWhiteSpace(viewBox))
        {
            var nums = NumRx.Matches(viewBox).Select(m => double.Parse(m.Value, CultureInfo.InvariantCulture)).ToArray();
            if (nums.Length == 4)
            {
                vbMinX = nums[0]; vbMinY = nums[1]; vbW = nums[2]; vbH = nums[3];
                hasViewBox = true;
            }
        }

        double widthMm = LengthToMm((string?)svg.Attribute("width"), hasViewBox ? vbW : 0);
        double heightMm = LengthToMm((string?)svg.Attribute("height"), hasViewBox ? vbH : 0);

        if (!hasViewBox)
        {
            // No viewBox: treat document units directly as px (96dpi) and there is no offset.
            vbW = widthMm > 0 ? widthMm / PxToMm : 1000;
            vbH = heightMm > 0 ? heightMm / PxToMm : 1000;
            if (widthMm <= 0) widthMm = vbW * PxToMm;
            if (heightMm <= 0) heightMm = vbH * PxToMm;
        }
        else
        {
            if (widthMm <= 0) widthMm = vbW * PxToMm;
            if (heightMm <= 0) heightMm = vbH * PxToMm;
        }

        double scaleX = vbW > 0 ? widthMm / vbW : PxToMm;
        double scaleY = vbH > 0 ? heightMm / vbH : PxToMm;
        return new ViewBoxInfo(vbMinX, vbMinY, scaleX, scaleY, heightMm);
    }

    private static double LengthToMm(string? raw, double fallbackUserUnits)
    {
        if (string.IsNullOrWhiteSpace(raw)) return 0;
        var m = Regex.Match(raw.Trim(), @"([-+]?[0-9.]+)\s*([a-z%]*)", RegexOptions.IgnoreCase);
        if (!m.Success) return 0;
        double val = double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
        var unit = m.Groups[2].Value.ToLowerInvariant();
        return unit switch
        {
            "mm" => val,
            "cm" => val * 10.0,
            "in" => val * 25.4,
            "pt" => val * 25.4 / 72.0,
            "px" or "" => val * PxToMm,
            _ => val * PxToMm
        };
    }

    private static (double X, double Y) ToMm(double x, double y, ViewBoxInfo v, AffineTransform t)
    {
        var (tx, ty) = t.Apply(x, y);
        double mmX = (tx - v.MinX) * v.ScaleX;
        double mmY = v.HeightMm - (ty - v.MinY) * v.ScaleY; // flip so Y grows upward for CNC
        return (mmX, mmY);
    }

    private static void WalkChildren(XElement parent, GCodeWriter writer, ViewBoxInfo view, AffineTransform inherited)
    {
        foreach (var el in parent.Elements())
        {
            var name = el.Name.LocalName.ToLowerInvariant();
            var local = inherited.Multiply(ParseTransform((string?)el.Attribute("transform")));

            try
            {
                switch (name)
                {
                    case "path":
                        DrawPath((string?)el.Attribute("d") ?? "", writer, view, local);
                        break;
                    case "polyline":
                    case "polygon":
                        DrawPoly((string?)el.Attribute("points") ?? "", writer, view, local, closed: name == "polygon");
                        break;
                    case "rect":
                        DrawRect(el, writer, view, local);
                        break;
                    case "circle":
                        DrawEllipse(el, writer, view, local, isCircle: true);
                        break;
                    case "ellipse":
                        DrawEllipse(el, writer, view, local, isCircle: false);
                        break;
                    case "line":
                        DrawLine(el, writer, view, local);
                        break;
                    case "g":
                    case "svg":
                    case "a":
                        WalkChildren(el, writer, view, local);
                        break;
                    default:
                        // defs, style, text, metadata, image, use, etc. - ignore gracefully
                        break;
                }
            }
            catch
            {
                // Ignore malformed individual elements, keep importing the rest.
            }

            if (name is "g" or "svg" or "a") continue;
        }
    }

    private static void DrawRect(XElement el, GCodeWriter writer, ViewBoxInfo view, AffineTransform t)
    {
        double x = Attr(el, "x"), y = Attr(el, "y");
        double w = Attr(el, "width"), h = Attr(el, "height");
        if (w <= 0 || h <= 0) return;
        var pts = new List<(double X, double Y)>
        {
            ToMm(x, y, view, t), ToMm(x + w, y, view, t), ToMm(x + w, y + h, view, t), ToMm(x, y + h, view, t)
        };
        writer.DrawPolyline(pts, closed: true);
    }

    private static void DrawEllipse(XElement el, GCodeWriter writer, ViewBoxInfo view, AffineTransform t, bool isCircle)
    {
        double cx = Attr(el, "cx"), cy = Attr(el, "cy");
        double rx = isCircle ? Attr(el, "r") : Attr(el, "rx");
        double ry = isCircle ? rx : Attr(el, "ry");
        if (rx <= 0 || ry <= 0) return;

        const int segs = 36;
        var pts = new List<(double X, double Y)>(segs);
        for (int i = 0; i < segs; i++)
        {
            double a = 2 * Math.PI * i / segs;
            pts.Add(ToMm(cx + rx * Math.Cos(a), cy + ry * Math.Sin(a), view, t));
        }
        writer.DrawPolyline(pts, closed: true);
    }

    private static void DrawLine(XElement el, GCodeWriter writer, ViewBoxInfo view, AffineTransform t)
    {
        double x1 = Attr(el, "x1"), y1 = Attr(el, "y1"), x2 = Attr(el, "x2"), y2 = Attr(el, "y2");
        writer.DrawPolyline(new List<(double X, double Y)> { ToMm(x1, y1, view, t), ToMm(x2, y2, view, t) });
    }

    private static double Attr(XElement el, string name)
    {
        var s = (string?)el.Attribute(name);
        return string.IsNullOrWhiteSpace(s) ? 0 : double.Parse(NumRx.Match(s).Value, CultureInfo.InvariantCulture);
    }

    private static void DrawPoly(string points, GCodeWriter writer, ViewBoxInfo view, AffineTransform t, bool closed)
    {
        var nums = NumRx.Matches(points).Select(m => double.Parse(m.Value, CultureInfo.InvariantCulture)).ToArray();
        var pts = new List<(double X, double Y)>();
        for (int i = 0; i + 1 < nums.Length; i += 2)
            pts.Add(ToMm(nums[i], nums[i + 1], view, t));
        if (pts.Count > 0) writer.DrawPolyline(pts, closed);
    }

    // --- SVG path ('d') mini-parser -----------------------------------------------------

    private static void DrawPath(string d, GCodeWriter writer, ViewBoxInfo view, AffineTransform t)
    {
        var tokens = TokenizePath(d);
        int i = 0;
        double cx = 0, cy = 0;      // current point (user units)
        double startX = 0, startY = 0; // subpath start, for Z
        var current = new List<(double X, double Y)>();
        char cmd = 'M';

        void Flush(bool closed)
        {
            if (current.Count > 0) writer.DrawPolyline(current, closed);
            current = new List<(double X, double Y)>();
        }

        while (i < tokens.Count)
        {
            var tok = tokens[i];
            if (IsCommandLetter(tok))
            {
                cmd = tok[0];
                i++;
                continue;
            }

            bool rel = char.IsLower(cmd);
            char up = char.ToUpperInvariant(cmd);

            switch (up)
            {
                case 'M':
                {
                    Flush(false);
                    double nx = ReadNum(tokens, ref i), ny = ReadNum(tokens, ref i);
                    cx = rel ? cx + nx : nx; cy = rel ? cy + ny : ny;
                    startX = cx; startY = cy;
                    current.Add(ToMm(cx, cy, view, t));
                    break;
                }
                case 'L':
                {
                    double nx = ReadNum(tokens, ref i), ny = ReadNum(tokens, ref i);
                    cx = rel ? cx + nx : nx; cy = rel ? cy + ny : ny;
                    current.Add(ToMm(cx, cy, view, t));
                    break;
                }
                case 'H':
                {
                    double nx = ReadNum(tokens, ref i);
                    cx = rel ? cx + nx : nx;
                    current.Add(ToMm(cx, cy, view, t));
                    break;
                }
                case 'V':
                {
                    double ny = ReadNum(tokens, ref i);
                    cy = rel ? cy + ny : ny;
                    current.Add(ToMm(cx, cy, view, t));
                    break;
                }
                case 'C':
                {
                    double x1 = ReadNum(tokens, ref i), y1 = ReadNum(tokens, ref i);
                    double x2 = ReadNum(tokens, ref i), y2 = ReadNum(tokens, ref i);
                    double ex = ReadNum(tokens, ref i), ey = ReadNum(tokens, ref i);
                    if (rel) { x1 += cx; y1 += cy; x2 += cx; y2 += cy; ex += cx; ey += cy; }
                    foreach (var p in FlattenCubic(cx, cy, x1, y1, x2, y2, ex, ey))
                        current.Add(ToMm(p.X, p.Y, view, t));
                    cx = ex; cy = ey;
                    break;
                }
                case 'S':
                {
                    // Smooth cubic without proper reflection of previous control point - approximate using endpoint as both controls.
                    double x2 = ReadNum(tokens, ref i), y2 = ReadNum(tokens, ref i);
                    double ex = ReadNum(tokens, ref i), ey = ReadNum(tokens, ref i);
                    if (rel) { x2 += cx; y2 += cy; ex += cx; ey += cy; }
                    foreach (var p in FlattenCubic(cx, cy, x2, y2, x2, y2, ex, ey))
                        current.Add(ToMm(p.X, p.Y, view, t));
                    cx = ex; cy = ey;
                    break;
                }
                case 'Q':
                {
                    double x1 = ReadNum(tokens, ref i), y1 = ReadNum(tokens, ref i);
                    double ex = ReadNum(tokens, ref i), ey = ReadNum(tokens, ref i);
                    if (rel) { x1 += cx; y1 += cy; ex += cx; ey += cy; }
                    foreach (var p in FlattenQuadratic(cx, cy, x1, y1, ex, ey))
                        current.Add(ToMm(p.X, p.Y, view, t));
                    cx = ex; cy = ey;
                    break;
                }
                case 'T':
                {
                    double ex = ReadNum(tokens, ref i), ey = ReadNum(tokens, ref i);
                    if (rel) { ex += cx; ey += cy; }
                    current.Add(ToMm(ex, ey, view, t));
                    cx = ex; cy = ey;
                    break;
                }
                case 'A':
                {
                    ReadNum(tokens, ref i); ReadNum(tokens, ref i); // rx, ry (approximated as line)
                    ReadNum(tokens, ref i); // x-axis-rotation
                    ReadNum(tokens, ref i); // large-arc-flag
                    ReadNum(tokens, ref i); // sweep-flag
                    double ex = ReadNum(tokens, ref i), ey = ReadNum(tokens, ref i);
                    if (rel) { ex += cx; ey += cy; }
                    current.Add(ToMm(ex, ey, view, t));
                    cx = ex; cy = ey;
                    break;
                }
                case 'Z':
                {
                    cx = startX; cy = startY;
                    Flush(true);
                    break;
                }
                default:
                    i++; // unknown command - skip token to avoid infinite loop
                    break;
            }
        }

        Flush(false);
    }

    private static bool IsCommandLetter(string tok) =>
        tok.Length == 1 && "MmLlHhVvCcSsQqTtAaZz".Contains(tok[0]);

    private static double ReadNum(List<string> tokens, ref int i)
    {
        if (i >= tokens.Count) return 0;
        var v = double.Parse(tokens[i], CultureInfo.InvariantCulture);
        i++;
        return v;
    }

    private static List<string> TokenizePath(string d)
    {
        var result = new List<string>();
        var rx = new Regex(@"[MmLlHhVvCcSsQqTtAaZz]|[-+]?(?:\d+\.?\d*|\.\d+)(?:[eE][-+]?\d+)?");
        foreach (Match m in rx.Matches(d)) result.Add(m.Value);
        return result;
    }

    private static IEnumerable<(double X, double Y)> FlattenCubic(double x0, double y0, double x1, double y1, double x2, double y2, double x3, double y3, int segments = 12)
    {
        for (int s = 1; s <= segments; s++)
        {
            double t = (double)s / segments;
            double mt = 1 - t;
            double x = mt * mt * mt * x0 + 3 * mt * mt * t * x1 + 3 * mt * t * t * x2 + t * t * t * x3;
            double y = mt * mt * mt * y0 + 3 * mt * mt * t * y1 + 3 * mt * t * t * y2 + t * t * t * y3;
            yield return (x, y);
        }
    }

    private static IEnumerable<(double X, double Y)> FlattenQuadratic(double x0, double y0, double x1, double y1, double x2, double y2, int segments = 10)
    {
        for (int s = 1; s <= segments; s++)
        {
            double t = (double)s / segments;
            double mt = 1 - t;
            double x = mt * mt * x0 + 2 * mt * t * x1 + t * t * x2;
            double y = mt * mt * y0 + 2 * mt * t * y1 + t * t * y2;
            yield return (x, y);
        }
    }

    // --- transform="translate(..) scale(..) matrix(..) rotate(..)" (subset) --------------

    private readonly record struct AffineTransform(double A, double B, double C, double D, double E, double F)
    {
        public static readonly AffineTransform Identity = new(1, 0, 0, 1, 0, 0);

        public (double X, double Y) Apply(double x, double y) => (A * x + C * y + E, B * x + D * y + F);

        public AffineTransform Multiply(AffineTransform o) => new(
            A * o.A + C * o.B, B * o.A + D * o.B,
            A * o.C + C * o.D, B * o.C + D * o.D,
            A * o.E + C * o.F + E, B * o.E + D * o.F + F);
    }

    private static AffineTransform ParseTransform(string? raw)
    {
        var result = AffineTransform.Identity;
        if (string.IsNullOrWhiteSpace(raw)) return result;

        foreach (Match m in Regex.Matches(raw, @"(\w+)\s*\(([^)]*)\)"))
        {
            var fn = m.Groups[1].Value.ToLowerInvariant();
            var nums = NumRx.Matches(m.Groups[2].Value).Select(x => double.Parse(x.Value, CultureInfo.InvariantCulture)).ToArray();
            AffineTransform t;
            switch (fn)
            {
                case "translate":
                    t = new AffineTransform(1, 0, 0, 1, nums.Length > 0 ? nums[0] : 0, nums.Length > 1 ? nums[1] : 0);
                    break;
                case "scale":
                    double sx = nums.Length > 0 ? nums[0] : 1;
                    double sy = nums.Length > 1 ? nums[1] : sx;
                    t = new AffineTransform(sx, 0, 0, sy, 0, 0);
                    break;
                case "rotate" when nums.Length >= 1:
                    double a = nums[0] * Math.PI / 180.0;
                    t = new AffineTransform(Math.Cos(a), Math.Sin(a), -Math.Sin(a), Math.Cos(a), 0, 0);
                    break;
                case "matrix" when nums.Length == 6:
                    t = new AffineTransform(nums[0], nums[1], nums[2], nums[3], nums[4], nums[5]);
                    break;
                default:
                    t = AffineTransform.Identity;
                    break;
            }
            result = result.Multiply(t);
        }
        return result;
    }
}
