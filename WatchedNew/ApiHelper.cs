using Core.Units;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TMDbLib.Client;
using TMDbLib.Objects;
using TMDbLib.Objects.TvShows;
using TMDbLib.Objects.General;

namespace Core {
    public class ApiHelper {

        public static ApiHelper Global = new ApiHelper();

        private const string API = "7f18d06082f60683fa7a878a396a617c";

        private TMDbClient m_Client = null;

        private ApiHelper() { 
            this.Client = new TMDbClient(ApiHelper.API);
            TMDbConfig Config = new TMDbConfig();
            this.Client.GetConfig();
        }

        public TMDbClient Client {
            get { return m_Client; }
            private set { m_Client = value; }
        }

        /// <summary>
        /// Lädt die Serie mit allen Staffeln und allen Folgen
        /// </summary>
        /// <param name="ShowID"></param>
        /// <param name="AddEmptySeasons"></param>
        /// <returns></returns>
        public Serie LoadSeries(int ShowID, bool AddEmptySeasons) {

            TvShow ApiShow = Client.GetTvShow(ShowID);

            Serie Show = new Serie(ApiShow.Name);
            foreach (TvSeason ApiSeason in ApiShow.Seasons) {
                Staffel Season = this.LoadStaffel(ShowID, ApiSeason.SeasonNumber);
                if ((!AddEmptySeasons && Season.Folgen.Count > 0) || AddEmptySeasons) {
                    Show.Staffeln.Add(Season);
                }
                
                
            }

            return Show;
        }

        /// <summary>
        /// Lädt die Staffel mit allen Folgen
        /// </summary>
        /// <param name="SerienID"></param>
        /// <param name="StaffelNummer"></param>
        /// <returns></returns>
        public Staffel LoadStaffel(int SerienID, int StaffelNummer) {

            if (SerienID < 1) {
                throw new ArgumentException();
            }

            if (StaffelNummer < 0) {
                throw new ArgumentException();
            }

            TvSeason ApiSeason = Client.GetTvSeason(SerienID, StaffelNummer, TvSeasonMethods.Undefined, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

            Staffel Season = new Staffel(ApiSeason.SeasonNumber, null, ApiSeason.Name);
            foreach (TvEpisode Episode in ApiSeason.Episodes) {
                Season.Folgen.Add(new Folge(Episode.EpisodeNumber, false, null, Episode.Name));
            }

            return Season;

        }

        public List<TvShowBase> Search(string Name) {
            SearchContainer<TvShowBase> Request = Client.SearchTvShow(Name);
            return Request.Results;
        }

    
    
    }
}
