using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace RedditBots {
    public class MyAnimeListHandler : AbstractHandler {

        MyAnimeListAccount user;

        private const string CONFIG_FILENAME = "MyAnimeListConfig.xml";
        private const string MYANIMELIST_MANGA_SEARCH_URL = "http://myanimelist.net/api/manga/search.xml?q=";
        private const string MYANIMELIST_ANIME_SEARCH_URL = "http://myanimelist.net/api/anime/search.xml?q=";

        public MyAnimeListHandler() {
            user = LoadConfig<MyAnimeListAccount>(CONFIG_FILENAME);
        }

        public MyAnimeListAnime SearchAnime(string q) {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(MYANIMELIST_ANIME_SEARCH_URL + q);
            request.Method = "GET";
            request.Accept = "application/xml";
            request.ContentType = "application/xml";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            //Encodes basicauth
            string auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user.username + ":" + user.password));
            request.Headers.Add("Authorization", "Basic " + auth);

            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
                    string xmlstring = WebUtility.HtmlDecode(reader.ReadToEnd());
                    xmlstring = xmlstring.Replace("<br />", "");
                    xmlstring = Regex.Replace(xmlstring, @"[\&\;]+", "");
                    XmlSerializer serializer = new XmlSerializer(typeof(MyAnimeListAnime));
                    StringReader stringReader = new StringReader(xmlstring);
                    MyAnimeListAnime anime = (MyAnimeListAnime)serializer.Deserialize(stringReader);
                    return anime;
                }
            } catch (WebException e) {
                
                if (e.Status == WebExceptionStatus.ProtocolError) {
                    retVal += "\r\nInvalid MyAnimeList credentials";
                } else {
                    retVal += "\r\nUnable to contact MyAnimeList";
                }
            }

            return null;
        }

        public MyAnimeListManga SearchManga(string q) {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(MYANIMELIST_MANGA_SEARCH_URL + q);
            request.Method = "GET";
            //Encodes basicauth
            string auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user.username + ":" + user.password));
            request.Headers.Add("Authorization", "Basic " + auth);

            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
                    string xmlstring = WebUtility.HtmlDecode(reader.ReadToEnd());
                    xmlstring = xmlstring.Replace("<br />", ""); //Removes HTML break tags
                    xmlstring = Regex.Replace(xmlstring, @"[\&\;]+", ""); //Removes & and ; because the escaped characters were being parsed
                    XmlSerializer serializer = new XmlSerializer(typeof(MyAnimeListManga));
                    using (StringReader stringReader = new StringReader(xmlstring)) {
                        MyAnimeListManga manga = (MyAnimeListManga)serializer.Deserialize(stringReader);
                        return manga;
                    }
                }
            } catch (WebException e) {
                retVal += "\r\nUnable to contact MyAnimeList";
            }

            return null;
        }

        static string RemoveInvalidXmlChars(string text) {
            var validXmlChars = text.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
            return new string(validXmlChars);
        }



    }
}
