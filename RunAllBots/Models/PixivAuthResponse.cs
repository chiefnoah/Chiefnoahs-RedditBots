using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunAllBots.PixivResponse {
    public class Auth {
            public Response response { get; set; }

        public class Response {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string scope { get; set; }
            public string refresh_token { get; set; }
            public User user { get; set; }
        }

        public class User {
            public Profile_Image_Urls profile_image_urls { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string account { get; set; }
        }

        public class Profile_Image_Urls {
            public string px_16x16 { get; set; }
            public string px_50x50 { get; set; }
            public string px_170x170 { get; set; }
        }
    }
}
