using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TMDbLib.Objects.TvShows;

namespace Watched.Windows {
    /// <summary>
    /// Interaktionslogik für Search.xaml
    /// </summary>
    public partial class Search : Window {


        private int m_ShowId;


        public Search() {
            InitializeComponent();
        }

        public int ShowId {
            get { return m_ShowId; }
            private set { m_ShowId = value; }
        }


        private void AddSerie(object sender, RoutedEventArgs e) {
            int? Current = (int?)(sender as FrameworkElement).Tag;
            if (Current != null && MessageBox.Show("Serie wirklich hinzufügen?", "Serie hinzufügen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                this.DialogResult = true;
                this.ShowId = (int)Current;
                this.Close();
            }
        }

        private void SearchSerienname(object sender, TextChangedEventArgs e) {

            string Query = this.txtSearchSerienname.Text;

            if (Query.Length <= 2) {
                this.lvSerien.ItemsSource = new List<TvShowBase>();
            }
            else{
                try {
                    List<TvShowBase> SearchResult = ApiHelper.Global.Search(Query);
                    this.lvSerien.Dispatcher.Invoke(new Action(delegate {
                        this.lvSerien.ItemsSource = SearchResult;
                    }));
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Fehlgeschlagene Anfrage an Server.");
                }

            }

        }
    }

}
