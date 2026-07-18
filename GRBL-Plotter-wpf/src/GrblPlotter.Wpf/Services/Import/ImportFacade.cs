using System.IO;
using GrblPlotter.Wpf.Models;

namespace GrblPlotter.Wpf.Services.Import;

/// <summary>Single entry point that detects a file's type by extension and routes it to the matching importer.</summary>
public static class ImportFacade
{
    public static GCodeDocument OpenAny(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        switch (ext)
        {
            case ".svg":
                return SvgImporter.Load(path);
            case ".dxf":
                return DxfImporter.Load(path);
            case ".hpgl":
            case ".plt":
                return HpglImporter.Load(path);
            case ".gbr":
            case ".ger":
                return GerberImporter.Load(path);
            case ".csv":
                return CsvDrillImporter.Load(path);
            case ".png":
            case ".jpg":
            case ".jpeg":
            case ".bmp":
            case ".gif":
            case ".tif":
            case ".tiff":
                return ImageLineTracer.Load(path);
            case ".nc":
            case ".gcode":
            case ".ngc":
            case ".tap":
                return GCodeParser.LoadFile(path);
            case ".txt":
                return OpenTextGuess(path);
            default:
                // Unknown extension: assume it is plain G-code text.
                return GCodeParser.LoadFile(path);
        }
    }

    /// <summary>".txt" is ambiguous - try drill/CSV first, and fall back to plain G-code text if that yields nothing.</summary>
    private static GCodeDocument OpenTextGuess(string path)
    {
        var asDrill = CsvDrillImporter.Load(path);
        if (asDrill.Segments.Count > 0) return asDrill;
        return GCodeParser.LoadFile(path);
    }
}
