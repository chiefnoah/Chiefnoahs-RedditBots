using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Things;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Mono.Data.Sqlite;
using System.Data;
using System.Text.RegularExpressions;
using RunAllBots.PixivAuthentication;
using Newtonsoft.Json;

namespace RunAllBots {
    abstract class AbstractBot {

        protected string retVal;
        protected Reddit reddit;
        protected AuthenticatedUser user;

        protected const string PixivBaseUrl = "http://spapi.pixiv.net/iphone/illust.php";
        protected const string PixivPAPIBaseUrl = "https://public-api.secure.pixiv.net/v1";
        protected const string SauceNAOBaseUrl = "http://saucenao.com/search.php";
        protected const string SauceNAOAPIKey = "07c14d9e56055d3f9119b4fba0cef10b42824057";
        protected const string PixivClientID = "bYGKuGVw91e0NMfPGp44euvGt59s";
        protected const String PixivClientSecret = "HP3RmkgAmEGro0gn1x9ioawQE8WMfvLXDz3ZqxpK";

        IDbConnection dbConnection;

        public AbstractBot() {
            reddit = new Reddit();

            //Establish a connection to the database
            dbConnection = (IDbConnection)new SqliteConnection("Data Source=BotData.db,Version=3");
            dbConnection.Open();
            InitializeDatabase();
            //TODO: find a way to gaurantee the database connection gets closed
        }

        /// <summary>
        /// The main entry method for a bot. This should return a string for logging.
        /// </summary>
        /// <returns>Formatted string for logging</returns>
        abstract public String Run();


        /// <summary>
        /// Saves a post into a database.
        /// </summary>
        /// <param name="botId">ID of the bot.</param>
        /// <param name="botName">Name of the bot. This is usually the Reddit username of the bot.</param>
        /// <param name="postID">ID (in base 36) of a Reddit post.</param>
        protected void SavePost(int botId, string botName, string postID) {
            String sql = "REPLACE INTO BotData (botId, botName, postID) VALUES(" + botId + ", \"" + botName + "\", \"" + postID + "\")";

            IDbCommand dbcmd = dbConnection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();

            //Perform cleanup
            dbcmd.Dispose();
            dbcmd = null;
        }

        /// <summary>
        /// Returns whether the provided postID has been saved to table botname
        /// </summary>
        /// <param name="botID">ID of the bot</param>
        /// <param name="postID">ID of the Reddit post to search for</param>
        /// <returns></returns>
        protected Boolean CheckIfPostSaved(int botID, string postID) {
            String sql = "SELECT EXISTS(SELECT * FROM BotData WHERE botId=" + botID + " AND postId =\"" + postID + "\");";

            IDbCommand dbcmd = dbConnection.CreateCommand();
            dbcmd.CommandText = sql;
            IDataReader reader = dbcmd.ExecuteReader();

            //We don't actually want to read the results, we just want to check if the object exists
            while (reader.Read()) {
                bool exists = reader.GetBoolean(0);
                if (exists) {
                    reader.Close();
                    reader = null;
                    dbcmd.Dispose();
                    dbcmd = null;

                    return true;
                }

            }

            //Perform cleanup
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;

            return false;
        }

        /// <summary>
        /// Initializes the database.
        /// </summary>
        private void InitializeDatabase() {
            string sql = "CREATE TABLE IF NOT EXISTS BotData (id INTEGER PRIMARY KEY AUTOINCREMENT, botId INT, botName TEXT, postID TEXT)";

            IDbCommand dbcmd = dbConnection.CreateCommand();
            dbcmd.CommandText = sql;
            dbcmd.ExecuteNonQuery();

            //Perform cleanup
            dbcmd.Dispose();
            dbcmd = null;
        }


