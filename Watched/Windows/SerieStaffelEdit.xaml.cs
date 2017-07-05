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
using Core;

namespace Watched.Windows {
    /// <summary>
    /// Interaktionslogik für SerieStaffelEdit.xaml
    /// </summary>
    public partial class SerieStaffelEdit : Window {

        private ObservableCollection<Staffel> m_Before;

        public SerieStaffelEdit(ObservableCollection<Staffel> Staffeln = null, string Serienname = "") {
            InitializeComponent();

            Staffeln = Staffeln ?? new ObservableCollection<Staffel>();

            foreach (var ItemStaffel in Staffeln) {
                foreach (var ItemFolge in ItemStaffel.Folgen) {
                    ItemFolge.ZugehörigeStaffel = ItemStaffel;
                }
            }

            this.Title = Serienname;

            this.cbStaffeln.ItemsSource = new ObservableCollection<Staffel>(Staffeln.OrderByDescending(Current => Current.Nummer));
            this.Before = Staffeln.Clone();
        }

        private ObservableCollection<Staffel> Before {
            get { return m_Before; }
            set { m_Before = value; }
        }

        public object Return {
            get { return this.cbStaffeln.ItemsSource; }
        }

        private void AddStaffel(object sender, RoutedEventArgs e) {
            int Nummer = int.MinValue;
            if (int.TryParse(this.tbStaffelNummer.Text, out Nummer)) {
                ((ObservableCollection<Staffel>)cbStaffeln.ItemsSource).Add(new Staffel(Nummer, null, this.tbStaffelName.Text));
                this.tbStaffelNummer.Text = string.Empty;
                this.tbStaffelName.Text = string.Empty;
            }
        }

        private void AddFolge(object sender, RoutedEventArgs e) {
            int Nummer = int.MinValue;
            if (int.TryParse(this.tbFolgeNummer.Text, out Nummer)) {
                ((Staffel)cbStaffeln.SelectedItem).Folgen.Add(new Folge(Nummer, false, null, this.tbFolgeName.Text));
                this.tbFolgeNummer.Text = string.Empty;
                this.tbFolgeName.Text = string.Empty;
            }
        }

        private void RemoveStaffel(object sender, RoutedEventArgs e) {
            try {
                if (MessageBox.Show("Wirklich löschen?", "Löschen?", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                    ((ObservableCollection<Staffel>)cbStaffeln.ItemsSource).Remove(((Staffel)cbStaffeln.SelectedItem));
                    cbStaffeln.SelectedIndex = 0;
                }
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message, "Staffel entfernen nicht möglich.");
            }
        }

        private void EditStaffel(object sender, RoutedEventArgs e) {
            try {
                Staffel Current = (Staffel)cbStaffeln.SelectedItem;

                StaffelEdit Edit = new StaffelEdit((Staffel)Current.Clone());
                if ((bool)Edit.ShowDialog()) {
                    int Selected = this.cbStaffeln.SelectedIndex;
                    ((ObservableCollection<Staffel>)this.cbStaffeln.ItemsSource).Replace(Current, (Staffel)Edit.Return);
                    this.cbStaffeln.SelectedIndex = Selected;
                }

            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Staffel bearbeiten nicht möglich.");
            }
        }

        private void EditFolgeDoubleClick(object sender, MouseButtonEventArgs e) {
            FrameworkElement CurrentSender = (FrameworkElement)sender;
            this.EditFolge((Folge)this.lvFolgen.SelectedItem);
        }

        private void EditFolge(object sender, MouseButtonEventArgs e) {
            FrameworkElement CurrentSender = (FrameworkElement)sender;
            this.EditFolge((Folge)CurrentSender.Tag);
        }

        private void EditFolge(Folge Current) {
            FolgeEdit Edit = new FolgeEdit((Folge)Current.Clone());

            if ((bool)Edit.ShowDialog()) {
                Staffel CurrentStaffel = (Staffel)cbStaffeln.SelectedItem;
                CurrentStaffel.Folgen.Replace(Current, (Folge)Edit.Return);
            }
        }

        private void RemoveFolge(object sender, MouseButtonEventArgs e) {
            FrameworkElement CurrentSender = (FrameworkElement)sender;
            Folge CurrentFolge = (Folge)CurrentSender.Tag;

            if (MessageBox.Show("Wirklich löschen?", "Löschen?", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                Staffel CurrentStaffel = (Staffel)cbStaffeln.SelectedItem;
                CurrentStaffel.Folgen.Remove(CurrentFolge);
            }
        }

        private void ButtonSave(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
            this.Close();
        }

        private void AddFolgeRange(object sender, RoutedEventArgs e) {
            FolgeAddRange Add = new FolgeAddRange(((Staffel)cbStaffeln.SelectedItem).Folgen);
            if ((bool)Add.ShowDialog()) { 
            
            }
        }

        private void SearchFolgenVerpasst(object sender, RoutedEventArgs e) {
            Staffel CurrentStaffel = (Staffel)cbStaffeln.SelectedItem;
            
            int NummerNeu = CurrentStaffel.Folgen.NummerNeueFolge();

            IEnumerable<int> Lücken = new int[0];

            if(NummerNeu > 1){
                FromTo FT = new FromTo(1,  NummerNeu - 1);
                Lücken = FT.Generate().Where(Current => !CurrentStaffel.Folgen.Select(InnerCurrent => InnerCurrent.Nummer).Contains(Current));
            }

            if (Lücken.Count() > 0) {
                MessageBox.Show(string.Join(", ", Lücken), "Verpasste Folgen", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else {
                MessageBox.Show("Keine verpassten Folgen!", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }

            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

            var SBefore = string.Join(string.Empty, this.Before.Select(Current => Current.ToXML()));
            var SAfter = string.Join(string.Empty, this.cbStaffeln.ItemsSource.OfType<Staffel>().Select(Current => Current.ToXML()));

            if (this.DialogResult != true && SBefore != SAfter) {
                if (Stuff.AskForSave() == MessageBoxResult.Yes) {
                    this.DialogResult = true;
                }
                else {
                    this.DialogResult = false;
                }
            }
        }
    }
}
