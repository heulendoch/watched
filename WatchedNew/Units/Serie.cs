using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;

namespace Core.Units {
    
    public class Serie : IXml, INotifyPropertyChanged, ICloneable {

        /// <summary>
        /// Name der Serie
        /// </summary>
        private string m_Name;

        /// <summary>
        /// Staffeln der Serie
        /// </summary>
        private ObservableCollection<Staffel> m_Staffeln = new ObservableCollection<Staffel>();


        private ICommand m_ComAddZuletztGesehenStaffelNeueFolgeHeute;

        private ICommand m_ComAddZuletztGesehenStaffelNeueFolgeGestern;



        public ICommand ComAddZuletztGesehenStaffelNeueFolgeHeute {
            get { return m_ComAddZuletztGesehenStaffelNeueFolgeHeute; }
            private set { m_ComAddZuletztGesehenStaffelNeueFolgeHeute = value; }
        }

        public ICommand ComAddZuletztGesehenStaffelNeueFolgeGestern {
            get { return m_ComAddZuletztGesehenStaffelNeueFolgeGestern; }
            private set { m_ComAddZuletztGesehenStaffelNeueFolgeGestern = value; }
        }




        private ICommand m_ComAddNeueStaffelFolgeEinsGesehenHeute;

        private ICommand m_ComAddNeueStaffelFolgeEinsGesehenGestern;

        public ICommand ComAddNeueStaffelFolgeEinsGesehenHeute {
            get { return m_ComAddNeueStaffelFolgeEinsGesehenHeute; }
            set { m_ComAddNeueStaffelFolgeEinsGesehenHeute = value; }
        }

        public ICommand ComAddNeueStaffelFolgeEinsGesehenGestern {
            get { return m_ComAddNeueStaffelFolgeEinsGesehenGestern; }
            set { m_ComAddNeueStaffelFolgeEinsGesehenGestern = value; }
        }


        public event PropertyChangedEventHandler PropertyChanged;

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
                St.Folgen.Add(new Folge(1, new DateTime[] { DateTime.Today.AddDays(AddDays) }));
                this.Staffeln.Add(St);
            }
            else {
                this.ZuletztGesehenStaffel.Folgen.Add(new Folge(this.ZuletztGesehenFolge.Nummer + 1, new DateTime[] { DateTime.Today.AddDays(AddDays) }));
            }

            
            this.OnPropertyChanged("Staffeln");
            this.OnPropertyChanged("ZuletztGesehenStaffel");
            this.OnPropertyChanged("ZuletztGesehenFolge");
        }

        private void AddNeueStaffel(int AddDays) {

            Staffel St = this.ZuletztGesehenStaffel == null ? new Staffel(0) : this.ZuletztGesehenStaffel;
            this.Staffeln.Add(St.Next(DateTime.Today.AddDays(AddDays)));

            this.OnPropertyChanged("Staffeln");
            this.OnPropertyChanged("ZuletztGesehenStaffel");
            this.OnPropertyChanged("ZuletztGesehenFolge");
        }

        /// <summary>
        /// Name der Serie
        /// </summary>
        public string Name {
            get { return this.m_Name; }
            set { 
                this.m_Name = value;
                this.OnPropertyChanged("Name");
            }
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
            get { return this.Staffeln.OrderByDescending(Current => Current.ZuletztGesehenFolge.ZeitpunktGesehenZuletzt).ThenByDescending(Current => Current.Nummer).FirstOrDefault(); }
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

        protected virtual void OnPropertyChanged(string PropertyName) {
            Helper.VerifyPropertyName(this, PropertyName);
            PropertyChangedEventHandler Handler = this.PropertyChanged;
            if (Handler != null) {
                Handler(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public object Clone() {
            return new Serie(this.Name, this.Staffeln.Clone());
        }

        #endregion

        public const string XmlAttrName = "Name";



    }
}
