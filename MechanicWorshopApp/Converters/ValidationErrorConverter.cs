using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace MechanicWorkshopApp.Converters
{
    public class ValidationErrorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Collections.IEnumerable errors)
            {
                var firstError = errors.Cast<ValidationError>().FirstOrDefault();
                return firstError?.ErrorContent; // Devuelve el contenido del primer error, si existe
            }
            return null; // No hay errores
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}