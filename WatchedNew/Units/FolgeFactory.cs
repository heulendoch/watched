using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Core.Units {
    public class FolgeFactory {

        public static Folge FromXElement(XElement Current) {

            int Nummer = Convert.ToInt32(Current.Attribute(Folge.XmlAttrNummer).Value);
            string Name = Current.Attribute(Folge.XmlAttrName).Value;

            List<DateTime> Gesehen = new List<DateTime>();
            foreach (XElement InnerCurrent in Current.Elements("D")) {
                Gesehen.Add(new DateTime(Convert.ToInt64(InnerCurrent.Attribute("T").Value)));
            }

            return new Folge(Nummer, Gesehen, Name);
        }


    }
}
