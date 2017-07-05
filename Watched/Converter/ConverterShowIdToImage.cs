using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TMDbLib.Objects.General;
using TMDbLib.Objects.TvShows;

namespace Watched.Converter {
    class ConverterShowIdToImage : IValueConverter {

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

            int? Result = value as int?;

            if (value != null) {
                try {

                    if (!Caching.ImagesTvShowSearch.ContainsKey((int)Result)) {

                        ImagesWithId Bla = ApiHelper.Global.Client.GetTvShowImages((int)Result);

                        if (Bla.Posters.Count > 0) {
                            Uri ImgUri = ApiHelper.Global.Client.GetImageUrl("w92", Bla.Posters[0].FilePath);
                            BitmapImage Temp = new BitmapImage(ImgUri);

                            Caching.ImagesTvShowSearch.Add((int)Result, Temp);

                            return Temp;
                        }
                    }
                    else { 
                        return Caching.ImagesTvShowSearch[(int)Result];
                    }

                }
                catch (Exception e) {
                    Debug.WriteLine(e.Message);
                }
            }

            return new BitmapImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
