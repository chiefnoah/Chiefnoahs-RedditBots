using System;
using System.Linq;
using RedditSharp;
using RedditSharp.Things;
using Mono.Data.Sqlite;
using System.Text.RegularExpressions;

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
        //Only initialize the ones you actually want to use.
        //It can be slow to intialize some
        /*AniDBHandler anidbHandler;
        DanbooruHandler danbooruHandler;
        IQDBHandler iqdbHandler;
        MyAnimeListHandler myanimelistHandler;
        PixivHandler pixivHandler;
        SauceNAOHandler saucenaoHandler; */

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
        protected int GetPixivIdFromComments(RedditSharp.Things.Post post) {
            string regexPattern = @"illust_id=\d+";

            foreach (var comment in post.Comments) {
                if ((comment.Author == post.Author.ToString() || comment.Author == "SauceHunt") && comment.Body.Contains("illust_id=")) {
                    Match match = Regex.Match(comment.Body, regexPattern);
                    if (match.Success) {
                        return Int32.Parse(match.Value.Substring(10));
                    }
                }
            }
            return -1;
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
