using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polenter.Serialization;
using RedditSharp;
using RedditSharp.Things;
using RunAllBots;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


namespace RedditBots {

    class AutoTagBot : AbstractBot {
        private const string AniDBDumpUrl = "http://anidb.net/api/anime-titles.xml.gz";
        private const int MaxDBAgeHours = 168;



        public AutoTagBot() {
            CheckUpdateDB();
            reddit = new Reddit();
        }
        /// <summary>
        /// Runs the bot
        /// </summary>
        public override string Run() {

            //TODO: Add scanned images to a database so they aren't scanned again.
            if (users == null) {
                users = new SortedList<string, AuthenticatedUser>();
            }
            users.Add("AutoTagBot", reddit.LogIn("AutoTagBot", "11noah"));
            retVal = "";
            List<Post> untaggedPosts = GetUntaggedRedditPosts(users.Last().Value);

            int numOfPostsChecked = 0;
            //Checks if there were any untagged reddit posts.
            foreach (Post post in untaggedPosts) {
                if (!CheckIfPostSaved(1, post.Id)) {
                    numOfPostsChecked++;
                    //Gets the title of the anime the image is from. A very convoluted process...
                    String title = GetImageTitleTag(post.Url.ToString());
                    Console.WriteLine(post.Title + " -- Source: " + title);
                    //reddit.ComposePrivateMessage("Untagged post found", "An untagged post was found: " + post.Title, "chiefnoah");
                    //TODO: Flair post. Comment.
                    //post.SetFlair(title, "");
                    //post.Comment("*TODO:* \n\n* Autoflairbot messsage.\n\n* Add ability for users to message bot and have flair removed");
                    SavePost(1, "AutoTagBot", post.Id);
                }
            }
            retVal += "\r\nTotal scanned: " + numOfPostsChecked;
            Console.WriteLine("Done.");
            Console.Read();
            return retVal;
        }

        /// <summary>
        /// Returns the title of the anime an image is from.
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public string GetImageTitleTag(string URL) {
            string[] tags = null;
            //Gets the pixiv source ID using SauceNAO
            string pixivId = GetImageSourceId(URL);
            if (pixivId != null) {
                //Retrieves the tags from pixiv
                tags = GetImageTagsByPixivId(pixivId);
                //Iterates through all the tags, checks if they're the title of an anime or "original"
                foreach (string tag in tags) {
                    //Check if the tag says original
                    if (tag.Equals("オリジナル")) {
                        return "Original";
                    } else {
                        //Not original, so search for an anime title in the tags
                        string title = SearchForTitle(tag);
                        if (title != null) {
                            //We found a tag! Now do other stuff with it.
                            return title;
                        }
                    }

                }
            }
            retVal += "\r\nUnable to find pixiv tags for: " + URL;
            return null;
        }

        /// <summary>
        /// Returns all the posts that don't have a tag inside "[" or a flair set
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public List<Post> GetUntaggedRedditPosts(AuthenticatedUser user) {
            //Subreddit subreddit = reddit.GetSubreddit("/r/chiefnoahstests");
            Subreddit subreddit = reddit.GetSubreddit("/r/awwnime");

            List<RedditSharp.Things.Post> untaggedPosts = new List<RedditSharp.Things.Post>();
            //Check if the posts is tagged or if there is already a flair set and if it's a link post
            foreach (var post in subreddit.New.Take(30)) {
                if (post.Title.IndexOf("[") < 0 && (post.LinkFlairText == "" || post.LinkFlairText == null) && !post.IsSelfPost) {
                    //Check if the post is hosted properly
                    if (post.Domain == "cdn.awwni.me" || post.Domain == "i.imgur.com" || post.Domain == "redditbooru.com") {
                        untaggedPosts.Add(post);
                    }
                }
            }
            return untaggedPosts;
        }

        private string SearchForTitle(string tagtitle) {
            //Load the xml database and deserialize it
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(animetitles));
            System.IO.StreamReader file = new System.IO.StreamReader("anime-titles.xml");
            animetitles AniDB = new animetitles();
            AniDB = (animetitles)reader.Deserialize(file);
            animetitlesAnime anime = null;
            //Query the database for tagtitle
            try {
                anime = AniDB.anime.SingleOrDefault(a => a.title.Any(b => b.Value.Contains(tagtitle)));
            } catch (InvalidOperationException) {
                //If it throws an exception it means there were more than one result, and the tag is likely not the title
                return null;
            }

            if (anime != null) {
                //TODO: decide which title is best, and return it
                List<string> potentialTitles = (from t in anime.title
                                                where (t.lang == "x-jat" || t.lang == "en") && (t.type == "main" || t.type == "official")
                                                select t.Value.ToString()).ToList();
                List<string> ordered = potentialTitles.OrderBy(x => x.Length).ToList();
                return ordered.First();
            }

            retVal += "\nCould not find anime title for: " + tagtitle;
            return null;
        }

        private void CheckUpdateDB() {
            //Check if the XML database we have is older than a week.
            if (!IsBelowThreshold("anime-titles.xml", MaxDBAgeHours) || !System.IO.File.Exists("anime-titles.xml")) {
                Console.WriteLine("DB out of date. Downloading latest version...");

                //Download latest file
                WebClient Client = new WebClient();
                Client.DownloadFile(AniDBDumpUrl, "anime-titles.xml.gz");

                //Decompress the downloaded file. Delete the archive when done
                DeCompressFile("anime-titles.xml.gz", "anime-titles.xml");
                System.IO.File.Delete("anime-titles.xml.gz");
            }
        }

        public bool IsBelowThreshold(string filename, int hours) {
            var threshold = DateTime.Now.AddHours(-hours);
            return System.IO.File.GetCreationTime(filename).AddHours(-hours) <= threshold;
        }

        public static void DeCompressFile(string CompressedFile, string DeCompressedFile) {
            byte[] buffer = new byte[1024 * 1024];

            using (System.IO.FileStream fstrmCompressedFile = System.IO.File.OpenRead(CompressedFile)) // fi.OpenRead())
    {
                using (System.IO.FileStream fstrmDecompressedFile = System.IO.File.Create(DeCompressedFile)) {
                    using (System.IO.Compression.GZipStream strmUncompress = new System.IO.Compression.GZipStream(fstrmCompressedFile,
                            System.IO.Compression.CompressionMode.Decompress)) {
                        int numRead = strmUncompress.Read(buffer, 0, buffer.Length);

                        while (numRead != 0) {
                            fstrmDecompressedFile.Write(buffer, 0, numRead);
                            fstrmDecompressedFile.Flush();
                            numRead = strmUncompress.Read(buffer, 0, buffer.Length);
                        } // Whend

                        //int numRead = 0;

                        //while ((numRead = strmUncompress.Read(buffer, 0, buffer.Length)) != 0)
                        //{
                        //    fstrmDecompressedFile.Write(buffer, 0, numRead);
                        //    fstrmDecompressedFile.Flush();
                        //} // Whend

                        strmUncompress.Close();
                    } // End Using System.IO.Compression.GZipStream strmUncompress 

                    fstrmDecompressedFile.Flush();
                    fstrmDecompressedFile.Close();
                } // End Using System.IO.FileStream fstrmCompressedFile 

                fstrmCompressedFile.Close();
            } // End Using System.IO.FileStream fstrmCompressedFile 

        } // End
    }
}
