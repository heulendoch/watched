using Core.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Watched.Converter {

    public class ConverterEnumStaffelTextAEF : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            try {
                var enumValue = (StaffelAEF)value;
                switch(enumValue) {
                    case StaffelAEF.No: { return "Nein"; }
                    case StaffelAEF.Yes: { return "Ja"; }
                    case StaffelAEF.Partly: { return "Teilweise"; }
                }
            }
            catch(Exception ex) {
                Debug.WriteLine("ConverterEnumStaffelTextAEF: " + ex.Message);
            }

            return "???";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
