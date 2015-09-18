using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace RedditBots {
    public class IQDBHandler : AbstractHandler {

        public const string IQDB_URL = "http://danbooru.iqdb.org/";

        public IQDBHandler() {

        }


        public List<string> GetIQDBTags(string url) {

            string htmlPage = GetSearchPage(url);

            //Return an empty list of strings if something happens
            if (htmlPage.Contains("No relevant matches")) {
                return new List<string>();
            }
            Regex reg = new Regex("(?<=Tags: )[^\"]*(?=\")", RegexOptions.None);
            Match match = reg.Match(htmlPage);
            Group group = match.Groups[0];
            string tagString = match.Value.ToString();
            char[] delimeter = { ' ' };

            List<string> tags = tagString.Split(delimeter, StringSplitOptions.RemoveEmptyEntries).ToList();
            tags = tags.Distinct().ToList();


            return tags;

        }

        public int GetDanbooruId(string url) {
            string htmlPage = GetPage(url);

            //Return an empty list of strings if something happens
            if (htmlPage.Contains("No relevant matches")) {
                retVal += "\r\nCould not find image on IQDB " + url;
                return -1;
            }

            Regex reg = new Regex(@"(http://danbooru.donmai.us/posts/)([\d]+)", RegexOptions.None);
            Match match = reg.Match(htmlPage);
            Group group = match.Groups[0];
            try {
                Uri uri = new Uri(match.Value);
                int firstId = Int32.Parse(uri.Segments.Last());
                return firstId;
            } catch (FormatException e) {
                retVal += "\r\nUnable to parse danbooru ID from IQDB page";
                return -1;
            }

        }

        private string GetSearchPage(string url) {
            if (url == null || url.Length < 1) {
                return "";
            }
            //REGEX: (?<=Tags: )[^\"]*(?=\")

            //Set up the web request
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(IQDB_URL);
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
            formData.Add("url", url);
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
            try {
                using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
                using (Stream stream = res.GetResponseStream())
                using (StreamReader sreader = new StreamReader(stream, Encoding.GetEncoding(res.CharacterSet))) {
                    htmlPage = sreader.ReadToEnd();
                    return htmlPage;
                }
            } catch (WebException e) {
                retVal += "\r\nUnable to reach IQDB " + url;
                return "";
            }
        }

        public string GetPage(string url) {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(IQDB_URL + "?url=" + url);
            request.Method = "GET";
            request.Accept = "*/*";
            //request.Host = "iqdb.org";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.155 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Accept-Encoding", "gzip, deflate");
            headers.Add("Origin", "chrome-extension://aejoelaoggembcahagimdiliamlcdmfm");
            headers.Add("Accept-Language", "en-US,en;q=0.8,ja;q=0.6");

            //request.Expect = null;
            request.Headers.Add(headers);

            string htmlPage;
            try {
                using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
                using (Stream stream = res.GetResponseStream())
                using (StreamReader sreader = new StreamReader(stream, Encoding.GetEncoding(res.CharacterSet))) {
                    htmlPage = sreader.ReadToEnd();
                    return htmlPage;
                }
            } catch (WebException e) {
                retVal += "\r\nUnable to reach IQDB " + url;
                return "";
            }
        }


    }
}
