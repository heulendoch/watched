using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Core.Units;

namespace Watched.Windows {
    /// <summary>
    /// Interaktionslogik für FolgeEdit.xaml
    /// </summary>
    public partial class FolgeEdit : Window {
        public FolgeEdit(Folge Current = null) {
            InitializeComponent();

            this.DataContext = Current;
        }


        public object Return {
            get { return this.DataContext; }
        }

        private void RemoveItemZeitpunktGesehen(object sender, MouseButtonEventArgs e) {
            var Send = sender as FrameworkElement;
            if (Send != null) {
                var Zeitpunkt = (DateTime)Send.DataContext;
                ((ObservableCollection<DateTime>)lbZeitpunkte.ItemsSource).Remove(Zeitpunkt);
            }
        }

        private void AddItemZeitpunktGesehen(object sender, RoutedEventArgs e) {
            DateTime Parsed = DateTime.MinValue;
            if (DateTime.TryParse(this.tbZeitpunktGesehen.Text, out Parsed)) {
                ((ObservableCollection<DateTime>)lbZeitpunkte.ItemsSource).Add(Parsed);
                this.tbZeitpunktGesehen.Text = string.Empty;
            }
        }

        private void ButtonSave(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
            this.Close();
        }


    }
}

