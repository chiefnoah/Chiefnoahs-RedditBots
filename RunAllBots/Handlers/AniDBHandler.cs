using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RedditBots {
    public class AniDBHandler : AbstractHandler {
        private const string ANI_DB_DUMP_URL = "http://anidb.net/api/anime-titles.xml.gz";
        private const int MAX_DB_AGE = 168; //Maximum age of database in hours

        DatabaseHandler databaseHandler;

        public AniDBHandler() {
            databaseHandler = new DatabaseHandler();
            CheckUpdateDB();
        }

        public AniDBHandler(DatabaseHandler databaseHandler) {
            this.databaseHandler = databaseHandler;
            CheckUpdateDB();
        }

        /// <summary>
        /// Searches AniDB anime titles for the text passed
        /// </summary>
        /// <param name="title">Text to search for in anime titles</param>
        /// <returns>List of anime objects</returns>
        public List<animetitlesAnime> SearchTitle(string title) {
            List<animetitlesAnime> anime = new List<animetitlesAnime>();
            string sql = "SELECT * FROM AnimeTitle WHERE animeID IN (SELECT animeID FROM AnimeTitle WHERE title LIKE \"%" + title + "%\") ORDER BY animeID";
            using (SqliteDataReader reader = databaseHandler.ExecuteSQLQuery(sql)) {
                if (reader.HasRows) {
                    animetitlesAnime lastAnime = new animetitlesAnime();
                    List<animetitlesAnimeTitle> lastTitles = new List<animetitlesAnimeTitle>();
                    while (reader.Read()) {
                        int aid = (int)reader.GetInt32(1); //First result is the animeID
                        //If the current animeId is not the same aid as the lastAnime,
                        //add that anime to the list and re-initialize lastAnime
                        if (aid != lastAnime.aid && lastAnime.aid != 0) {
                            lastAnime.title = lastTitles.ToArray(); //We have to do this because it'd be more work to resize the array every time another title is added
                            anime.Add(lastAnime);
                            lastAnime = new animetitlesAnime();
                        }
                        //This is only useful if it's a new anime
                        lastAnime.aid = (ushort)aid;
                        //Initialize a new title object to be added to the anime
                        animetitlesAnimeTitle newTitle = new animetitlesAnimeTitle();
                        newTitle.type = reader["type"].ToString(); //type
                        newTitle.lang = reader["language"].ToString(); //xml:lang
                        newTitle.Value = reader["title"].ToString(); //Value

                        lastTitles.Add(newTitle);
                    }
                    //Cuts off last result if we don't do this here
                    lastAnime.title = lastTitles.ToArray();
                    anime.Add(lastAnime);
                }
            }
            return anime;
        }

        /// <summary>
        /// Searches AniDB anime titles for all tags in IEnumerable passed
        /// </summary>
        /// <param name="titles">IEnumerable of strings to search for in anime titles</param>
        /// <returns>List of anime objects</returns>
        public List<animetitlesAnime> SearchTitleList(IEnumerable<string> titles) {
            List<animetitlesAnime> anime = new List<animetitlesAnime>();

            string sql = "SELECT * FROM AnimeTitle WHERE animeID IN (SELECT animeID FROM AnimeTitle WHERE ";
            foreach (string t in titles) {
                sql += "title LIKE \"%" + t + "%\" OR ";
            }
            sql = sql.Remove(sql.LastIndexOf(" OR"));
            sql += ") ORDER BY animeID";
            using (SqliteDataReader reader = databaseHandler.ExecuteSQLQuery(sql)) {
                if (reader.HasRows) {
                    animetitlesAnime lastAnime = new animetitlesAnime();
                    List<animetitlesAnimeTitle> lastTitles = new List<animetitlesAnimeTitle>();
                    while (reader.Read()) {
                        int aid = (int)reader.GetInt32(1); //First result is the animeID
                        //If the current animeId is not the same aid as the lastAnime,
                        //add that anime to the list and re-initialize lastAnime
                        if (aid != lastAnime.aid && lastAnime.aid != 0) {
                            lastAnime.title = lastTitles.ToArray(); //We have to do this because it'd be more work to resize the array every time another title is added
                            anime.Add(lastAnime);
                            lastAnime = new animetitlesAnime();
                        }
                        //This is only useful if it's a new anime
                        lastAnime.aid = (ushort)aid;
                        //Initialize a new title object to be added to the anime
                        animetitlesAnimeTitle newTitle = new animetitlesAnimeTitle();
                        newTitle.type = reader["type"].ToString(); //type
                        newTitle.lang = reader["language"].ToString(); //xml:lang
                        newTitle.Value = reader["title"].ToString(); //Value

                        lastTitles.Add(newTitle);
                    }
                    //Cuts off last result if we don't do this here
                    lastAnime.title = lastTitles.ToArray();
                    anime.Add(lastAnime);
                }
            }
            return anime;
        }


        private void CheckUpdateDB() {
            //Check if the XML database we have is older than a week.
            if (!IsBelowThreshold("anime-titles.xml", MAX_DB_AGE) || !System.IO.File.Exists("anime-titles.xml")) {
                Console.WriteLine("DB out of date. Downloading latest version...");

                //Download latest file
                WebClient Client = new WebClient();
                Client.DownloadFile(ANI_DB_DUMP_URL, "anime-titles.xml.gz");

                //Decompress the downloaded file. Delete the archive when done
                DeCompressFile("anime-titles.xml.gz", "anime-titles.xml");
                System.IO.File.Delete("anime-titles.xml.gz");

                //TODO: Check if there were any changes to the XML database before readding it to the sqlite database
                AddXMLToDB();
            }
        }

        /// <summary>
        /// Converts the XML database downloaded from AniDB.net to a sqlite database for faster querying
        /// </summary>
        public void AddXMLToDB() {
            try {
                using (SqliteCommand command = new SqliteCommand(databaseHandler.GetConnection()))
                using (SqliteTransaction transaction = databaseHandler.StartSQLTransaction()) {
                    //Delete all records from the database. It's impossible to know exactly what was added
                    //So we just delete everything and start over. It's inefficient, but 
                    command.CommandText = "DELETE FROM AnimeTitle";
                    command.ExecuteNonQuery();

                    //Load the XML file into an object
                    animetitles AniDB = new animetitles();
                    XmlSerializer reader = new XmlSerializer(typeof(animetitles));
                    using (StreamReader file = new StreamReader("anime-titles.xml")) {
                        AniDB = (animetitles)reader.Deserialize(file);
                    }
                    command.CommandText = "INSERT INTO AnimeTitle(animeID, type, language, title) VALUES(@aid, @type, @lang, @title)";
                    //This might be more efficient with a sql transaction?
                    foreach (animetitlesAnime a in AniDB.anime) {
                        foreach (animetitlesAnimeTitle t in a.title) {
                            //Inserts the title as a new row

                            command.Parameters.Add(new SqliteParameter("@aid", a.aid));
                            command.Parameters.Add(new SqliteParameter("@type", t.type));
                            command.Parameters.Add(new SqliteParameter("@lang", t.lang));
                            command.Parameters.Add(new SqliteParameter("@title", t.Value));
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            } catch (SqliteException e) {
                //TODO: error handling
                Console.WriteLine("AniDB sqlite error: " + e.Message);
            }
        }

        private bool IsBelowThreshold(string filename, int hours) {
            var threshold = DateTime.Now.AddHours(-hours);
            return System.IO.File.GetCreationTime(filename).AddHours(-hours) <= threshold;
        }



        private static void DeCompressFile(string CompressedFile, string DeCompressedFile) {
            byte[] buffer = new byte[1024 * 1024];

            using (System.IO.FileStream fstrmCompressedFile = System.IO.File.OpenRead(CompressedFile)) {
                using (System.IO.FileStream fstrmDecompressedFile = System.IO.File.Create(DeCompressedFile)) {
                    using (System.IO.Compression.GZipStream strmUncompress = new System.IO.Compression.GZipStream(fstrmCompressedFile,
                            System.IO.Compression.CompressionMode.Decompress)) {
                        int numRead = strmUncompress.Read(buffer, 0, buffer.Length);

                        while (numRead != 0) {
                            fstrmDecompressedFile.Write(buffer, 0, numRead);
                            fstrmDecompressedFile.Flush();
                            numRead = strmUncompress.Read(buffer, 0, buffer.Length);
                        }

                        strmUncompress.Close();
                    } // End Using System.IO.Compression.GZipStream strmUncompress 

                    fstrmDecompressedFile.Flush();
                    fstrmDecompressedFile.Close();
                } // End Using System.IO.FileStream fstrmCompressedFile 

                fstrmCompressedFile.Close();
            } // End Using System.IO.FileStream fstrmCompressedFile 

        } // End
    }
}
