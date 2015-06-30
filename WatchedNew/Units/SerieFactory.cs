using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Core.Units {
    public class SerieFactory {

        public static Serie FromXElement(XElement Current) {

            string Name = Current.Attribute(Staffel.XmlAttrName).Value;

            List<Staffel> Staffeln = new List<Staffel>();
            foreach (XElement InnerCurrent in Current.Element(Staffel.XmlListe).Elements(typeof(Staffel).Name)) {
                Staffeln.Add(StaffelFactory.FromXElement(InnerCurrent));
            }

            return new Serie(Name, Staffeln);
        }

    }
}
