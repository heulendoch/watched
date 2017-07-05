using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Watched.Converter {

    public class ConverterBoolToYesNo : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool booleanValue = (bool)value;
            return booleanValue ? "Ja" : "Nein";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }
    }
}
