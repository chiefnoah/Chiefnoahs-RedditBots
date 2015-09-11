using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RedditBots


{
    class AniDB
    {
        private const string ANI_DB_DUMP_URL = "http://anidb.net/api/anime-titles.xml.gz";
        private const int MAX_DB_AGE = 168; //Maximum age of database in hours

        public AniDB()
        {

        }




        public List<string> SearchForTitles(List<string> searchTags)
        {
            List<string> titles = new List<string>();
            animetitles AniDB = new animetitles();

            //Load the XML database for querying
            XmlSerializer reader = new XmlSerializer(typeof(animetitles));
            using (StreamReader file = new StreamReader("anime-titles.xml"))
            {
                AniDB = (animetitles)reader.Deserialize(file);

            }

            titles.Add(AniDB.anime.All(db => searchTags.Any(s => db.title)))


                return titles;
        }





    }
}
