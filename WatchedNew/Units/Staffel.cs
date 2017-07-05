using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;

namespace Core.Units {


    public enum StaffelAEF {

        Yes,

        No,

        Partly

    }

    public enum AddEntryNumber {

        /// <summary>
        /// Nächsthöhere Nummer hinzufügen
        /// </summary>
        NextNumber,

        /// <summary>
        /// Nächsthöhere Nummer hinzufügen, ausgehend von der Folge die zuletzt gesehen wurde
        /// </summary>
        NextNumberFromLastSeen

    }

    public enum AddEntryExists { 
    
        /// <summary>
        /// Neue Folge hinzufügen auch wenn eine Folge mit diese Nummer bereits vorhanden ist
        /// </summary>
        AddNewEntry,

        /// <summary>
        /// Der Folge mit der Nummer nur einen zuletzt gesehen Eintrag hinzufügen falls vorhanden, ansonsten neue Folge hinzufügen
        /// </summary>
        AddToExistingEntry

    }

    /// <summary>
    /// Klasse zur Abbildung einer Staffel
    /// </summary>
    public class Staffel : Entity, IXml, ICloneable {

        public const string DefaultName = "";
        
        /// <summary>
        /// Nummer der Staffel
        /// </summary>
        private int m_Nummer;

        /// <summary>
        /// Folgen der Staffel
        /// </summary>
        private ObservableCollection<Folge> m_Folgen = new ObservableCollection<Folge>();


        public ICommand ComChangeAEF { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nummer">Nummer der Staffel</param>
        /// <param name="Folgen">Folgen der Staffel</param>
        /// <param name="Name">Name der Staffel</param>
        public Staffel(int Nummer, IEnumerable<Folge> Folgen = null, string Name = "") {
            this.Nummer = Nummer;

            if (Folgen != null) {
                foreach (Folge Current in Folgen) {
                    this.Folgen.Add(Current);
                }
            }

            this.Name = Staffel.DefaultName;
            if (!string.IsNullOrWhiteSpace(Name)) {
                this.Name = Name;
            }

            this.AEF = AEF;

            this.ComChangeAEF = new RelayCommand(param => ChangeAEF());
        }



        /// <summary>
        /// Gibt an ob sich die Staffel "auf externer Festplatte" befindet
        /// <param>null = nicht AEF, false = teilweise AEF, true = alle AEF</param>
        /// </summary>
        public StaffelAEF AEF {
            get {

                if(this.Folgen == null || this.Folgen.Count < 1) {
                    return StaffelAEF.No;
                }

                var EpisodesAEF = this.Folgen.Where(Current => Current.AEF).Count();

                if(EpisodesAEF == 0)
                    return StaffelAEF.No;
                else if(EpisodesAEF != this.Folgen.Count())
                    return StaffelAEF.Partly;
                else
                    return StaffelAEF.Yes;

            }
            set {



                this.OnPropertyChanged();
            }
        }

        public void ChangeAEF() {

            var Current = this.AEF;

            if(this.Folgen != null) {
                foreach(var Item in this.Folgen) {
                    Item.AEF = Current == StaffelAEF.Yes ? false : true;
                }
            }

            this.UpdatePropertyAEF();
        }

        public void UpdatePropertyAEF() {
            this.OnPropertyChanged(nameof(AEF));

        }

        /// <summary>
        /// Nummer der Staffel
        /// </summary>
        public int Nummer {
            get { return this.m_Nummer; }
            set { 
                this.m_Nummer = value;
                this.OnPropertyChanged();
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
            SNext.Folgen.Add(new Folge(1, false, new DateTime[] { GesehenFolgeEins }));
            return SNext;
        }

        public void AddFolge(int AddDays, AddEntryNumber EntryNumber, AddEntryExists EntryExists) {


            int Nummer = 1;

            if (this.Folgen.Count > 0) {
                switch (EntryNumber) {
                    case AddEntryNumber.NextNumber:
                        int[] SortArray = this.Folgen.Select(Current => Current.Nummer).ToArray();
                        Array.Sort(SortArray);
                        Nummer = SortArray.Last();
                        break;
                    case AddEntryNumber.NextNumberFromLastSeen:
                        Nummer = this.ZuletztGesehenFolge.Nummer + 1;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }


            Folge Temp = this.Folgen.Where(Current => Current.Nummer == Nummer).FirstOrDefault();

            if (Temp != null && EntryExists == AddEntryExists.AddToExistingEntry) {
                Temp.ZeitpunktGesehen.Add(DateTime.Today.AddDays(AddDays));
            }
            else {
                this.Folgen.Add(new Folge(Nummer, false, DateTime.Today.AddDays(AddDays)));
            }
        }


        #region Interfaces

        public XElement ToXML() {
            XElement Current = new XElement(typeof(Staffel).Name);
            Current.Add(new XAttribute(Staffel.XmlAttrNummer, this.Nummer));
            Current.Add(new XAttribute(Staffel.XmlAttrName, this.Name));
            Current.Add(new XAttribute(Staffel.XmlAttrAEF, "X"));

            XElement Folgen = new XElement(Folge.XmlListe);
            foreach (Folge CurrentFolge in this.Folgen) {
                Folgen.Add(CurrentFolge.ToXML());
            }

            Current.Add(Folgen);

            return Current;
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
        
        public const string XmlAttrAEF = "AEF";

        public const string XmlListe = "ListeStaffeln";



    }
}
