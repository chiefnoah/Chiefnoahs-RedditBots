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

namespace RunAllBots
{
    abstract class AbstractBot {

        protected string retVal;
        protected Reddit reddit;
        protected AuthenticatedUser user;

        protected const string PixivBaseUrl = "http://spapi.pixiv.net/iphone/illust.php";
        protected const string SauceNAOBaseUrl = "http://saucenao.com/search.php";
        protected const string SauceNAOAPIKey = "07c14d9e56055d3f9119b4fba0cef10b42824057";

        public AbstractBot() {
            reddit = new Reddit();
        }

        /// <summary>
        /// The main entry method for a bot. This should return a string for logging.
        /// </summary>
        /// <returns>Formatted string for logging</returns>
        abstract public String Run();


        /// <summary>
        /// Gets image source using SauceNAO.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns>ID</returns>
        public string GetImageSourceId(string imageUrl) {
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
            return null;
        }

        /// <summary>
        /// Returns an array of tags from the pixiv image
        /// </summary>
        protected String[] GetImageTagsByPixivId(string id) {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(PixivBaseUrl + "?illust_id=" + id);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream str = res.GetResponseStream();
            StreamReader strr = new StreamReader(str, Encoding.UTF8);
            string temp = strr.ReadToEnd().Replace("\"", "");

            string[] all = temp.Split(',');
            string[] tags = all[13].Split(' ');

            strr.Close();
            str.Close();
            res.Close();
            return tags;
        }


        /// <summary>
        /// Scans the bots inbox and forwards them 
        /// </summary>
        protected void ForwardInbox() {
            //TODO: Check inbox for messages.
            if (user != null) {
                foreach (var m in user.Inbox.Take(5)) {
                    if (m.Unread) {
                        reddit.ComposePrivateMessage("Bot Inbox Forward",
                            "From: " + m.Author +
                            "\r\nSubreddit: " + m.Subreddit +
                        "\r\n[Context](" + m.Shortlink + ")" +
                        "Content: " + m.BodyHtml, "chiefnoah");
                    }
                }
            }
            
            
        }
    }
}
