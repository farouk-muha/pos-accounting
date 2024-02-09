using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace POSAccounting.Utils
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class FlopBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if ((bool)value) return Properties.Resources.Yes;
                return Properties.Resources.No;
            }
            catch
            {
                return value;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return true;
        }
    }
}
