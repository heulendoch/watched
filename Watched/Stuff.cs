using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Watched {
    public class Stuff {

        public static MessageBoxResult AskForSave() {
            return MessageBox.Show("Sie haben Änderungen vorgenommen möchten sie diese speichern?", "Änderungen verwerfen?", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}
