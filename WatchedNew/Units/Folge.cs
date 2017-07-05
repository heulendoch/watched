using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;

namespace Core.Units {
    
    public class Folge : Entity, IXml, ICloneable {

        public const string DefaultName = "";

        /// <summary>
        /// Gibt an ob sich die Folge "auf externer Festplatte" befindet
        /// </summary>
        private bool m_AEF;

        /// <summary>
        /// Nummer der Folge
        /// </summary>
        private int m_Nummer;

        /// <summary>
        /// 
        /// </summary>
        private Staffel m_ZugehörigeStaffel;

        /// <summary>
        /// Zeitpunkte zu dem die Folge gesehen wurde
        /// </summary>
        private ObservableCollection<DateTime> m_ZeitpunktGesehen = new ObservableCollection<DateTime>();

        #region Commands

        /// <summary>
        /// Status von AEF ändern
        /// </summary>
        public ICommand ComChangeAEF { get; private set; }

        /// <summary>
        /// Als gesehen Zeitpunkt 'Heute' hinzufügen
        /// </summary>
        public ICommand ComAddZeitpunktGesehenHeute { get; private set; }

        /// <summary>
        /// Als gesehen Zeitpunkt 'Gestern' hinzufügen
        /// </summary>
        public ICommand ComAddZeitpunktGesehenGestern { get; private set; }

        #endregion


        #region Konstruktoren

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nummer">Nummer der Folge</param>
        /// <param name="ZeitpunktGesehen">Zeitpunkte zu dem die Folge gesehen wurde</param>
        /// <param name="Name">Name der Folge</param>
        public Folge(int Nummer, bool AEF, IEnumerable<DateTime> ZeitpunktGesehen = null, string Name = "") {
            this.Init(Nummer, AEF, ZeitpunktGesehen, Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nummer">Nummer der Folge</param>
        /// <param name="ZeitpunktGesehen">Zeitpunkt zu dem die Folge gesehen wurde</param>
        /// <param name="Name">Name der Folge</param>
        public Folge(int Nummer, bool AEF, DateTime ZeitpunktGesehen, string Name = "") {
            this.Init(Nummer, AEF, new DateTime[] { ZeitpunktGesehen }, Name);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gibt an ob sich die Folge "auf externer Festplatte" befindet
        /// </summary>
        public bool AEF {
            get { return this.m_AEF; }
            set {
                this.m_AEF = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Nummer der Folge
        /// </summary>
        public int Nummer {
            get { return this.m_Nummer; }
            set {
                this.m_Nummer = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Staffel ZugehörigeStaffel {
            get { return this.m_ZugehörigeStaffel; }
            set {
                this.m_ZugehörigeStaffel = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Zeitpunkte zu dem die Folge gesehen wurde
        /// </summary>
        public ObservableCollection<DateTime> ZeitpunktGesehen {
            get { return this.m_ZeitpunktGesehen; }
            set {
                this.m_ZeitpunktGesehen = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("ZeitpunktGesehenZuletzt");
            }
        }

        /// <summary>
        /// Der letzte Zeitpunkt zu dem die Folge gesehen wurde
        /// </summary>
        public DateTime ZeitpunktGesehenZuletzt {
            get { return this.ZeitpunktGesehen.OrderByDescending(Current => Current).FirstOrDefault(); }
            private set {
                this.OnPropertyChanged();
                this.OnPropertyChanged("ZeitpunktGesehen");
            }
        }




        #endregion


        private void Init(int Nummer, bool AEF, IEnumerable<DateTime> ZeitpunktGesehen = null, string Name = "") {

            this.Nummer = Nummer;
            this.AEF = AEF;

            if(ZeitpunktGesehen != null) {
                foreach(DateTime Current in ZeitpunktGesehen) {
                    this.ZeitpunktGesehen.Add(Current);
                }

            }

            this.Name = Folge.DefaultName;
            if(!string.IsNullOrWhiteSpace(Name)) {
                this.Name = Name;
            }

            this.ComChangeAEF = new RelayCommand(param => this.ChangeAEF());

            this.ComAddZeitpunktGesehenHeute = new RelayCommand(param => this.AddZeitpunktGesehen(0));
            this.ComAddZeitpunktGesehenGestern = new RelayCommand(param => this.AddZeitpunktGesehen(-1));
        }

        private void ChangeAEF() {
            this.AEF = !this.AEF;
            this.ZugehörigeStaffel.UpdatePropertyAEF();
        }

        public void AddZeitpunktGesehen(int Days) {
            this.ZeitpunktGesehen.Add(DateTime.Today.AddDays(Days));
        }

        #region Interfaces

        public XElement ToXML() {
            XElement Current = new XElement(typeof(Folge).Name);
            Current.Add(new XAttribute(Folge.XmlAttrNummer, this.Nummer));
            Current.Add(new XAttribute(Folge.XmlAttrName, this.Name));
            Current.Add(new XAttribute(Folge.XmlAttrAEF, this.AEF));

            foreach (DateTime DTCurrent in this.ZeitpunktGesehen) {
                Current.Add(new XElement("D", new XAttribute("T", DTCurrent.Ticks))); 
            }

            return Current;
        }

        public object Clone() {
            return new Folge(this.Nummer, this.AEF, this.ZeitpunktGesehen, this.Name);
        }

        public override string ToString() {
            return string.Format("{0}{1}", Nummer, Name != Staffel.DefaultName ? " (" + Name + ")" : string.Empty);
        }

        #endregion

        public const string XmlAttrNummer = "Nummer";

        public const string XmlAttrAEF = "AEF";

        public const string XmlListe = "ListeFolge";



    }
}
