using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditBots;
using RedditSharp.Things;

namespace RedditBots {
    public class BotKanMusus : AbstractBot {

        DanbooruHandler danbooruHandler;
        PixivHandler pixivHandler;
        SauceNAOHandler saucenaoHandler;
        IQDBHandler iqdbHandler;

        public BotKanMusus() {
            danbooruHandler = new DanbooruHandler();
            pixivHandler = new PixivHandler();
            saucenaoHandler = new SauceNAOHandler();
            iqdbHandler = new IQDBHandler();
        }

        public override string Run() {
            List<KanMususBot> bots = GetAllBots();
            retVal += "\r\nStarted: " + DateTime.Now;
            retVal += "\r\nNumber of KanMusu bots running: " + bots.Count;
            RunAllBots(bots);
            retVal += "\r\nFinished: " + DateTime.Now + "\r\n\r\n";
            Console.WriteLine("\r\nDone!");
            //Console.Read ();
            return retVal;
        }

        /// <summary>
        /// Loops through all the bots
        /// </summary>
        /// <param name="bots"></param>
        public void RunAllBots(List<KanMususBot> bots) {
            List<RedditSharp.Things.Post> toCommentOn = new List<RedditSharp.Things.Post>();
            List<string> subreddits = new List<string>();
            int newPostsCount = 0;

            foreach (KanMususBot bot in bots) {
                user = reddit.LogIn(bot.username, bot.password);
                ForwardInbox(bot.id, bot.username, bot.password);
                foreach (KanMususBotSubreddit subr in bot.Subreddits) {
                    subreddits.Add(subr.Value);
                }
            }
            subreddits = subreddits.Distinct().ToList();
            foreach (Post post in GetRecentPosts(subreddits)) {
                if (CheckIfPostSaved(post.Id)) {
                    continue;
                }

                newPostsCount++;
                //Console.WriteLine("Scanning a new post...");
                List<string> tags = TryToGetTags(post);


                //Compares tags in all posts to the tags in all bots
                foreach (KanMususBot bot in bots) {
                    Boolean comment = false;

                    comment = bot.Tags.Any(bt => tags.Any(t => bt == t));
                    if (post.LinkFlairText == null) {
                        post.LinkFlairText = "";
                    }
                    if (!comment) {
                        comment = bot.Subreddits.Any(s => bot.Tags.Any(t => s.SearchTitleFlair && (post.Title.Contains(t) || post.LinkFlairText.Contains(t))));
                    }


                    //Comment on post if it should be commented on
                    if (comment) {
                        user = reddit.LogIn(bot.username, bot.password);
                        string message = "Bot " + bot.username + " commented on [" + post.Title + "](" + post.Shortlink + ")";
                        CommentOnPost(bot, post);
                        SendMessageToAdmin(bot.id, bot.username, bot.password, BOT_ADMIN, message);
                        retVal += "\r\nBot " + bot.username + " commented on " + post.Title + " - " + post.Shortlink;
                    }
                    SavePost(bot.id, bot.username, post.Id);
                }
            }
            retVal += "\r\nScanned " + newPostsCount + "\r\n";
        }

        public List<string> TryToGetTags(Post post) {
            if (post != null) {
                List<string> tags = new List<string>();
                //Try to get the Pixiv ID from the "source" comment on the post
                int pixivId = GetPixivIdFromComments(post);
                //If unable to get Pixiv ID from comment, use SauceNAO
                if (pixivId < 1) {
                    pixivId = saucenaoHandler.GetPixivIdFromUrl(post.Url.ToString());
                }

                //If we have a Pixiv ID at this point, we can query the tags for them.
                if (pixivId > 1) {
                    PixivWorksResponse.Response pixivWork = pixivHandler.GetPixivWork(pixivId);
                    if (pixivWork != null) {
                        tags = pixivWork.tags.ToList();
                        return tags;
                    }
                }
                //If we have no tags at this point, try using danbooru
                if (tags.Count < 1) {
                    int danbooruId = iqdbHandler.GetDanbooruId(post.Url.ToString());
                    tags = ((danbooruHandler.getPost(danbooruId).tag_string_character).Split(new char[0])).ToList();
                    if (tags != null) {
                        if (tags.Count > 0) {
                            return tags;
                        }
                    }
                }
            }
            //If we can't get tags, return an empty list
            return new List<string>();
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

        //TODO: Move this to AbstractBot and add overloads
        /// <summary>
        /// Returns the 10 latest posts from all subreddits passed as an argument
        /// </summary>
        /// <param name="subs">string array of all the subreddits to get posts from</param>
        /// <returns></returns>
        public List<Post> GetRecentPosts(List<string> subs) {
            List<Post> allPosts = new List<Post>();

            //Load access to all subreddits enabled for the bot
            List<RedditSharp.Things.Subreddit> subreddits = new List<RedditSharp.Things.Subreddit>();
            for (int i = 0; i < subs.Count(); i++) {
                subreddits.Add(reddit.GetSubreddit(subs[i]));
            }

            //The way I do it is super ineficient (I could condense it to one loop) but this could help prevent errors... mabye?
            //Either way, I'm too lazy to rewrite it
            foreach (var subreddit in subreddits) {
                allPosts.AddRange(subreddit.New.Take(10));
            }
            //retVal += "\r\nFound " + allPosts.Count + " new posts to scan";
            return allPosts;
        }

        /// <summary>
        /// Loads the bots from XML
        /// </summary>
        /// <returns></returns>
        public List<KanMususBot> GetAllBots() {
            try {
                KanMusus kanmusus = LoadConfig<KanMusus>("KanMususData.xml");
                List<KanMususBot> allBots = kanmusus.Bot.ToList<KanMususBot>();
                List<KanMususBot> enabledBots =
                    (from bot in allBots
                     where bot.enabled == true
                     select bot).ToList<KanMususBot>();
                return enabledBots;
            } catch (Exception e) {
                retVal += "\r\nUnable to load bot settings from config";
                Console.Write(e.Message);
                return null;
            }
        }
    }
}
