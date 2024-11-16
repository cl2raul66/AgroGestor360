using System.Globalization;

namespace AgroGestor360App.Tools.Converters;

public class MerchandiseCategoryToHumanIdConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        var category = (string)value;
        if (string.IsNullOrEmpty(category))
        {
            return category;
        }

        var section = category.Split(" ").Select(x => x[..3]);
        return string.Join("-", section);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
