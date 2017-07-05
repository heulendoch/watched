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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Watched.Windows;
using Core.Units;
using Core;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Watched {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();

            ObservableCollection<Serie> Serien = new ObservableCollection<Serie>();

            this.ShowLoading();

            BackgroundWorker Loading = new BackgroundWorker();

            Loading.DoWork += delegate {

                if (!XmlStuff.Load(ref Serien)) {
                    MessageBox.Show("Laden fehlgeschlagen.");
                }


                this.lvSerien.Dispatcher.Invoke(new Action(delegate {

                    this.lvSerien.ItemsSource = Serien;

                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvSerien.ItemsSource);
                    view.Filter = UserFilter;

                }));
            };

            Loading.RunWorkerCompleted += delegate {
                this.HideLoading();
            };

            Loading.RunWorkerAsync();

            this.txtFilterName.Focus();
        }

        private bool UserFilter(object obj) {
            Serie SObj = (Serie)obj;

            if (SObj == null)
                return false;

            if (string.IsNullOrWhiteSpace(this.txtFilterName.Text))
                return true;
            
            if(SObj.Name.ToLower().Contains(this.txtFilterName.Text.ToLower()))
                return true;

            return false;
        }

        #region XML



        #endregion


        private void EditStaffeln(object sender, MouseButtonEventArgs e) {
            FrameworkElement CurrentSender = (FrameworkElement)sender;
            this.EditStaffeln((Serie)CurrentSender.Tag);
        }

        private void EditStaffelnDoubleClick(object sender, MouseButtonEventArgs e) {

            bool Ignore = false;
            if (e.OriginalSource is FrameworkElement) {
                var OrgSource = (FrameworkElement)e.OriginalSource;

                if(OrgSource.Tag != null)
                    Ignore = OrgSource.Tag.ToString() == "Ignore1";
            }

            if (!e.Handled && !Ignore) {
                FrameworkElement CurrentSender = (FrameworkElement)sender;
                this.EditStaffeln((Serie)this.lvSerien.SelectedItem);
            }
        }

        private void EditStaffeln(Serie Current) {

            if (Current == null)
                return;

            SerieStaffelEdit Edit = new SerieStaffelEdit(Current.Staffeln.Clone(), Current.Name);

            if ((bool)Edit.ShowDialog()) {
                Current.Staffeln = (ObservableCollection<Staffel>)Edit.Return;
            }
        }

        private void ProofSavingRequired(object sender, System.ComponentModel.CancelEventArgs e) {
            Collection<Serie> Serien = (Collection<Serie>)this.lvSerien.ItemsSource;
            
            if (XmlStuff.SaveRequired(Serien)) {
                if (Stuff.AskForSave() == MessageBoxResult.Yes) {
                    XmlStuff.Save(Serien);
                }
            }
        }

        private void ButtonSave(object sender, RoutedEventArgs e) {
            XmlStuff.Save((ObservableCollection<Serie>)this.lvSerien.ItemsSource);
        }



        private void AddSerie(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrWhiteSpace(this.tbSerieName.Text)) {
                ((ObservableCollection<Serie>)this.lvSerien.ItemsSource).Add(new Serie(this.tbSerieName.Text));
                this.tbSerieName.Text = string.Empty;
            }
        }

        private void DeleteSerie(object sender, MouseButtonEventArgs e) {
            FrameworkElement CurrentSender = (FrameworkElement)sender;
            Serie Current = (Serie)CurrentSender.Tag;

            if (MessageBox.Show("Wirklich löschen?", "Löschen?", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                ((ObservableCollection<Serie>)this.lvSerien.ItemsSource).Remove(Current);
            }
        }

        private void txtFilterName_TextChanged(object sender, TextChangedEventArgs e) {
            CollectionViewSource.GetDefaultView(this.lvSerien.ItemsSource).Refresh();
        }

        private void Search(object sender, RoutedEventArgs e) {
            try {
                Search Dialog = new Search();
                if ((bool)Dialog.ShowDialog()) {

                    this.ShowLoading();

                    BackgroundWorker Worker = new BackgroundWorker();
                    Worker.DoWork += delegate {
                        Serie ToAddSerie = ApiHelper.Global.LoadSeries(Dialog.ShowId, false);
                        this.lvSerien.Dispatcher.Invoke(new Action(delegate {
                            ((ObservableCollection<Serie>)this.lvSerien.ItemsSource).Add(ToAddSerie);
                        }));
                    };

                    Worker.RunWorkerCompleted += delegate {
                        this.HideLoading();
                    };

                    Worker.RunWorkerAsync();

                    
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Es ist ein Fehler aufgetreten", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowLoading() {
            this.dpMain.Dispatcher.Invoke(new Action(delegate {
                this.dpMain.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                this.lvSerien.Visibility = System.Windows.Visibility.Collapsed;
                this.cIsLoading.Visibility = System.Windows.Visibility.Visible;
            }));
        }

        private void HideLoading() {
            this.dpMain.Dispatcher.Invoke(new Action(delegate {
                this.dpMain.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                this.lvSerien.Visibility = System.Windows.Visibility.Visible;
                this.cIsLoading.Visibility = System.Windows.Visibility.Collapsed;
            }));
        }

        private void lvSerien_KeyDown(object sender, KeyEventArgs e) {
            
            // Erkennen der gedrückten Taste muss noch optimiert werden
            string Key = e.Key.ToString();
            if (Regex.IsMatch(Key, "^[a-z0-9äöü_]$", RegexOptions.IgnoreCase)) {
                
                var Items = ((ObservableCollection<Serie>)this.lvSerien.ItemsSource).Where(Current => Current.Name.StartsWith(Key));

                int PositionOld = CollectionViewSource.GetDefaultView(this.lvSerien.ItemsSource).CurrentPosition;
                foreach (Serie Current in Items) {
                    CollectionViewSource.GetDefaultView(this.lvSerien.ItemsSource).MoveCurrentTo(Current);
                    int CurrentPosition = CollectionViewSource.GetDefaultView(this.lvSerien.ItemsSource).CurrentPosition;
                    if (PositionOld == CurrentPosition && Items.Last().Equals(Current)) {
                        // Wieder an den Anfang springen, das zuvor ausgewählte Element war bereits das letzte Element
                        CollectionViewSource.GetDefaultView(this.lvSerien.ItemsSource).MoveCurrentTo(Items.First());
                        // Item anzeigen
                        this.lvSerien.ScrollIntoView(Items.First());
                        break;
                    }
                    if (PositionOld < CurrentPosition) {
                        // Item anzeigen
                        this.lvSerien.ScrollIntoView(Current);
                        // Aktuelle Position ist größer der alten Position => passt
                        break;
                    }

                }
            }

        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

            if (!e.Handled && (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))) {
                FrameworkElement CurrentSender = (FrameworkElement)sender;
                var CurrentStaffel = (Staffel)((FrameworkElement)e.OriginalSource).Tag;
                CurrentStaffel.ChangeAEF();
            }

            e.Handled = true;
        }
    }
}
