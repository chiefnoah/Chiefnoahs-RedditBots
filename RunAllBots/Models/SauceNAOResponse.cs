using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBots {
    class SauceNAOResponse {


            public Header header { get; set; }
            public Result[] results { get; set; }

        public class Header {
            public string user_id { get; set; }
            public string account_type { get; set; }
            public string short_limit { get; set; }
            public string long_limit { get; set; }
            public int long_remaining { get; set; }
            public int short_remaining { get; set; }
            public int status { get; set; }
            public string results_requested { get; set; }
            public Index index { get; set; }
            public string search_depth { get; set; }
            public float minimum_similarity { get; set; }
            public string query_image_display { get; set; }
            public string query_image { get; set; }
            public string results_returned { get; set; }
        }

        public class Index {
            public _0 _0 { get; set; }
            public _1 _1 { get; set; }
            public _2 _2 { get; set; }
            public _3 _3 { get; set; }
            public _4 _4 { get; set; }
            public _5 _5 { get; set; }
            public _51 _51 { get; set; }
            public _6 _6 { get; set; }
            public _7 _7 { get; set; }
            public _8 _8 { get; set; }
            public _9 _9 { get; set; }
            public _10 _10 { get; set; }
            public _11 _11 { get; set; }
            public _12 _12 { get; set; }
            public _13 _13 { get; set; }
            public _14 _14 { get; set; }
            public _15 _15 { get; set; }
        }

        public class _0 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _1 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _2 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _3 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _4 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _5 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _51 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _6 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _7 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _8 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _9 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _10 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _11 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _12 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _13 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _14 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class _15 {
            public int status { get; set; }
            public int parent_id { get; set; }
            public int id { get; set; }
            public int results { get; set; }
        }

        public class Result {
            public Header1 header { get; set; }
            public Data data { get; set; }
        }

        public class Header1 {
            public string similarity { get; set; }
            public string thumbnail { get; set; }
            public int index_id { get; set; }
            public string index_name { get; set; }
        }

        public class Data {
            public string title { get; set; }
            public string pixiv_id { get; set; }
            public string member_name { get; set; }
            public string member_id { get; set; }
            public string[] creator { get; set; }
            public string source { get; set; }
            public string danbooru_id { get; set; }
            public string pool_name { get; set; }
            public int pool_id { get; set; }
            public string drawr_id { get; set; }
        }


    }
}
