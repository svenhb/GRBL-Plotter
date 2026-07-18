using System.Globalization;
using System.Windows;

namespace GrblPlotter.Wpf.Services;

/// <summary>en / zh-CN / de-DE ResourceDictionary switch (Wave D).</summary>
public static class LocalizationService
{
    public static string Current { get; private set; } = "en";

    /// <summary>Lookup a <c>Str.*</c> resource; returns <paramref name="fallback"/> if missing.</summary>
    public static string Get(string key, string? fallback = null)
    {
        try
        {
            var obj = Application.Current?.TryFindResource(key);
            if (obj is string s && !string.IsNullOrEmpty(s))
                return s;
        }
        catch { /* ignore */ }
        return fallback ?? key;
    }

    public static void Apply(string culture)
    {
        var c = (culture ?? "en").Trim().ToLowerInvariant();
        Current = c.StartsWith("zh") ? "zh" : c.StartsWith("de") ? "de" : "en";
        var dict = new ResourceDictionary
        {
            Source = new Uri($"Themes/Strings.{Current}.xaml", UriKind.Relative)
        };
        var app = Application.Current;
        for (int i = app.Resources.MergedDictionaries.Count - 1; i >= 0; i--)
        {
            var src = app.Resources.MergedDictionaries[i].Source?.OriginalString ?? "";
            if (src.Contains("Strings.", StringComparison.OrdinalIgnoreCase))
                app.Resources.MergedDictionaries.RemoveAt(i);
        }
        app.Resources.MergedDictionaries.Insert(0, dict);
        try
        {
            CultureInfo.CurrentUICulture = Current switch
            {
                "zh" => new CultureInfo("zh-CN"),
                "de" => new CultureInfo("de-DE"),
                _ => new CultureInfo("en-US")
            };
        }
        catch { /* ignore */ }
    }
}
