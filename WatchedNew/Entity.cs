using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core {

    public abstract class Entity : INotifyPropertyChanged {

        public const string XmlAttrName = "Name";


        /// <summary>
        /// Name der Entität
        /// </summary>
        private string m_Name;


        public event PropertyChangedEventHandler PropertyChanged;


        public Entity() {


        }

        /// <summary>
        /// Name der Entität
        /// </summary>
        public string Name {
            get { return this.m_Name; }
            set {
                this.m_Name = value;
                this.OnPropertyChanged();
            }
        }

        [DebuggerStepThrough]
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null) {
            Helper.VerifyPropertyName(this, PropertyName);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }




    }
}