        /// <summary>
        /// Searches the posts comments for the source url and returns the ID from the url
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        protected string GetPixivIdFromComments(RedditSharp.Things.Post post) {
            string regexPattern = @"illust_id=\d+";

            foreach (var comment in post.Comments) {
                if((comment.Author == post.Author.ToString() || comment.Author == "SauceHunt") && comment.Body.Contains("illust_id=")) {
                    Match match = Regex.Match(comment.Body, regexPattern);
                    if(match.Success) {
                        return match.Value.Substring(10);
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Gets image source using SauceNAO.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns>ID</returns>
        protected string GetImageSourceId(string imageUrl) {
            try {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(SauceNAOBaseUrl + "?url=" + imageUrl + "&api_key=" + SauceNAOAPIKey + "&output_type=2");
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Stream str = res.GetResponseStream();
                StreamReader strr = new StreamReader(str, Encoding.UTF8);
                string jsonStr = strr.ReadToEnd();
                JObject o = JObject.Parse(jsonStr);
                Single similarity = float.Parse((string)o["results"][0]["header"]["similarity"], CultureInfo.InvariantCulture);
                if (similarity > 60f) {
                    return (string)o["results"][0]["data"]["pixiv_id"];
                }
                retVal += "\r\nCould not find source for: " + imageUrl;
            } catch (WebException e) {
                retVal += "SauceNAO Error: " + e.Message;
                int derp = e.HResult;
                Console.Write(derp);
                Console.Read();
                return "API Request quote maxed";
            }
            return null; //This should *never* be reached
        }

        /// <summary>
        /// Replaces the deprecated (and removed) pixiv mobile API. Now uses the new pAPI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected String[] GetImageTagsByPixivId(string id) {
            if (id == null) {
                return new string[] {};
            }
            HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(PixivPAPIBaseUrl + "/works/" + id + ".json");
            req.Method = "GET";
            req.ContentType = "application/x-www-form-urlencoded";

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", "Bearer " + GetPixivAccessToken());
            req.Headers = headers;

            string jsonOutput;
            try {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Stream stream = res.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                jsonOutput = reader.ReadToEnd();
            } catch (WebException e) {
                return new string[] {};
            }

            PixivResponse.Works works = JsonConvert.DeserializeObject<PixivResponse.Works>(jsonOutput);

            if (works.status == "success") {
                return works.response[0].tags;
            }
            return new string[] {};
        }


        protected string GetPixivAccessToken() {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://oauth.secure.pixiv.net/auth/token");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Referer = "http://pixiv.net";


            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(User));
            String filepath = "./Bots/Data/PixivAuthentication.xml";
            System.IO.StreamReader file = new System.IO.StreamReader(filepath);
            User user = new User();
            user = (User)reader.Deserialize(file);

            string parameters = "&username=" + user.Username + "&password=" + user.Password + "&grant_type=password&client_id=" + PixivClientID + "&client_secret=" + PixivClientSecret;

            using (StreamWriter stOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.UTF8)) {
                stOut.Write(parameters);
                stOut.Close();
            }

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream stream = res.GetResponseStream();
            StreamReader sreader = new StreamReader(stream, Encoding.UTF8);
            string jsonOutput = sreader.ReadToEnd();

            PixivResponse.Auth jsonRes = JsonConvert.DeserializeObject<PixivResponse.Auth>(jsonOutput);
            //Cleanup

            res.Close();
            stream.Close();
            sreader.Close();
            return jsonRes.response.access_token;
        }


        /// <summary>
        /// Scans the bots inbox and forwards them 
        /// </summary>
        protected void ForwardInbox(int botId, string botName) {
            if (user != null) {
                foreach (var m in user.UnreadMessages.Take(5)) {
                    //Console.WriteLine("Found unread messages\r\nKind: " + m.Kind);
                    //t1 = comment
                    if (m.Kind == "t1") {
                        var comment = (RedditSharp.Things.Comment)m;
                        if (!CheckIfPostSaved(botId, comment.Id)) {
                            reddit.ComposePrivateMessage("Bot Inbox Forward",
                                "From: " + comment.Author +
                                "\r\n\r\nSubreddit: " + comment.Subreddit +
                            "\r\n\r\nContent: " + comment.Body, "chiefnoah");
                            SavePost(botId, botName, comment.Id);
                        }
                    }
                }

            }
        }


    }
}
