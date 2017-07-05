using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;

namespace Core.Units {
    
    public class Serie : Entity, IXml, ICloneable {

        /// <summary>
        /// Staffeln der Serie
        /// </summary>
        private ObservableCollection<Staffel> m_Staffeln = new ObservableCollection<Staffel>();

        #region ICommands

        public ICommand ComAddZuletztGesehenStaffelNeueFolgeHeute { get; private set; }

        public ICommand ComAddZuletztGesehenStaffelNeueFolgeGestern { get; private set; }

        public ICommand ComAddNeueStaffelFolgeEinsGesehenHeute { get; private set; }

        public ICommand ComAddNeueStaffelFolgeEinsGesehenGestern { get; private set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name">Name der Serie</param>
        /// <param name="Staffeln"></param>
        public Serie(string Name, IEnumerable<Staffel> Staffeln = null) {

            if (string.IsNullOrWhiteSpace(Name)) {
                throw new ArgumentException();
            }
            
            this.Name = Name;

            if (Staffeln != null) {
                foreach (Staffel Current in Staffeln) {
                    this.Staffeln.Add(Current);
                }

            }

            this.ComAddZuletztGesehenStaffelNeueFolgeHeute = new RelayCommand(param => this.AddNeueFolge(0));
            this.ComAddZuletztGesehenStaffelNeueFolgeGestern = new RelayCommand(param => this.AddNeueFolge(-1));

            this.ComAddNeueStaffelFolgeEinsGesehenHeute = new RelayCommand(param => this.AddNeueStaffel(0));
            this.ComAddNeueStaffelFolgeEinsGesehenGestern = new RelayCommand(param => this.AddNeueStaffel(-1));
        }

        private void AddNeueFolge(int AddDays) {

            if (this.ZuletztGesehenStaffel == null) {
                Staffel St = new Staffel(1);
                St.Folgen.Add(new Folge(1, false, DateTime.Today.AddDays(AddDays)));
                this.Staffeln.Add(St);
            }
            else {
                this.ZuletztGesehenStaffel.AddFolge(AddDays, AddEntryNumber.NextNumberFromLastSeen, AddEntryExists.AddToExistingEntry);
            }

            
            this.OnPropertyChanged("Staffeln");
            this.OnPropertyChanged("ZuletztGesehenStaffel");
            this.OnPropertyChanged("ZuletztGesehenFolge");
        }

        private void AddNeueStaffel(int AddDays) {

            Staffel St = null;

            if (this.ZuletztGesehenStaffel != null) {

                St = this.Staffeln.Where(Current => Current.Nummer == this.ZuletztGesehenStaffel.Nummer + 1).FirstOrDefault();
                if (St != null) {
                    St.AddFolge(AddDays, AddEntryNumber.NextNumberFromLastSeen, AddEntryExists.AddToExistingEntry);
                }
                else {
                    St = new Staffel(this.ZuletztGesehenStaffel.Nummer);
                    this.Staffeln.Add(St.Next(DateTime.Today.AddDays(AddDays)));
                }

            }
            else {
                St = new Staffel(0);
                this.Staffeln.Add(St.Next(DateTime.Today.AddDays(AddDays)));
            }

            this.OnPropertyChanged("Staffeln");
            this.OnPropertyChanged("ZuletztGesehenStaffel");
            this.OnPropertyChanged("ZuletztGesehenFolge");
        }

        /// <summary>
        /// Staffeln der Serie
        /// </summary>
        public ObservableCollection<Staffel> Staffeln {
            get { return this.m_Staffeln; }
            set {

                if (value == null) {
                    throw new ArgumentNullException();
                }
                
                this.m_Staffeln = value;
                this.OnPropertyChanged("Staffeln");
                this.OnPropertyChanged("ZuletztGesehenStaffel");
                this.OnPropertyChanged("ZuletztGesehenFolge");
            }
        }

        
        public Staffel ZuletztGesehenStaffel {
            get { return this.GetZuletztGesehenStaffel(); }
        }

        [DebuggerStepThrough]
        private Staffel GetZuletztGesehenStaffel() {
            return this.Staffeln.OrderByDescending(Current => Current.ZuletztGesehenFolge.ZeitpunktGesehenZuletzt).ThenByDescending(Current => Current.Nummer).FirstOrDefault();
        }

        public Folge ZuletztGesehenFolge {
            get { return ZuletztGesehenStaffel.ZuletztGesehenFolge; }
        }

        #region Interfaces

        public XElement ToXML() {

            XElement Current = new XElement(typeof(Serie).Name);
            Current.Add(new XAttribute(Serie.XmlAttrName, this.Name));

            XElement CurrentStaffeln = new XElement(Staffel.XmlListe);
            foreach (Staffel CurrentStaffel in this.Staffeln) {
                CurrentStaffeln.Add(CurrentStaffel.ToXML());
            }

            Current.Add(CurrentStaffeln);

            return Current;
        }

        public object Clone() {
            return new Serie(this.Name, this.Staffeln.Clone());
        }

        #endregion

    }
}
