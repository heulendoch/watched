using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Core.Units;

namespace Core {

    public static class Extensions {

        public static bool Replace<T>(this Collection<T> Current, T Search, T Replace) {
            int Index = Current.IndexOf(Search);

            if (Index == -1) {
                return false;
            }
            
            Current.Remove(Search);
            Current.Insert(Index, Replace);

            return true;
        }

        public static ObservableCollection<T> Clone<T>(this ObservableCollection<T> Current) where T : ICloneable {

            ObservableCollection<T> NewCollection = new ObservableCollection<T>();
            foreach (T CurrentItem in Current) {
                NewCollection.Add((T)CurrentItem.Clone());
            }

            return NewCollection;

        }

        public static Folge ZuletztGesehen(this Collection<Folge> Current) {
            return Current.OrderByZuletztGesehenUndNummer().FirstOrDefault();
        }

        public static IOrderedEnumerable<Folge>OrderByZuletztGesehenUndNummer(this Collection<Folge> Current){
            return Current.OrderByDescending(InnerCurrent => InnerCurrent.ZeitpunktGesehenZuletzt).ThenByDescending(InnerCurrent => InnerCurrent.Nummer);
        }

        public static int NummerNeueFolge(this IEnumerable<Folge> Current) {
            return Current.OrderByDescending(InnerCurrent => InnerCurrent.Nummer).Select(InnerCurrent => InnerCurrent.Nummer).FirstOrDefault() + 1;
        }

        public static void AddRange(this Collection<Folge> Current, IEnumerable<int> Einträge) {
            foreach (int Number in Einträge) {
                Current.Add(new Folge(Number, false));
            }
        }

    }
}
