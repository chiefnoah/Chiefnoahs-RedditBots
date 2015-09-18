using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

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
            if (json != null && json != "") {
                try {
                    DanbooruPost post = JsonConvert.DeserializeObject<DanbooruPost>(json);
                    return post;
                } catch (InvalidOperationException) {
                    //This means we didn't get the right JSON data back
                    return null;
                }
            } else {
                return null;
            }
        }

        public DanbooruPost getPost(int id) {
            return getPost(GetPostJsonFromId(id));
        }

        public static string ParseName(string tag) {
            if (tag.Length < 1) return tag;
            List<string> name = new List<string>();
            string[] parts = tag.Split('_');
            foreach (string word in parts) {
                //If the word contains a opening parenthesis, it means that word is the start of the title of the show the caracter is from.
                //Break the loop. Everything after that will be part of the show title
                if (word.Contains("(")) break;
                //Don't capitalize words like "the" and "a"
                switch(word) {
                    case "the":
                    case "a":
                    case "of":
                    case "an":
                    case "to":
                    case "from":
                    case "by":
                    case "on":
                        name.Add(word);
                        break;
                    default:
                        name.Add(word[0].ToString().ToUpper() + word.Substring(1)); //Capitalizes the first letter because it's (hopefully) part of a name
                        break;
                }
            }
            return String.Join(" ", name);
        }
    }
}
