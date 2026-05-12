using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReminderNotebook.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Активне" => new SolidColorBrush(Color.FromRgb(76, 175, 80)),
            "Прострочено" => new SolidColorBrush(Color.FromRgb(244, 67, 54)),
            "Спрацювало" => new SolidColorBrush(Color.FromRgb(158, 158, 158)),
            _ => new SolidColorBrush(Color.FromRgb(96, 125, 139))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}