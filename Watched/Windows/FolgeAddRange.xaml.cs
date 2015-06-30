using Core;
using Core.Units;
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

namespace Watched.Windows {
    /// <summary>
    /// Interaktionslogik für FolgeAddRange.xaml
    /// </summary>
    public partial class FolgeAddRange : Window {

        Collection<Folge> m_Current;

        public FolgeAddRange(Collection<Folge> Current) {
            InitializeComponent();
            this.m_Current = Current;
            this.tbStart.Text = Current.NummerNeueFolge().ToString();
        }

        private void ButtonAdd(object sender, RoutedEventArgs e) {
            int Start = 0;
            int Ende = 0;

            if (int.TryParse(this.tbStart.Text.Trim(), out Start) && int.TryParse(this.tbEnde.Text.Trim(), out Ende)) {
                try {
                    FromTo FT = new FromTo(Start, Ende);

                    List<int> ÜberschneidungenNummern = new List<int>();
                    IEnumerable<int> VorhandeNummern = this.m_Current.Select(Current => Current.Nummer);
                    foreach (int Current in FT.Generate()) {
                        if (VorhandeNummern.Contains(Current)) {
                            ÜberschneidungenNummern.Add(Current);
                        }
                    }

                    if (ÜberschneidungenNummern.Count > 0) {
                        // Einige Einträge vorhanden
                        switch (MessageBox.Show(string.Format("Ja: Füge vorhande Einträge auch hinzu ({0})\nNein: Nur nicht vorhande Folgen hinzufügen\nAbbrechen: Vorgang Abbrechen", string.Join(", ", ÜberschneidungenNummern)), "", 
                            MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel)
                        ) { 
                            case MessageBoxResult.Yes:
                                this.m_Current.AddRange(FT.Generate());
                                this.DialogResult = true;
                                this.Close();
                                break;
                            case MessageBoxResult.No:
                                this.m_Current.AddRange(FT.Generate().Where(Current => !ÜberschneidungenNummern.Contains(Current)));
                                this.DialogResult = true;
                                this.Close();
                                break;
                            case MessageBoxResult.Cancel:
                                break;
                        }
                    }
                    else {
                        this.m_Current.AddRange(FT.Generate());
                        this.DialogResult = true;
                        this.Close();
                        
                    }

                }
                catch (Exception ex){
                    MessageBox.Show("Die Ende-Zahl muss größer als die Start Zahl sein\n" + ex.Message);
                }
            }
            else {
                MessageBox.Show("Bitte nur Zahlen eintragen");
            }

        }
    }
}
