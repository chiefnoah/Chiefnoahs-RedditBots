using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;

namespace RedditBots
{
    class WhichMoeBot : AbstractBot
    {

        public const string VERSION = "1.0.0";
        private const string CONFIG_FILENAME = "WhichMoeConfig.xml";

        AniDBHandler anidbHander;
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
            anidbHander = new AniDBHandler(databaseHandler);
            danbooruHandler = new DanbooruHandler();
            iqdbHandler = new IQDBHandler();
            myanimelistHandler = new MyAnimeListHandler();
            pixivHandler = new PixivHandler();
            saucenaoHandler = new SauceNAOHandler();

            config = LoadConfig<WhichMoeConfig>(CONFIG_FILENAME);
        }

        public override string Run()
        {
            user = reddit.LogIn(config.Account.username, config.Account.password);
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

            
               
            return retVal;
        }






    }
}
