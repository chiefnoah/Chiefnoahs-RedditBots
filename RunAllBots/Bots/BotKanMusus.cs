using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunAllBots;
using RedditBots;
using RedditSharp.Things;

namespace RedditBots {
    class BotKanMusus : AbstractBot {


        public override String Run() {
            List<KanMususBot> bots = GetAllBots();
            retVal += "\r\nDate: " + DateTime.Now;
            retVal += "\r\nNumber of KanMusu bots running: " + bots.Count;
            RunAllBots(bots);
			Console.WriteLine ("\r\nDone!");
			//Console.Read ();
            return retVal;
        }

        /// <summary>
        /// Loops through all the bots
        /// </summary>
        /// <param name="bots"></param>
        public void RunAllBots(List<KanMususBot> bots) {
            foreach (KanMususBot bot in bots) {
                user = reddit.LogIn(bot.username, bot.password);
                ForwardInbox(bot.id, bot.username);
                List<RedditSharp.Things.Post> posts = GetNewPosts(bot);
                List<RedditSharp.Things.Post> toCommentOn = new List<RedditSharp.Things.Post>();
                foreach (Post post in posts) {
                    string[] tags;
                    string pixivId = GetPixivIdFromComments(post);
                    tags = GetImageTagsByPixivId(pixivId);
                    if (pixivId == null || tags.Count() < 1) {
                        pixivId = GetImageSourceId(post.Url.ToString());
                        tags = GetImageTagsByPixivId(pixivId);
                    }
                    //Console.Write(tags);
                    foreach (string botTag in bot.Tags) {
                        foreach (string tag in tags) {
                            if (tag == botTag) {
                                toCommentOn.Add(post);
                            }
                        }
                        
                        //Performs a LINQ query to check if it should search the title or flair for a title.
                        //This helps avoid false positives (but will also likely decrease overall accuracy)
                        bool searchTitleFlair = (from s in bot.Subreddits
                                                where s.SearchTitleFlair && s.Value.Contains(post.Subreddit)
                                                select s).Any();
                        if (searchTitleFlair) {
                            //Quick fix for nullpo
                            if (post.LinkFlairText == null) {
                                post.LinkFlairText = "";
                            }
                            if (post.Title.IndexOf(botTag) > 0 || post.LinkFlairText.IndexOf(botTag) > 0) {
                                toCommentOn.Add(post);
                            }
                        }


                    }
                    SavePost(bot.id, bot.username, post.Id);
                }
                //This should make everything unique so we don't get posts
                toCommentOn = toCommentOn.Distinct().ToList();
                foreach (Post post in toCommentOn) {
                    retVal += "\r\nBot " + bot.username + " commented on " + toCommentOn.Count() + " posts";
                    //CommentOnPost(bot, post);
                }
            }
        }

        /// <summary>
        /// Comments on the post with a random comment taken from the bot
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="post"></param>
        public void CommentOnPost(KanMususBot bot, Post post) {

            Random rnd = new Random();
            int rInt = rnd.Next(0, bot.Replies.Length);
            string comment = bot.Replies[rInt];
            post.Comment(bot.Replies[rInt]);
        }

        /// <summary>
        /// Returns posts from the bots enabled subreddits that have not been previously scanned
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        public List<RedditSharp.Things.Post> GetNewPosts(KanMususBot bot) {
            List<Post> allPosts = new List<Post>();

            //Load access to all subreddits enabled for the bot
            List<RedditSharp.Things.Subreddit> subs = new List<RedditSharp.Things.Subreddit>();
            foreach (var sub in bot.Subreddits) {
                subs.Add(reddit.GetSubreddit(sub.Value));
            }

            //The way I do it is super ineficient (I could condense it to one loop) but this could help prevent errors... mabye?
            //Either way, I'm too lazy to rewrite it
            foreach (var subreddit in subs) {
                foreach (var post in subreddit.New.Take(10)) {
                    if (!CheckIfPostSaved(bot.id, post.Id)) {
                        allPosts.Add(post);
                    }
                }
            }
            retVal += "\r\nFound " + allPosts.Count + " new posts to scan";
            return allPosts;
        }

        /// <summary>
        /// Loads the bots from XML
        /// </summary>
        /// <returns></returns>
        public List<KanMususBot> GetAllBots() {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(KanMusus));
            String filepath = "./Bots/Data/KanMususData.xml";
            System.IO.StreamReader file = new System.IO.StreamReader(filepath);
            KanMusus kanmusus = new KanMusus();
            kanmusus = (KanMusus)reader.Deserialize(file);
            List<KanMususBot> allBots = kanmusus.Bot.ToList<KanMususBot>();
            List<KanMususBot> enabledBots =
                (from bot in allBots
                 where bot.enabled == true
                 select bot).ToList<KanMususBot>();
            return enabledBots;
        }
    }
}
