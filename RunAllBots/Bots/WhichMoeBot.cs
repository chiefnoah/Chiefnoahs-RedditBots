using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;

namespace RedditBots {
    public class WhichMoeBot : AbstractBot {

        public const string VERSION = "1.0";
        private const string CONFIG_FILENAME = "WhichMoeConfig.xml";
        private const int NUM_COMMENTS = 30;

        AniDBHandler anidbHandler;
        DanbooruHandler danbooruHandler;
        IQDBHandler iqdbHandler;
        MyAnimeListHandler myanimelistHandler;
        PixivHandler pixivHandler;
        SauceNAOHandler saucenaoHandler;

        DatabaseHandler databaseHandler;

        WhichMoeConfig config;

        /* TODO list:
           * Copy AniDB database handling code over from AutoTagBot
           * Query IQDB and SauceNAO for -booru and Pixiv IDs respectively
           * Query -booru/Pixiv for tags/info
           * Adapt XML settings loader from KanMususBot
           * Interface with -booru APIs instead of just using the IQDB tags */

        public WhichMoeBot() {
            databaseHandler = new DatabaseHandler();
            anidbHandler = new AniDBHandler(databaseHandler);
            danbooruHandler = new DanbooruHandler();
            iqdbHandler = new IQDBHandler();
            myanimelistHandler = new MyAnimeListHandler();
            pixivHandler = new PixivHandler();
            saucenaoHandler = new SauceNAOHandler();

            config = LoadConfig<WhichMoeConfig>(CONFIG_FILENAME);
        }

