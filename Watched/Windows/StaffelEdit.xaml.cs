using Core.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Watched.Windows {

    /// <summary>
    /// Interaktionslogik für StaffelEdit.xaml
    /// </summary>
    public partial class StaffelEdit : Window {

        public StaffelEdit(Staffel Current = null) {
            InitializeComponent();
            this.DataContext = Current;
        }

        public object Return {
            get { return this.DataContext; }
        }

        private void ButtonSave(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
            this.Close();
        }
    }
}
