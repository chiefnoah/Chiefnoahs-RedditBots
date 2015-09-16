using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RedditBots {
    public class SauceNAOHandler : AbstractHandler {

        private const string CONFIG_FILENAME = "SauceNAOConfig.xml";
        protected const string SAUCE_NAO_BASE_URL = "http://saucenao.com/search.php";

        private SauceNAOConfig config;

        public SauceNAOHandler() {
            config = LoadConfig<SauceNAOConfig>(CONFIG_FILENAME);
        }

        public int GetPixivIdFromUrl(string url) {
            try {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(SAUCE_NAO_BASE_URL + "?url=" + url + "&api_key=" + config.User.api_key + "&output_type=2");
                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                using (Stream str = res.GetResponseStream())
                using (StreamReader strr = new StreamReader(str, Encoding.UTF8)) {
                    string jsonStr = strr.ReadToEnd();
                    strr.Close();
                    JObject o = JObject.Parse(jsonStr);
                    Single similarity = float.Parse((string)o["results"][0]["header"]["similarity"], CultureInfo.InvariantCulture);
                    if (similarity > 60f) {
                        return Int32.Parse((string)o["results"][0]["data"]["pixiv_id"]);
                    }
                    retVal += "\r\nCould not find pixiv source for: " + url;
                }
            } catch (WebException e) {
                retVal += "SauceNAO Error: " + e.Message;
                return -1;
            } catch (JsonReaderException) {
                retVal += "\r\nSauceNao Error: No source or not parsable image: " + url;
                return -1;
            }
            return -1; //This should *never* be reached
        }

    }
}
