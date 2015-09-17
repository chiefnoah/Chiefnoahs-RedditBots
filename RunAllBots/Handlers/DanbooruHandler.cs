using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace RedditBots {
    public class DanbooruHandler : AbstractHandler{
        private const string DANBOORU_URL = "http://danbooru.donmai.us/";


        /// <summary>
        /// Constructor
        /// </summary>
        public DanbooruHandler() {

        }


        /// <summary>
        /// Returns a JSON data from the danbooru post based on the id
        /// </summary>
        /// <param name="id">post ID</param>
        /// <returns>string of JSON data</returns>
        /// <exception cref="WebException">Invalid ID or unable to reach Danbooru</exception>
        public string GetPostJsonFromId(int id) {
            Uri uri = new Uri(DANBOORU_URL + "posts/" + id + ".json");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.UserAgent = "WhichMoe? - /u/chiefnoah";
            request.Method = "GET";

            try {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            } catch (WebException e) {
                return null;
            }
        }

        private DanbooruPost getPost(string json) {
            try {
                DanbooruPost post = JsonConvert.DeserializeObject<DanbooruPost>(json);
                return post;
            } catch (InvalidOperationException) {
                //This means we didn't get the right JSON data back
                return null;
            }
        }

        public DanbooruPost getPost(int id) {
            return getPost(GetPostJsonFromId(id));
        }
    }
}
