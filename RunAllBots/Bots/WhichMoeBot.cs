using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBots
{
    class WhichMoeBot : AbstractBot
    {

        /* TODO list:
           * Copy AniDB database handling code over from AutoTagBot
           * Query IQDB and SauceNAO for -booru and Pixiv IDs respectively
           * Query -booru/Pixiv for tags/info
           * Adapt XML settings loader from KanMususBot
           * Interface with -booru APIs instead of just using the IQDB tags






        */

        public override string Run()
        {
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
               * Save checked comment to database
            */


        

            return retVal;
        }

    }
}
