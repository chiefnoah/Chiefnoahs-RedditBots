using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RedditBots {
    public class PixivHandler : AbstractHandler {

        public const string PIXIV_BASE_URL = "https://public-api.secure.pixiv.net/v1";
        protected const string PIXIV_OAUTH_URL = "https://oauth.secure.pixiv.net/auth/token";
        public const string PIXIV_USER_URL = "http://www.pixiv.net/member.php?id=";

        //See here for where I got these: https://github.com/upbit/pixivpy/blob/master/pixivpy3/api.py
        protected const string PIXIV_CLIENT_ID = "bYGKuGVw91e0NMfPGp44euvGt59s";
        protected const string PIXIV_CLIENT_SECRET = "HP3RmkgAmEGro0gn1x9ioawQE8WMfvLXDz3ZqxpK";

        private const string CONFIG_FILENAME = "PixivAuthentication.xml";

        private string token;

        private int retries = 0;

        PixivUser user;
        public PixivHandler() {
            user = LoadConfig<PixivUser>(CONFIG_FILENAME);
        }


        public PixivWorksResponse.Response GetPixivWork(int id) {
            if (token == null || token == "") {
                Authenticate();
            }
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(PIXIV_BASE_URL + "/works/" + id + ".json");
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", "Bearer " + token);
            request.Headers = headers;

            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
                    string jsonstring = reader.ReadToEnd();
                    PixivWorksResponse worksRes = JsonConvert.DeserializeObject<PixivWorksResponse>(jsonstring);
                    if (worksRes.status != "success") {
                        retVal += "\r\nNo Pixiv Works result. Bad ID? ID: " + id;
                        return null;
                    }
                    retries = 0;
                    return worksRes.response[0]; //A works request should always only have 1 result
                }
            } catch (WebException e) {
                if (e.Status == WebExceptionStatus.ProtocolError) {
                    if (retries < 3) {
                        retVal += "\r\nToken either missing or expired. Retry " + retries;
                        retries++;
                        if (Authenticate() != null) {
                            retVal += "\r\nAuthenticated with Pixiv";
                        }
                        return GetPixivWork(id);
                    }
                }
                retVal += "\r\nUnable to reach Pixiv";
                return null;
            }
        }

        /// <summary>
        /// Gets OAuth access token from Pixiv APi
        /// </summary>
        /// <returns>OAuth Access token</returns>
        private string Authenticate() {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(PIXIV_OAUTH_URL);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Referer = "http://pixiv.net";

            string parameters = "&username=" + user.Username + "&password=" + user.Password + "&grant_type=password&client_id=" + PIXIV_CLIENT_ID + "&client_secret=" + PIXIV_CLIENT_SECRET;

            using (StreamWriter stout = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8)) {
                stout.Write(parameters);
            }
            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
                    string jsonstring = reader.ReadToEnd();
                    PixivAuthResponse pixivAuthResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PixivAuthResponse>(jsonstring);
                    token = pixivAuthResponse.response.access_token;
                    return token;
                }
            } catch (WebException) {
                retVal += "\r\nUnable to Authenticate with Pixiv";
                token = null;
                return null;
            }
        }
    }
}
