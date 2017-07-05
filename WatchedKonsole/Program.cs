using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Core.Units;
using System.Text.RegularExpressions;
using System.Globalization;
using Core;
using System.Collections;

namespace WatchedKonsole {
    class Program {
        static void Main(string[] args) {
            /*
            string XmlFile = @"C:\tmp\lost.xml";
            File.Delete(XmlFile);

            List<DateTime> Gesehen1 = new List<DateTime>();
            Gesehen1.Add(DateTime.Now);
            Gesehen1.Add(DateTime.Now.AddDays(1));
            Gesehen1.Add(DateTime.Now.AddDays(2));

            List<Folge> FStaffel1 = new List<Folge>();
            FStaffel1.Add(new Folge(1));
            FStaffel1.Add(new Folge(2, Gesehen1));
            FStaffel1.Add(new Folge(3));
            FStaffel1.Add(new Folge(4));

            List<Folge> FStaffel2 = new List<Folge>();
            FStaffel2.Add(new Folge(1));
            FStaffel2.Add(new Folge(2));
            FStaffel2.Add(new Folge(3));
            FStaffel2.Add(new Folge(4, Gesehen1));
            
            List<Staffel> LostStaffeln = new List<Staffel>();
            LostStaffeln.Add(new Staffel(1, FStaffel1));
            LostStaffeln.Add(new Staffel(2, FStaffel2));

            Serie CurrentSerie = new Serie("Lost", LostStaffeln);

            List<Folge> FStaffelX = new List<Folge>();
            FStaffel1.Add(new Folge(1));
            FStaffel1.Add(new Folge(2, Gesehen1));
            FStaffel1.Add(new Folge(3));
            FStaffel1.Add(new Folge(4));

            List<Staffel> HeroesStaffeln = new List<Staffel>();
            LostStaffeln.Add(new Staffel(1, FStaffelX));

            Serie CurrentSerie2 = new Serie("Heroes", HeroesStaffeln);



            XDocument Doc = new XDocument();
            Doc.Add(CurrentSerie.ToXML());
            Doc.Save(XmlFile);

            XDocument DocLoad = XDocument.Load(XmlFile);
            Serie Lost = SerieFactory.FromXElement(DocLoad.Root);
             * 
             * */


            List<string> Parsed = new List<string>();
            string[] Content = File.ReadAllLines(@"C:\Users\j.roeding\Desktop\Watched.txt");

            int Unmatched = 0;
            string SearchSerie1 = @"(.+) [sS] ([0-9]+) ?[xX] ?([0-9]+[-\/]?[0-9]*)  ?\(([^)]+).*";
            string SearchSerie2 = @"(.+) [sS] ([0-9]+) ?[xX] ?([0-9]+[-\/]?[0-9]*)";

            

            string Date = string.Empty;
            foreach (string LineForeach in Content) {

                string Line = LineForeach;

                // Leere bzw. unrelevante Zeilen überspringen
                if (string.IsNullOrWhiteSpace(Line) || Line == "-" || Line == "?")
                    continue;

                // Zeile enthält Datumsangabe (Datum an dem nachfolgende Einträge gesehen wurden)
                if (Regex.IsMatch(Line, @"^[0-9]+\.[0-9]+\.[0-9]+")) {
                    // Datumsangabe merken
                    Date = Line;
                    // Zur nächsten Zeile
                    continue;
                }
                
                // Reguläre Serien ermitteln und anpassen
                if (Regex.IsMatch(Line, SearchSerie1)) {
                    Line = Regex.Replace(Line, SearchSerie1, @"<Eintrag><Serie>$1</Serie><Staffel>$2</Staffel><Folgen>$3</Folgen><FolgenName>$4</FolgenName><Gesehen></Gesehen></Eintrag>");
                    Line = ParseGesehenText(Line, Date);
                }

                if(Regex.IsMatch(Line, SearchSerie2)){
                    Line = Regex.Replace(Line, SearchSerie2, @"<Eintrag><Serie>$1</Serie><Staffel>$2</Staffel><Folgen>$3</Folgen><FolgenName></FolgenName><Gesehen></Gesehen></Eintrag>");
                    Line = ParseGesehenText(Line, Date);
                }

                // Episoden 'Ausgaben' ermitteln und anpassen
                Get(ref Line, "E3", "E3 PK", Date);

                Get(ref Line, "Rocketbeans", "Almost Daily", Date);
                Get(ref Line, "Rocketbeans", @"Kino\+", Date);
                Get(ref Line, "Rocketbeans", @"Bohn Jour", Date);
                Get(ref Line, "Rocketbeans", @"#MoinMoin", Date);
                Get(ref Line, "Rocketbeans", @"Beans vs Halo", Date);
                Get(ref Line, "Rocketbeans", @"RocketBeans Gamescom", Date);
                Get(ref Line, "Rocketbeans", @"Zock'n'Talk", Date);
                Get(ref Line, "Rocketbeans", @"Super Investigativ", Date);
                Get(ref Line, "Rocketbeans", @"Philsofa", Date);

                Get(ref Line, "OR", @"Pelzig hält sich", Date);
                Get(ref Line, "OR", @"Heute Show", Date);
                Get(ref Line, "OR", @"Roche & Böhmermann", Date);
                Get(ref Line, "OR", @"Quarks & Co", Date);
                Get(ref Line, "OR", @"LateLine", Date);
                Get(ref Line, "OR", @"NEO MAGAZIN", Date);

                Get(ref Line, "Dragonball", @"DBZ", Date);
                Get(ref Line, "Dragonball", @"Dragonball", Date);

                Get(ref Line, "Giga", @"Radio Giga", Date);
                Get(ref Line, "Giga", @"Radio Giga Special", Date);
                Get(ref Line, "Giga", @"Radio Giga - Film", Date);
                Get(ref Line, "Giga", @"JFK", Date);
                Get(ref Line, "Giga", @"G-Log", Date);
                Get(ref Line, "Giga", @"Giga – Top 100 -", Date);
                Get(ref Line, "Giga", @"GIGA Mac Tech", Date);
                Get(ref Line, "Giga", @"Giga Web-TV", Date);



                Get(ref Line, "Game One", @"Plauschangriff", Date);
                Get(ref Line, "Game One", @"Game One", Date);
                
                Get(ref Line, "RPG Heaven", @"Ausgepackt", Date);
                Get(ref Line, "RPG Heaven", @"Gedankensprung", Date);
                Get(ref Line, "RPG Heaven", @"Weihnachts Gyros 2013", Date);

                Get(ref Line, "_Diverses", @"Random Encounter", Date);
                Get(ref Line, "_Diverses", @"\|\|Backup", Date);
                Get(ref Line, "_Diverses", @"AndroidTalk", Date);
                Get(ref Line, "_Diverses", @"CRIME Stammtisch", Date);
                Get(ref Line, "_Diverses", @"TechTalk", Date);
                Get(ref Line, "_Diverses", @"Stay Forever", Date);
                Get(ref Line, "_Diverses", @"FK\.TV", Date);
                Get(ref Line, "_Diverses", @"AreaCast", Date);
                Get(ref Line, "_Diverses", @"Konsolen & Konsorten", Date);
                Get(ref Line, "_Diverses", @"Postecke", Date);
                Get(ref Line, "_Diverses", @"TvTotal", Date);

                Get(ref Line, "_Hoerbuecher", @"Dan Brown – Inferno", Date);
                Get(ref Line, "_Hoerbuecher", @"Cory Doctorow – Little Brother", Date);

                Get(ref Line, "_Anime", @"Black Lagoon", Date);
                Get(ref Line, "_Anime", @"Detektiv Conan", Date);
                Get(ref Line, "_Anime", @"Highschool of the Dead", Date);
                Get(ref Line, "_Anime", @"Elfenlied", Date);
                Get(ref Line, "_Anime", @"Death Note", Date);

                Get(ref Line, "Kwobb", @"Kwappcast", Date);
                Get(ref Line, "Kwobb", @"Kwobbcast", Date);

                if (Line == LineForeach) {
                    Unmatched++;
                    Console.WriteLine(Date + "=>" + Line);
                }
                else {
                    Parsed.Add(Line);
                }


            }

            List<WatchedObj> Watched = new List<WatchedObj>();
            foreach (string CLine in Parsed) {
                XElement Current = XElement.Parse(CLine.Replace("&", "&amp;"));
                Watched.Add(new WatchedObj(
                    Current.Element("Serie").Value, 
                    Current.Element("Staffel").Value, 
                    Current.Element("Folgen").Value,
                    Current.Element("FolgenName").Value,
                    Current.Element("Gesehen").Value
                ));
            }

            /*
            List<WatchedObj> Custom = new List<WatchedObj>();
            foreach (WatchedObj C in Watched) {
                for (int i = 0; i < C.Folgen.Count; i++) {
                    Custom.Add(new WatchedObj(C.Serienname, C.Staffelnummer.ToString(), C.Folgen[i].ToString(), C.Folgenname, C.Gesehen.ToString("dd.MM.yyyy")));
                } 
            }*/

            List<Serie> Serien = new List<Serie>();

            int MehrfachGesehen = 0;

            foreach (var Current in Watched.GroupBy(Tmp => Tmp.Serienname)) { 
                //Console.WriteLine(Current.Key);

                List<Staffel> Staffeln = new List<Staffel>();

                foreach (var InnerCurrent in Current.GroupBy(Tmp => Tmp.StaffelGroupBy)) {
                    //Console.WriteLine("   " + InnerCurrent.Key);

                    List<WatchedObj> WObjListe = InnerCurrent.ToList();
                    if(WObjListe.Count > 0) {

                        List<Folge> FolgenUnbereinigt = new List<Folge>();

                        foreach (WatchedObj WOjb in WObjListe) {
                            foreach(int CurrentNummer in WOjb.Folgen){
                                FolgenUnbereinigt.Add(new Folge(CurrentNummer, false, new DateTime[] { WOjb.Gesehen }, WOjb.Folgenname)); 
                            }
                            //Console.WriteLine("       " + WOjb.Gesehen);
                        }

                        List<Folge> FolgenBereinigt = new List<Folge>();

                        foreach (var Group in FolgenUnbereinigt.GroupBy(GroupCurrent => GroupCurrent.ToString() + WObjListe[0].Staffelbezeichnung)) {
                            if (Group.Count() > 1) {

                                List<DateTime> Gesehen = new List<DateTime>();
                                foreach (Folge FCurrent in Group) {
                                    Gesehen.Add(FCurrent.ZeitpunktGesehen.First());
                                }
                                //Console.WriteLine(Current.Key + " => " + WObjListe[0].Staffelnummer + " => " + Group.Key);
                                //MehrfachGesehen++;
                                Folge New = Group.First();
                                New.ZeitpunktGesehen = new System.Collections.ObjectModel.ObservableCollection<DateTime>(Gesehen);
                                FolgenBereinigt.Add(New);
                            }
                            else {
                                FolgenBereinigt.Add(Group.First());
                            }
                        }

                        Staffeln.Add(new Staffel(WObjListe[0].Staffelnummer, FolgenBereinigt, WObjListe[0].Staffelbezeichnung));
                    }

                    
                }

                Serien.Add(new Serie(Current.Key, Staffeln));
            }

            XmlStuff.Save(Serien);

            Console.WriteLine(Unmatched);
            //Console.WriteLine("Mehrfach ==>" + MehrfachGesehen);
            Console.WriteLine("Fertig");
            Console.ReadKey();
        }



