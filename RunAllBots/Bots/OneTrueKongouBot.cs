using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;

namespace RedditBots
{
    class OneTrueKongouBot : AbstractBot
    {
        private const int BOT_ID = 8;
        private const string SUBREDDIT = "OneTrueKongou";

        //Why can't an array be a constant?
        private string[] COMMENTS = { "[Burning love!](https://www.youtube.com/watch?v=YnheOlgK5uo)",
            "[Kongou-desu](https://www.youtube.com/watch?v=YnheOlgK5uo)"};


        public override string Run()
        {
            string output = "";
            KanMusus config = LoadConfig<KanMusus>("KanMususData.xml");
            KanMususBot kongouBot = config.Bot.First(b => b.username == "KongouBot");
            //Log into reddit.
            reddit.LogIn(kongouBot.username, kongouBot.password); //This really really really needs to not be hard coded
            //I'm too lazy to re-implement a loader for loading the values from XML... again

            //One liner to get the recent 5 posts from /r/OneTrueKongou
            List<RedditSharp.Things.Post> posts = reddit.GetSubreddit(SUBREDDIT).New.Take(5).ToList();


            //Loop through all the posts. Check if they've already been commented on, comment on them if they haven't
            foreach (var post in posts)
            {
                if (!CheckIfPostSaved(BOT_ID, post.Id)) {
                    output += "\r\nOneTrueKongouBot is commenting on " + post.Title + " - " + post.Shortlink;
                    //Comment on the post!
                    Random rnd = new Random();
                    int randomInt = rnd.Next(0, COMMENTS.Length);
                    post.Comment(COMMENTS[randomInt]);
                    SavePost(BOT_ID, "KongouBot", post.Id);
                }

            }

            return output;
        }
    }
}
