using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunAllBots.PixivResponse {
    class Works {
        
public string status { get; set; }
public Response[] response { get; set; }
public int count { get; set; }

public class Response
{
public int id { get; set; }
public string title { get; set; }
public string caption { get; set; }
public string[] tags { get; set; }
public string[] tools { get; set; }
public Image_Urls image_urls { get; set; }
public int width { get; set; }
public int height { get; set; }
public object stats { get; set; }
public int publicity { get; set; }
public string age_limit { get; set; }
public string created_time { get; set; }
public string reuploaded_time { get; set; }
public User user { get; set; }
public bool is_manga { get; set; }
public bool is_liked { get; set; }
public int favorite_id { get; set; }
public int page_count { get; set; }
public string book_style { get; set; }
public string type { get; set; }
public object metadata { get; set; }
public object content_type { get; set; }
}

public class Image_Urls
{
public string small { get; set; }
}

public class User
{
public int id { get; set; }
public string account { get; set; }
public string name { get; set; }
public bool is_following { get; set; }
public bool is_follower { get; set; }
public bool is_friend { get; set; }
public object is_premium { get; set; }
public Profile_Image_Urls profile_image_urls { get; set; }
public object stats { get; set; }
public object profile { get; set; }
}

public class Profile_Image_Urls
{
public string px_50x50 { get; set; }
}

    }
}
