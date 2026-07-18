using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GrblPlotter.Wpf.Converters;

/// <summary>Visible when the bound value is null (e.g. an "no image loaded yet" placeholder hint).</summary>
public sealed class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value == null ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

/// <summary>Converts a "#AARRGGBB"/"#RRGGBB" hex string (as used by <see cref="Services.ViewColorSettings"/>)
/// into a brush for small color-preview swatches.</summary>
public sealed class HexToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s && !string.IsNullOrWhiteSpace(s))
        {
            try
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(s));
            }
            catch
            {
                // fall through to default brush below
            }
        }
        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is SolidColorBrush b ? b.Color.ToString() : "#FF000000";
}