        public static string ParseGesehenText(string Text, string Date) {
            return Text.Replace("<Gesehen></Gesehen>", string.Format("<Gesehen>{0}</Gesehen>", Date));
        }

        public static bool Get(ref string Line, string Creator, string Ausstrahlunsgname, string Date) {

            bool Return = false;
            string SearchWith = "^" + Ausstrahlunsgname + @" ?#([0-9]+[-\/]?[0-9]*) ?\(([^)]+).*";
            if (Regex.IsMatch(Line, SearchWith, RegexOptions.IgnoreCase)) {
                Line = Regex.Replace(Line, SearchWith, @"<Eintrag><Serie>__Serie__</Serie><Staffel>__Staffel__</Staffel><Folgen>$1</Folgen><FolgenName>$2</FolgenName><Gesehen></Gesehen></Eintrag>");
                Return = true;
            }

            string SearchWithout = "^" + Ausstrahlunsgname + @" ?#([0-9]+[-\/]?[0-9]*)";
            if (Regex.IsMatch(Line, SearchWithout, RegexOptions.IgnoreCase)) {
                Line = Regex.Replace(Line, SearchWithout, @"<Eintrag><Serie>__Serie__</Serie><Staffel>__Staffel__</Staffel><Folgen>$1</Folgen><FolgenName></FolgenName><Gesehen></Gesehen></Eintrag>");
                Return = true;
            }

            if (Return) {
                Line = Line.Replace("__Serie__", Creator);
                Line = Line.Replace("__Staffel__", Ausstrahlunsgname);
                Line = ParseGesehenText(Line, Date);
            }

            return Return;
        }

    }

