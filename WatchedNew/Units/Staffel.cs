using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Core.Units {
    
    /// <summary>
    /// Klasse zur Abbildung einer Staffel
    /// </summary>
    public class Staffel : IXml, INotifyPropertyChanged, ICloneable {

        public const string DefaultName = "";

        /// <summary>
        /// Nummer der Staffel
        /// </summary>
        private int m_Nummer;

        /// <summary>
        /// Name der Staffel
        /// </summary>
        private string m_Name;

        /// <summary>
        /// Folgen der Staffel
        /// </summary>
        private ObservableCollection<Folge> m_Folgen = new ObservableCollection<Folge>();


        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nummer">Nummer der Staffel</param>
        /// <param name="Folgen">Folgen der Staffel</param>
        /// <param name="Name">Name der Staffel</param>
        public Staffel(int Nummer, IEnumerable<Folge> Folgen = null, string Name = "") {
            this.m_Nummer = Nummer;

            if (Folgen != null) {
                foreach (Folge Current in Folgen) {
                    this.Folgen.Add(Current);
                }

            }

            this.Name = Staffel.DefaultName;
            if (!string.IsNullOrWhiteSpace(Name)) {
                this.Name = Name;
            }
        }

        /// <summary>
        /// Nummer der Staffel
        /// </summary>
        public int Nummer {
            get { return this.m_Nummer; }
            set { 
                this.m_Nummer = value;
                this.OnPropertyChanged("Nummer");
            }
        }

        /// <summary>
        /// Name der Staffel
        /// </summary>
        public string Name {
            get { return this.m_Name; }
            private set {
                this.m_Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Folgen der Staffel
        /// </summary>
        public ObservableCollection<Folge> Folgen {
            get { return this.m_Folgen; }
            set { 
                this.m_Folgen = value;
                this.OnPropertyChanged("Folgen");
            }
        }

        public Folge ZuletztGesehenFolge {
            get { return this.Folgen.ZuletztGesehen(); }
        }

        public Staffel Next(DateTime GesehenFolgeEins) {
            Staffel SNext = new Staffel(this.Nummer + 1);
            SNext.Folgen.Add(new Folge(1, new DateTime[] { GesehenFolgeEins }));
            return SNext;
        }

        #region Interfaces

        public XElement ToXML() {
            XElement Current = new XElement(typeof(Staffel).Name);
            Current.Add(new XAttribute(Staffel.XmlAttrNummer, this.Nummer));
            Current.Add(new XAttribute(Staffel.XmlAttrName, this.Name));

            XElement Folgen = new XElement(Folge.XmlListe);
            foreach (Folge CurrentFolge in this.Folgen) {
                Folgen.Add(CurrentFolge.ToXML());
            }

            Current.Add(Folgen);

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
            return new Staffel(this.Nummer, this.Folgen.Clone(), this.Name);
        }

        public override string ToString() {
            string TmpName = Name != Staffel.DefaultName ? Nummer + " (" + Name + ")" : Nummer.ToString();
            return Nummer == 0 && TmpName != Staffel.DefaultName ? Name : TmpName;
        }

        #endregion

        public const string XmlAttrNummer = "Nummer";
        public const string XmlAttrName = "Name";

        public const string XmlListe = "ListeStaffeln";



    }
}
