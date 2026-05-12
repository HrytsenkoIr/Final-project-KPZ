using System.Globalization;
using System.Windows.Data;

namespace ReminderNotebook.Converters;

public class DateTimeFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DateTime dt) return string.Empty;

        var format = parameter as string ?? "dd.MM.yyyy HH:mm";
        return dt.ToString(format, new CultureInfo("uk-UA"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}