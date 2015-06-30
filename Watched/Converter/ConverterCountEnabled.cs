using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Watched.Converter {
    
    /// <summary>
    /// Umwandlung von 
    /// <para>Für WPF Integration</para>
    /// </summary>
    public class ConverterCountEnabled : IValueConverter {

        #region IValueConverter Implementierungen

        /// <returns>Das formatierte Datum</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

            bool Return = false;

            if (value != null) {
                try {
                    long TParse = long.MinValue;
                    if (long.TryParse(value.ToString(), out TParse)) {
                        Return = TParse > 0;
                    }
                }
                catch (Exception) { }
            }

            return Return;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion

    }
}