    public class WatchedObj {

        private string m_Serienname;

        private int m_Staffelnummer;

        private string m_Staffelbezeichnung;

        private List<int> m_Folgen;

        private string m_Folgenname;

        private DateTime m_Gesehen;

        public WatchedObj(string Serienname, string Staffelnummer, string Folgen, string Folgenname, string Gesehen) {

            this.Serienname = Serienname;

            int Staffel = int.MinValue;
            if (!int.TryParse(Staffelnummer, out Staffel)) {
                this.Staffelbezeichnung = Staffelnummer;
                Staffel = int.MinValue;
            }


            this.Staffelnummer = Staffel;

            this.Folgen = this.ParseFolgen(Folgen).ToList();
            this.Folgenname = Folgenname;
            this.Gesehen = DateTime.ParseExact(Gesehen, "dd'.'MM'.'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
        
        }

        public string Serienname {
            get { return this.m_Serienname; }
            set { this.m_Serienname = value; }
        }

        public int Staffelnummer {
            get { return m_Staffelnummer == int.MinValue ? 0 : this.m_Staffelnummer; }
            set { this.m_Staffelnummer = value; }
        }

        public string Staffelbezeichnung {
            get { return m_Staffelbezeichnung; }
            set { m_Staffelbezeichnung = value; }
        }

        public List<int> Folgen {
            get { return m_Folgen; }
            set { m_Folgen = value; }
        }

        public string StaffelGroupBy {
            get {
                return this.m_Staffelnummer == int.MinValue ? this.Staffelbezeichnung : this.m_Staffelnummer.ToString();
            }
        }

        public string Folgenname {
            get { return m_Folgenname; }
            set { m_Folgenname = value; }
        }

        public DateTime Gesehen {
            get { return m_Gesehen; }
            set { m_Gesehen = value; }
        }


        private IEnumerable<int> ParseFolgen(string Text) {

            Text = Text.Trim();

            if (Text.Contains('/')) {
                foreach (string Current in Text.Split('/')) {
                    yield return int.Parse(Current);
                }
            }
            else if (Text.Contains('-')) {
                string[] Split = Text.Split('-');
                int Start = int.Parse(Split[0]);
                int Stop = int.Parse(Split[1]);

                for (; Start <= Stop; Start++) {
                    yield return Start;
                }
            }
            else {
                yield return int.Parse(Text);
            }


        }

        public override string ToString() {
            return string.Format("N={0}, S#={1}, Folgen={2}", this.Serienname, this.Staffelnummer, string.Join("'", this.Folgen));
        }
    
    }
}
