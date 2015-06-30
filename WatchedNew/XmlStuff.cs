using Core;
using Core.Units;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Core {
    public static class XmlStuff {

        private const string XmlRoot = "Root";

        public static XDocument LoadXml() {

            XDocument Doc = null;

            using (FileStream FStream = new FileStream(Helper.DataFilePathDefault(), FileMode.Open)) {

                using (MemoryStream MemStream = new MemoryStream()) {
                    using (GZipStream Gzip = new GZipStream(FStream, CompressionMode.Decompress)) {
                        Gzip.CopyTo(MemStream);
                    }

                    string Text = Encoding.UTF8.GetString(MemStream.ToArray());

                    string BOM = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    if (Text.StartsWith(BOM)) {
                        Text = Text.Remove(0, BOM.Length);
                    }
                    
                    Doc = XDocument.Parse(Text);
                }
            }

            return Doc;
        }

        public static bool Load(ref ObservableCollection<Serie> Serien) {
            if (File.Exists(Helper.DataFilePathDefault())) {

                try {
                    XDocument DocLoad = LoadXml();

                    foreach (XElement Current in DocLoad.Root.Elements("Serie")) {
                        Serien.Add(SerieFactory.FromXElement(Current));
                    }

                    return true;
                }
                catch(Exception) {
                    return false;
                }
            }

            return true;
        }

        private static XDocument SaveXml(IEnumerable<Serie> Serien) {
            XDocument Doc = new XDocument();

            XElement Root = new XElement(XmlStuff.XmlRoot);
            foreach (Serie Current in Serien) {
                Root.Add(Current.ToXML());
            }

            Doc.Add(Root);

            return Doc;
        }

        public static void Save(IEnumerable<Serie> Serien) {

            XDocument Doc = SaveXml(Serien);

            using (MemoryStream MemStream = new MemoryStream()) {
                Doc.Save(MemStream);

                if (File.Exists(Helper.DataFilePathDefault())) {
                    string NewFileName = Path.Combine(Helper.FilePathDefault(), "data" + DateTime.Today.ToString("yyy-MM-dd") + ".gz");
                    File.Delete(NewFileName);
                    File.Move(Helper.DataFilePathDefault(), NewFileName);
                }

                using (FileStream FStream = new FileStream(Helper.DataFilePathDefault(), FileMode.Create)) {
                    using (GZipStream Gzip = new GZipStream(FStream, CompressionMode.Compress)) {
                        Gzip.Write(MemStream.ToArray(), 0, MemStream.ToArray().Length);
                    }
                }
            }
        }

        public static bool SaveRequired(IEnumerable<Serie> Serien) {
            
            string TextOfSaveXml = XmlStuff.SaveXml(Serien).ToString();

            if (!File.Exists(Helper.DataFilePathDefault())) {

                return !TextOfSaveXml.EndsWith(string.Format("<{0} />", XmlStuff.XmlRoot));
            }
            else {
                return XmlStuff.LoadXml().ToString() != TextOfSaveXml;
            }

            
        }

    }
}
