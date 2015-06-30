using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Core.Units {
    public class StaffelFactory {

        public static Staffel FromXElement(XElement Current) {

            int Nummer = Convert.ToInt32(Current.Attribute(Staffel.XmlAttrNummer).Value);
            string Name = Current.Attribute(Staffel.XmlAttrName).Value;

            List<Folge> Folgen = new List<Folge>();
            foreach (XElement InnerCurrent in Current.Element(Folge.XmlListe).Elements(typeof(Folge).Name)) {
                Folgen.Add(FolgeFactory.FromXElement(InnerCurrent));
            }

            return new Staffel(Nummer, Folgen, Name);
        }

    }
}
