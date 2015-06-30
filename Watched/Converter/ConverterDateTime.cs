using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Watched.Converter {
    
    /// <summary>
    /// Umwandlung von DateTime
    /// <para>Für WPF Integration</para>
    /// </summary>
    public class ConverterDateTime : IValueConverter {

        #region IValueConverter Implementierungen

        /// <summary>
        /// Wandelt das den genannten DateTime-Objekt um
        /// </summary>
        /// <param name="value">Wert</param>
        /// <param name="targetType">Der sendente Type</param>
        /// <param name="parameter">[Es gibt keine Parameter]</param>
        /// <param name="culture"></param>
        /// <returns>Das formatierte Datum</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

            DateTime Result = new DateTime();

            if (value != null) {
                try {

                    if (value.GetType() == typeof(DateTime)) {
                        Result = (DateTime)value;
                    }
                }
                catch (Exception) { }
            }

            return string.Format("{0}", Result.ToShortDateString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion

    }
}
