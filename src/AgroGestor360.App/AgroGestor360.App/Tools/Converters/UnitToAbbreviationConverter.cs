using System.Globalization;

namespace AgroGestor360.App.Tools.Converters;

public class UnitToAbbreviationConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        var theValue = value as string;
        if (!string.IsNullOrEmpty(theValue))
        {
            int pos1 = theValue.LastIndexOf('[');

            return theValue[(pos1 + 1)..(theValue.Length - 1)];
        }
        return theValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
