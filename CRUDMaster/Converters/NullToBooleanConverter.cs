using System.Globalization;
using System.Windows.Data;

namespace CRUDMaster.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Retorna true se o valor NÃO for nulo e ProdutoId for maior que 0
            return value != null && (value is int intValue ? intValue > 0 : true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
