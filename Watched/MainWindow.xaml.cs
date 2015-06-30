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

namespace Watched {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            /*
            List<DateTime> Gesehen1 = new List<DateTime>();
            Gesehen1.Add(DateTime.Now);
            Gesehen1.Add(DateTime.Now.AddDays(1));
            Gesehen1.Add(DateTime.Now.AddDays(2));

            List<Folge> FStaffel1 = new List<Folge>();
            FStaffel1.Add(new Folge(1));
            FStaffel1.Add(new Folge(2, Gesehen1));
            FStaffel1.Add(new Folge(3));
            FStaffel1.Add(new Folge(4));

            List<Folge> FStaffel2 = new List<Folge>();
            FStaffel2.Add(new Folge(1));
            FStaffel2.Add(new Folge(2));
            FStaffel2.Add(new Folge(3));
            FStaffel2.Add(new Folge(4, Gesehen1));

            List<Staffel> LostStaffeln = new List<Staffel>();
            LostStaffeln.Add(new Staffel(1, FStaffel1));
            LostStaffeln.Add(new Staffel(2, FStaffel2));

            Serie CurrentSerie = new Serie("Lost", LostStaffeln);

            List<Folge> FStaffelX = new List<Folge>();
            FStaffel1.Add(new Folge(1));
            FStaffel1.Add(new Folge(2, Gesehen1));
            FStaffel1.Add(new Folge(3));
            FStaffel1.Add(new Folge(4));

            List<Staffel> HeroesStaffeln = new List<Staffel>();
            LostStaffeln.Add(new Staffel(1, FStaffelX));

            Serie CurrentSerie2 = new Serie("Heroes", HeroesStaffeln);

            
            Serien.Add(CurrentSerie);
            Serien.Add(CurrentSerie2);*/

            ObservableCollection<Serie> Serien = new ObservableCollection<Serie>();

            if (!XmlStuff.Load(ref Serien)) { 
                MessageBox.Show("Laden fehlgeschlagen.");
            }


            this.lvSerien.ItemsSource = Serien;
        }

        #region XML



        #endregion


        private void EditStaffeln(object sender, MouseButtonEventArgs e) {
            FrameworkElement CurrentSender = (FrameworkElement)sender;
            this.EditStaffeln((Serie)CurrentSender.Tag);
        }

        private void EditStaffelnDoubleClick(object sender, MouseButtonEventArgs e) {
            FrameworkElement CurrentSender = (FrameworkElement)sender;
            this.EditStaffeln((Serie)this.lvSerien.SelectedItem);
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
                if (MessageBox.Show("Sie haben Änderungen vorgenommen möchten sie diese speichern?", "Änderungen verwerfen?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
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
    }
}
