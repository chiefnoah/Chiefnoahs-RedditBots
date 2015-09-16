using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using RedditSharp;
using RedditSharp.Things;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Mono.Data.Sqlite;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace RedditBots {

    public abstract class AbstractBot : AbstractHandler {

        protected const string BOT_ADMIN = "chiefnoah";


        protected Reddit reddit;
        protected AuthenticatedUser user;

        protected const string IQDB = "http://iqdb.org/";
        protected const string PIXIV_BASE_URL = "https://public-api.secure.pixiv.net/v1";
        protected const string SAUCE_NAO_BASE_URL = "http://saucenao.com/search.php";
        protected const string SAUCE_NAO_API_KEY = "07c14d9e56055d3f9119b4fba0cef10b42824057";
        protected const string PIXIV_CLIENT_ID = "bYGKuGVw91e0NMfPGp44euvGt59s";
        protected const string PIXIV_CLIENT_SECRET = "HP3RmkgAmEGro0gn1x9ioawQE8WMfvLXDz3ZqxpK";

        //Handlers
        AniDBHandler anidbHandler;
        DanbooruHandler danbooruHandler;
        IQDBHandler iqdbHandler;
        MyAnimeListHandler myanimelistHandler;
        PixivHandler pixivHandler;
        SauceNAOHandler saucenaoHandler;

        DatabaseHandler databaseHandler;

        public AbstractBot() {
            reddit = new Reddit();

            databaseHandler = new DatabaseHandler();
            //TODO: find a way to gaurantee the database connection gets closed
        }

        /// <summary>
        /// The main entry method for a bot. This should return a string for logging.
        /// </summary>
        /// <returns>Formatted string for logging</returns>
        abstract public string Run();

        /// <summary>
        /// Saves a post into a database.
        /// </summary>
        /// <param name="botId">ID of the bot.</param>
        /// <param name="botName">Name of the bot. This is usually the Reddit username of the bot.</param>
        /// <param name="postID">ID (in base 36) of a Reddit post.</param>
        protected void SavePost(int botId, string botName, string postID) {
            string sql = "REPLACE INTO BotData (botId, botName, postID) VALUES(" + botId + ", \"" + botName + "\", \"" + postID + "\")";
            databaseHandler.ExecuteSQLNonQuery(sql);
        }

        /// <summary>
        /// Returns whether the provided postID has been saved to table botname
        /// </summary>
        /// <param name="botID">ID of the bot</param>
        /// <param name="postID">ID of the Reddit post to search for</param>
        /// <returns></returns>
        protected Boolean CheckIfPostSaved(int botID, string postID) {
            string sql = "SELECT EXISTS(SELECT * FROM BotData WHERE botId=" + botID + " AND postId =\"" + postID + "\");";

            using (SqliteDataReader reader = databaseHandler.ExecuteSQLQuery(sql)) {
                //We don't actually want to read the results, we just want to check if the object exists
                while (reader.Read()) {
                    bool exists = reader.GetBoolean(0);
                    if (exists) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns whether the provided postID has been saved to table botname
        /// </summary>
        /// <param name="botID">ID of the bot</param>
        /// <param name="postID">ID of the Reddit post to search for</param>
        /// <returns></returns>
        protected Boolean CheckIfPostSaved(string postID) {
            string sql = "SELECT EXISTS(SELECT * FROM BotData WHERE postId =\"" + postID + "\");";

            using (SqliteDataReader reader = databaseHandler.ExecuteSQLQuery(sql)) {
                //We don't actually want to read the results, we just want to check if the object exists
                while (reader.Read()) {
                    bool exists = reader.GetBoolean(0);
                    if (exists) {
                        return true;
                    }

                }
            }
            return false;
        }


        /// <summary>
        /// Searches the posts comments for the source url and returns the ID from the url
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        protected string GetPixivIdFromComments(RedditSharp.Things.Post post) {
            string regexPattern = @"illust_id=\d+";

            foreach (var comment in post.Comments) {
                if ((comment.Author == post.Author.ToString() || comment.Author == "SauceHunt") && comment.Body.Contains("illust_id=")) {
                    Match match = Regex.Match(comment.Body, regexPattern);
                    if (match.Success) {
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
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(SAUCE_NAO_BASE_URL + "?url=" + imageUrl + "&api_key=" + SAUCE_NAO_API_KEY + "&output_type=2");
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Stream str = res.GetResponseStream();
                StreamReader strr = new StreamReader(str, Encoding.UTF8);
                string jsonStr = strr.ReadToEnd();
                strr.Close();
                JObject o = JObject.Parse(jsonStr);
                Single similarity = float.Parse((string)o["results"][0]["header"]["similarity"], CultureInfo.InvariantCulture);
                if (similarity > 60f) {
                    return (string)o["results"][0]["data"]["pixiv_id"];
                }
                retVal += "\r\nCould not find pixiv source for: " + imageUrl;
            } catch (WebException e) {
                retVal += "SauceNAO Error: " + e.Message;
                return null;
            } catch (JsonReaderException) {
                retVal += "\r\nSauceNao Error: No source or not parsable image: " + imageUrl;
                return null;
            }
            return null; //This should *never* be reached
        }

        /// <summary>
        /// Replaces the deprecated (and removed) pixiv mobile API. Now uses the new pAPI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected List<string> GetImageTagsByPixivId(string id) {
            if (id == null) {
                return new List<string>();
            }
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(PIXIV_BASE_URL + "/works/" + id + ".json");
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
            } catch (WebException) {
                retVal += "\r\nUnable to retrieve Pixiv tags from ID " + id;
                return new List<string>();
            }

            PixivWorksResponse works = JsonConvert.DeserializeObject<PixivWorksResponse>(jsonOutput);

            //Now fails out if the image is an album. This will prevent false positives.
            if (works.status == "success" && works.response[0].page_count < 2) {
                return works.response[0].tags.ToList();
            }
            return new List<string>();
        }


        protected string GetPixivAccessToken() {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://oauth.secure.pixiv.net/auth/token");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Referer = "http://pixiv.net";


            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PixivUser));
            string filepath = "./Bots/Data/PixivAuthentication.xml";
            System.IO.StreamReader file = new System.IO.StreamReader(filepath);
            PixivUser user = new PixivUser();
            user = (PixivUser)reader.Deserialize(file);

            string parameters = "&username=" + user.Username + "&password=" + user.Password + "&grant_type=password&client_id=" + PIXIV_CLIENT_ID + "&client_secret=" + PIXIV_CLIENT_SECRET;

            using (StreamWriter stOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.UTF8)) {
                stOut.Write(parameters);
            }

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream stream = res.GetResponseStream();
            StreamReader sreader = new StreamReader(stream, Encoding.UTF8);
            string jsonOutput = sreader.ReadToEnd();

            PixivAuthResponse jsonRes = JsonConvert.DeserializeObject<PixivAuthResponse>(jsonOutput);
            //Cleanup

            res.Close();
            stream.Close();
            sreader.Close();
            return jsonRes.response.access_token;
        }

        /// <summary>
        /// Returns a list of -booru tags queried from IQDB with the imageUrl argument
        /// </summary>
        /// <param name="imageUrl">URL for the image to query for</param>
        /// <returns></returns>
        public List<string> getBooruTags(string imageUrl) {
            if (imageUrl == null || imageUrl.Length < 1) {
                return null;
            }
            //REGEX: (?<=Tags: )[^\"]*(?=\")

            //Set up the web request
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(IQDB);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Host = "iqdb.org";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.155 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Accept-Encoding", "gzip, deflate");
            headers.Add("Origin", "chrome-extension://aejoelaoggembcahagimdiliamlcdmfm");
            headers.Add("Accept-Language", "en-US,en;q=0.8,ja;q=0.6");

            //request.Expect = null;
            request.Headers.Add(headers);
            
            //Build the parameters
            //string postData = "url=" + imageUrl + "&service[]=1&service[]=3&service[]=4&service[]=11";
            NameValueCollection formData = HttpUtility.ParseQueryString(String.Empty);
            formData.Add("file", "null");
            formData.Add("forcegray", "0");
            formData.Add("url", imageUrl);
            formData.Add("service[]", "1");
            formData.Add("service[]", "3");
            formData.Add("service[]", "4");
            formData.Add("service[]", "11");
            string postData = formData.ToString();
            //Write the parameters to the request stream
            using (StreamWriter stOut = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8)) {
                stOut.Write(postData);
                stOut.Close(); //I think this is unnecessary because of the using thingy
            }

            string htmlPage;
            HttpWebResponse res = (HttpWebResponse)request.GetResponse();
            using(Stream stream = res.GetResponseStream())
            using (StreamReader sreader = new StreamReader(stream, Encoding.GetEncoding(res.CharacterSet))) {
                htmlPage = sreader.ReadToEnd();
            }
            //Return an empty list of strings if something happens
            if (htmlPage.Contains("No relevant matches")) {
                return new List<string>();
            }
            Regex reg = new Regex("(?<=Tags: )[^\"]*(?=\")", RegexOptions.None);
                Match match = reg.Match(htmlPage);
                Group group = match.Groups[0];
                string tagString = match.Value.ToString();
                char[] delimeter = {' '};

                List<string> tags = tagString.Split(delimeter, StringSplitOptions.RemoveEmptyEntries).ToList();
                tags = tags.Distinct().ToList();

            
            return tags;
        }


        /// <summary>
        /// Scans the bots inbox and forwards them 
        /// </summary>
        protected void ForwardInbox(int botId, string botName, string botPassword) {
            if (user != null) {
                foreach (var m in user.UnreadMessages.Take(5)) {
                    //Console.WriteLine("Found unread messages\r\nKind: " + m.Kind);
                    //t1 = comment
                    if (m.Kind == "t1") {
                        RedditSharp.Things.Comment comment = (RedditSharp.Things.Comment)m;
                        if (!CheckIfPostSaved(botId, comment.Id)) {
                            string message = "From: " + comment.Author +
                                "\r\n\r\nSubreddit: " + comment.Subreddit +
                            "\r\n\r\nContent: " + comment.Body;
                            SendMessageToAdmin(botId, botName, botPassword, BOT_ADMIN, message);
                            SavePost(botId, botName, comment.Id);
                            retVal += "\r\nForwarded reply to administrator";
                        }
                    }
                }
            }
        }

        protected void SendMessageToAdmin(int botId, string botName, string botPassword, string to, string message) {
            if (user.Name != botName) {
                user = reddit.LogIn(botName, botPassword);
            }
            reddit.ComposePrivateMessage("Bot Admin Message", message, to);
        }
    }
}