        public override string Run() {

            //Forward inbox messages to the admin
            ForwardInbox(config.id, config.Account.username, config.Account.password);

            /* TODO List:
               * Get 50 or so latest comments to all configured subs
               * Check DB if comment has already been checked
               * Loop through all comments checking for "Which moe?" query (or whatever I decide to make it)
               * Gracefully failout if for some reason it was unable to parse the image
               * Query IQDB/SauceNAO for IDs
               * Query using IDs to get post info.
               * Check Pixiv tags against AniDB for series titles. Return common romaji and English titles
               * Convert -booru series title format to a more readable format (Camel Case, replace "_" with " ", etc.)
               * Format a comment using previously obtained info
               * Post comment
               * Save checked comment to database */

            //Log into Reddit user
            user = reddit.LogIn(config.Account.username, config.Account.password);

            //Loop through the configured subreddits
            foreach (var s in config.subreddits) {
                //Make sure the sub is enabled
                if (s.enabled) {
                    RedditSharp.Things.Subreddit subreddit = reddit.GetSubreddit(s.Value);
                    List<RedditSharp.Things.Comment> allComments = new List<RedditSharp.Things.Comment>();
                    List<RedditSharp.Things.Comment> summonedComments = new List<RedditSharp.Things.Comment>();
                    //Get the last NUM_COMMENTS number of comments from the sub
                    allComments.AddRange(subreddit.Comments.Take(NUM_COMMENTS));
                    //Loop through those comments. Add those that haven't been checked to the uncheckedComments list
                    foreach (var c in allComments) {
                        if (!CheckIfCommentChecked(c, config.id) && CheckForSummon(c.Body)) {
                            summonedComments.Add(c);
                        }
                    }

                    //Set allComments to null, we don't need it anymore
                    allComments = null;
                    foreach (var c in summonedComments) {
                        //Get the post the comment is from
                        RedditSharp.Things.Post p = reddit.GetPost(new Uri(c.Shortlink));

                        //Try to get the Pixiv ID from the comments
                        int pixivId = GetPixivIdFromComments(p);
                        //Check if the ID is above -1 (failure)
                        if (pixivId < 0) {
                            //Use SauceNAO to try to get a PixivID
                            pixivId = saucenaoHandler.GetPixivIdFromUrl(p.Url.AbsoluteUri);
                        }
                        PixivWorksResponse.Response pixivWork = null;
                        //Check if we have a valid Pixiv ID
                        if (pixivId > 0) {
                            //Get a Pixiv Work response
                            pixivWork = pixivHandler.GetPixivWork(pixivId);
                        }

                        bool comesFromPixivAlbum = false;
                        if(pixivWork != null) {
                            comesFromPixivAlbum = pixivWork.page_count > 1;
                        }
                        //Checks if any of the tags say "original" (including in japanese)
                        //Defaults to false if it's an album, it's impossible to tell anything about the tags if it's an album :(
                        //In this case, I consider danbooru tags to be more reliable so we'll check again after we load those
                        bool original = (!comesFromPixivAlbum && pixivWork.tags.Any(t => (t == "オリジナル" || t == "original")));

                        //Try to get a danbooru ID from IQDB
                        int danbooruId = iqdbHandler.GetDanbooruId(p.Url.AbsoluteUri);
                        DanbooruPost danbooruPost = null;
                        //Check to make sure we actually got an ID
                        if (danbooruId > 0) {
                            danbooruPost = danbooruHandler.getPost(danbooruId);
                        }

                        List<string> characters = new List<string>();
                        List<string> copyrights = new List<string>();
                        List<string> searchFor = new List<string>(); //Things to search for in AniDb
                        string[] characterString = { };
                        string[] artistString = { };
                        //If we actually have a danbooru post reference...
                        if (danbooruPost != null) {
                            //split the character string object at space and loop through the result with a DanbooruHandler.Parse to format them properly
                            if (danbooruPost.tag_string_character.Length > 0) {
                                characterString = danbooruPost.tag_string_character.Split(new char[0]);
                                for (int i = 0; i < characterString.Length; i++) {
                                    characterString[i] = DanbooruHandler.ParseName(characterString[i]);
                                }
                            }
                            if (danbooruPost.tag_string_artist.Length > 0) {
                                artistString = danbooruPost.tag_string_artist.Split(new char[0]);
                                for (int i = 0; i < artistString.Length; i++) {
                                    artistString[i] = DanbooruHandler.ParseName(artistString[i]);
                                }
                            }
                            original = (danbooruPost.tag_string_copyright == "original");
                            //If we don't have any titles from Pixiv/AniDB and it the work isn't original, parse the copyrights from Danbooru
                            if (!original && danbooruPost.tag_string_copyright.Length > 0) {
                                foreach (string tag in danbooruPost.tag_string_copyright.Split(new char[0])) {
                                    copyrights.Add(DanbooruHandler.ParseName(tag));
                                    searchFor.Add(DanbooruHandler.ParseName(tag));
                                }
                            }
                        }

                        List<string> artists = new List<string>();
                        //Searches AniDB for a title
                        List<animetitlesAnime> anidbAnimeTitles = new List<animetitlesAnime>();
                        if (pixivWork != null) {
                            if (!comesFromPixivAlbum) {
                                searchFor.AddRange(pixivWork.tags.ToList());
                            }
                            anidbAnimeTitles.AddRange(anidbHandler.SearchTitleList(searchFor));
                            artists.Add(pixivWork.user.name);
                        }
                        //Loop through all characters and Parse them then add them to characters list
                        foreach (string cs in characterString) {
                            characters.Add(DanbooruHandler.ParseName(cs));
                        }
                        //Checks if we actually have any info about the post
                        bool haveInfo = (characters.Count > 0 || anidbAnimeTitles.Count > 0 || original || artists.Count > 0);
                        //We've got all the info we need to reply
                        string reply = "**You have summoned the mighty /u/WhichMoeBot!** v" + VERSION + "\r\n\r\n";
                        if (!haveInfo) {
                            reply += "Unfortunately, I was unable to find any info about this post. If I *should* have been able to something, /u/chiefnoah will be on it to figure out why!";
                        } else {
                            reply += "Here is some info I was able to find on this [image](" + p.Url + "):^^WARNING! ^^Some ^^content ^^may ^^be ^^NSFW\r\n\r\n";


                            if (pixivWork != null) {
                                reply += "* Artist's Pixiv - [" + pixivWork.user.name + "](" + PixivHandler.PIXIV_USER_URL + pixivWork.user.id + ")\r\n";
                            }

                            if (danbooruPost != null) {
                                reply += "* Artist Name(s) (other) - " + String.Join(", ", artistString) + "\r\n";
                            }

                            if (characters.Count > 0) {
                                reply += "* Characters - " + String.Join(", ", characters) + "\r\n";
                            }

                            if (original) {
                                reply += "* The work appears to be original\r\n\r\n";
                            } else {
                                if (anidbAnimeTitles.Count > 0) {
                                    reply += "\r\nHere is some info about the anime, manga, game(s), etc. the image's characters come from:\r\n\r\n";
                                    foreach(var title in anidbAnimeTitles) {
                                        animetitlesAnimeTitle japaneseOfficial = title.title.First(t => (t.lang == "ja" && t.type == "official"));
                                        animetitlesAnimeTitle main = title.title.First(t => t.type == "main");
                                        reply += "Main title: **" + main.Value + "**\r\n\r\nOfficial Japanese title: " + japaneseOfficial.Value + "\r\n\r\n";
                                    }

                                } else if(copyrights.Count > 0) {
                                    reply += "* Source(s) - " + String.Join(", ", copyrights) + "\r\n\r\n";
                                }
                            }
                        }
                        reply += "^^I ^^am ^^a ^^bot. ^^Please ^^contact ^^/u/chiefnoah ^^for ^^any ^^issues, ^^questions, ^^or ^^suggestions. ^^Image ^^info ^^not ^^guaranteed ^^to ^^be ^^accurate.\r\n\r\n^^Sources: ";
                        //TODO: move this stuff up for when I check for those objects the first time
                        List<string> sources = new List<String>();
                        if (anidbAnimeTitles.Count > 0) {
                            sources.Add("^^[AniDb](http://anidb.net/)");
                        }
                        if(pixivWork != null) {
                            sources.Add("^^[Pixiv](http://www.pixiv.net/member_illust.php?mode=medium&illust_id=" + pixivWork.id + ")");
                        }
                        if(danbooruPost != null) {
                            sources.Add("^^[Danbooru](https://danbooru.donmai.us/posts/" + danbooruPost.id + ")");
                        }
                        reply += String.Join(", ", sources);
                        retVal += "WhichMoeBot commented on " + p.Url;
                        Console.Write(reply);

                        //Save Comment, send comment to reddit here
                        c.Reply(reply);
                        SaveComment(config.id, config.Account.username, c.Id);

                    }
                }
            }


            return retVal;
        }

        public bool CheckForSummon(string comment) {
            //
            bool summon = config.summons.Any(s => (comment.ToLower().Contains(s.Value.ToLower()) && s.enabled));
            //return true; //For testing. CHANGE THIS
            return summon;
        }




    }
}
