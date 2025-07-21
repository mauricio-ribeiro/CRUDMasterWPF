using CRUDMaster.Extensions;
using System.Globalization;
using System.Windows.Data;

namespace CRUDMaster.Converters
{
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (value is Enum enumValue)
            {
                return enumValue.GetDisplayName();
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
