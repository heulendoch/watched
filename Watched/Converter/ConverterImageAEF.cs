using Core.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Watched.Converter {

    public class ConverterImageAEF : IValueConverter {


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

            var Result = new BitmapImage();

            try {
                var enumValue = (StaffelAEF)value;
                switch(enumValue) {
                    case StaffelAEF.No: { Result = GetImage("pack://application:,,,/Resources/cross.png"); break; }
                    case StaffelAEF.Yes: { Result = GetImage("pack://application:,,,/Resources/ok.png"); break; }
                    case StaffelAEF.Partly: { Result = GetImage("pack://application:,,,/Resources/partly.png"); break; }
                }
            }
            catch(Exception ex) {
                Debug.WriteLine("ConverterImageAEF: " + ex.Message);
            }

            return Result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        private static BitmapImage GetImage(string PicPath) {
            BitmapImage Result = new BitmapImage();
            Result.BeginInit();
            Result.UriSource = new Uri(PicPath, UriKind.RelativeOrAbsolute);
            Result.EndInit();

            return Result;
        }

    }

}
