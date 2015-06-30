using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;

namespace Core.Units {
    
    public class Folge : IXml, INotifyPropertyChanged, ICloneable {

        public const string DefaultName = "";

        /// <summary>
        /// Nummer der Folge
        /// </summary>
        private int m_Nummer;
        
        /// <summary>
        /// Name der Folge
        /// </summary>
        private string m_Name;

        /// <summary>
        /// Zeitpunkte zu dem die Folge gesehen wurde
        /// </summary>
        private ObservableCollection<DateTime> m_ZeitpunktGesehen = new ObservableCollection<DateTime>();

        #region Commands

        /// <summary>
        /// Als gesehen Zeitpunkt 'Heute' hinzufügen
        /// </summary>
        private ICommand m_ComAddZeitpunktGesehenHeute;

        /// <summary>
        /// Als gesehen Zeitpunkt 'Gestern' hinzufügen
        /// </summary>
        private ICommand m_ComAddZeitpunktGesehenGestern;



        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nummer">Nummer der Folge</param>
        /// <param name="Name">Name der Folge</param>
        public Folge(int Nummer, IEnumerable<DateTime> ZeitpunktGesehen = null, string Name = "") {
            this.Nummer = Nummer;
            
            if (ZeitpunktGesehen != null) { 
                foreach(DateTime Current in ZeitpunktGesehen){
                    this.ZeitpunktGesehen.Add(Current);
                }
                
            }

            this.Name = Folge.DefaultName;
            if (!string.IsNullOrWhiteSpace(Name)) {
                this.Name = Name;
            }

            this.ComAddZeitpunktGesehenHeute = new RelayCommand(param => this.AddZeitpunktGesehen(0));
            this.ComAddZeitpunktGesehenGestern = new RelayCommand(param => this.AddZeitpunktGesehen(-1));
        }

        /// <summary>
        /// Nummer der Folge
        /// </summary>
        public int Nummer {
            get { return this.m_Nummer; }
            set {
                this.m_Nummer = value;
                this.OnPropertyChanged("Nummer");
            }
        }

        /// <summary>
        /// Name der Folge
        /// </summary>
        public string Name {
            get { return this.m_Name; }
            set { 
                this.m_Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Zeitpunkte zu dem die Folge gesehen wurde
        /// </summary>
        public ObservableCollection<DateTime> ZeitpunktGesehen {
            get { return this.m_ZeitpunktGesehen; }
            set {
                this.m_ZeitpunktGesehen = value;
                this.OnPropertyChanged("ZeitpunktGesehen");
                this.OnPropertyChanged("ZeitpunktGesehenZuletzt");
            }
        }

        /// <summary>
        /// Der letzte Zeitpunkt zu dem die Folge gesehen wurde
        /// </summary>
        public DateTime ZeitpunktGesehenZuletzt {
            get { return this.ZeitpunktGesehen.OrderByDescending(Current => Current).FirstOrDefault(); }
            private set {
                this.OnPropertyChanged("ZeitpunktGesehen");
                this.OnPropertyChanged("ZeitpunktGesehenZuletzt");
            }
        }

        #region Commands

        /// <summary>
        /// Als gesehen Zeitpunkt 'Heute' hinzufügen
        /// </summary>
        public ICommand ComAddZeitpunktGesehenHeute {
            get { return this.m_ComAddZeitpunktGesehenHeute; }
            private set { 
                this.m_ComAddZeitpunktGesehenHeute = value;
                this.OnPropertyChanged("ComAddZeitpunktGesehenHeute");
            }
        }

        /// <summary>
        /// Als gesehen Zeitpunkt 'Gestern' hinzufügen
        /// </summary>
        public ICommand ComAddZeitpunktGesehenGestern {
            get { return this.m_ComAddZeitpunktGesehenGestern; }
            private set { 
                this.m_ComAddZeitpunktGesehenGestern = value;
                this.OnPropertyChanged("ComAddZeitpunktGesehenGestern");
            }
        }

        #endregion

        public void AddZeitpunktGesehen(int Days) {
            this.ZeitpunktGesehen.Add(DateTime.Today.AddDays(Days));
        }

        


        #region Interfaces

        public XElement ToXML() {
            XElement Current = new XElement(typeof(Folge).Name);
            Current.Add(new XAttribute(Folge.XmlAttrNummer, this.Nummer));
            Current.Add(new XAttribute(Folge.XmlAttrName, this.Name));

            foreach (DateTime DTCurrent in this.ZeitpunktGesehen) {
                Current.Add(new XElement("D", new XAttribute("T", DTCurrent.Ticks))); 
            }

            return Current;
        }

        protected virtual void OnPropertyChanged(string PropertyName) {
            Helper.VerifyPropertyName(this, PropertyName);
            PropertyChangedEventHandler Handler = this.PropertyChanged;
            if (Handler != null) {
                Handler(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public object Clone() {
            return new Folge(this.Nummer, this.ZeitpunktGesehen, this.Name);
        }

        public override string ToString() {
            return string.Format("{0}{1}", Nummer, Name != Staffel.DefaultName ? " (" + Name + ")" : string.Empty);
        }

        #endregion

        public const string XmlAttrNummer = "Nummer";
        public const string XmlAttrName = "Name";

        public const string XmlListe = "ListeFolge";



    }
}
