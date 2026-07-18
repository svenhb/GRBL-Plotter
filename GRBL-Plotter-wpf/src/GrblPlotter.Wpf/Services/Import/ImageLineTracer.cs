using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>
/// Loads a bitmap (PNG/JPG/BMP/...), converts it to 8-bit grayscale using
/// WPF imaging (no System.Drawing dependency), thresholds it to mono, and
/// engraves it as horizontal scan lines wherever dark ("on") pixel runs are
/// found - a simple, dependency-free raster-to-G-code line tracer.
/// </summary>
public static class ImageLineTracer
{
    public static GCodeDocument Load(
        string path,
        double targetWidthMm = 100,
        double? targetHeightMm = null,
        int threshold = 128,
        bool invert = false,
        double lineSpacingMm = 0.3,
        bool boustrophedon = true)
    {
        using var stream = File.OpenRead(path);
        return Trace(stream, path, targetWidthMm, targetHeightMm, threshold, invert, lineSpacingMm, boustrophedon);
    }

    public static GCodeDocument Trace(
        Stream imageStream,
        string? path,
        double targetWidthMm = 100,
        double? targetHeightMm = null,
        int threshold = 128,
        bool invert = false,
        double lineSpacingMm = 0.3,
        bool boustrophedon = true)
    {
        var writer = new GCodeWriter();
        writer.Header("Image line-trace import");

        try
        {
            var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            var frame = decoder.Frames[0];
            var gray = new FormatConvertedBitmap(frame, PixelFormats.Gray8, null, 0);

            int w = gray.PixelWidth, h = gray.PixelHeight;
            if (w <= 0 || h <= 0)
            {
                writer.Comment("Image has no pixels.");
                writer.Footer();
                return GCodeParser.Parse(writer.ToText(), path);
            }

            int stride = w;
            var pixels = new byte[stride * h];
            gray.CopyPixels(pixels, stride, 0);

            double heightMm = targetHeightMm ?? targetWidthMm * h / w;
            double scaleX = targetWidthMm / w;
            double scaleY = heightMm / h;

            int rowStep = Math.Max(1, (int)Math.Round(lineSpacingMm / scaleY));

            bool leftToRight = true;
            for (int row = 0; row < h; row += rowStep)
            {
                int rowOffset = row * stride;
                double ymm = heightMm - row * scaleY;

                var runs = FindDarkRuns(pixels, rowOffset, w, threshold, invert);
                if (!boustrophedon || leftToRight)
                {
                    foreach (var (start, end) in runs)
                        writer.DrawPolyline(new List<(double X, double Y)> { (start * scaleX, ymm), (end * scaleX, ymm) });
                }
                else
                {
                    for (int i = runs.Count - 1; i >= 0; i--)
                    {
                        var (start, end) = runs[i];
                        writer.DrawPolyline(new List<(double X, double Y)> { (end * scaleX, ymm), (start * scaleX, ymm) });
                    }
                }
                leftToRight = !leftToRight;
            }
        }
        catch (Exception ex)
        {
            writer.Comment($"Image import error: {ex.Message}");
        }

        writer.Footer();
        return GCodeParser.Parse(writer.ToText(), path);
    }

    private static List<(int Start, int End)> FindDarkRuns(byte[] pixels, int rowOffset, int width, int threshold, bool invert)
    {
        var runs = new List<(int Start, int End)>();
        int runStart = -1;
        for (int x = 0; x < width; x++)
        {
            byte v = pixels[rowOffset + x];
            bool dark = invert ? v >= threshold : v < threshold;
            if (dark)
            {
                if (runStart < 0) runStart = x;
            }
            else if (runStart >= 0)
            {
                runs.Add((runStart, x));
                runStart = -1;
            }
        }
        if (runStart >= 0) runs.Add((runStart, width));
        return runs;
    }
}
