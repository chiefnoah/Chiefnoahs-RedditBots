using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedditBots;
using System.Collections.Generic;
using RedditSharp;

namespace RedditBotsTests1 {
    [TestClass]
    public class AbstractBotTests {
        [TestMethod]
        public void AniDBTagSearchTest() {
            RedditBots.AniDBHandler anidb = new RedditBots.AniDBHandler();
            List<RedditBots.animetitlesAnime> anime = anidb.SearchTitle("東方");
            Assert.IsTrue(anime.Count >= 3);
        }

        [TestMethod]
        public void AniDBTagListSearchTest() {
            List<string> tags = new List<string>();
            tags.Add("艦隊これくしょん");
            tags.Add("KanColle");

            AniDBHandler anidb = new AniDBHandler();
            List<animetitlesAnime> anime = anidb.SearchTitleList(tags);
            Assert.IsTrue(anime.Count > 0);
        }

        [TestMethod]
        public void KanMusuBotLoadTest() {
            RedditBots.BotKanMusus testKanMusuBot = new RedditBots.BotKanMusus();
            List<KanMususBot> testKanMusuBots = testKanMusuBot.GetAllBots();
            Assert.AreNotEqual(null, testKanMusuBot);
            Assert.IsTrue(testKanMusuBots.Count > 0);
        }

        [TestMethod]
        public void DanbooruPostTest() {
            DanbooruHandler danbooruHandler = new DanbooruHandler();
            DanbooruPost post = danbooruHandler.getPost(2124445);
            Assert.AreEqual(321535, post.uploader_id);
        }

        [TestMethod]
        public void PixivWorkTest() {
            PixivHandler pixivHandler = new PixivHandler();
            PixivWorksResponse.Response work = pixivHandler.GetPixivWork(52553684);
            Assert.IsTrue(work != null);
            Assert.IsTrue(work.title == "喧嘩");
        }

        [TestMethod]
        public void MyAnimeListAnimeSearchTest() {
            MyAnimeListHandler myAnimeListHandler = new MyAnimeListHandler();
            MyAnimeListAnime anime = myAnimeListHandler.SearchAnime("accel");
            Assert.IsTrue(anime.entry.Length > 0);
        }

        [TestMethod]
        public void MyAnimeListMangaSearchTest() {
            MyAnimeListHandler myAnimeListHandler = new MyAnimeListHandler();
            MyAnimeListManga manga = myAnimeListHandler.SearchManga("accel");
            Assert.IsTrue(manga.entry.Length > 0);
        }

        [TestMethod]
        public void SauceNAOImageUrlSearchTest() {
            SauceNAOHandler sauceNAOHandler = new SauceNAOHandler();
            int id = sauceNAOHandler.GetPixivIdFromUrl("https://cdn.awwni.me/qswe.jpg");
            Assert.AreEqual(48168636, id);
        }

        [TestMethod]
        public void IQDBImageSearchTest() {
            IQDBHandler iqdbhandler = new IQDBHandler();
            int id = iqdbhandler.GetDanbooruId("https://cdn.awwni.me/r4dn.png");
            Assert.AreEqual(1825390, id);
        }

        [TestMethod]
        public void KanMusuBotTagParseTest() {
            Reddit reddit = new Reddit();
            RedditSharp.Things.Post post = reddit.GetPost(new Uri("https://www.reddit.com/r/awwnime/comments/3l8jem/just_a_little_remi_touhou/"));
            BotKanMusus kanmus = new BotKanMusus();
            List<string> tags = kanmus.TryToGetTags(post);
            Assert.IsTrue(tags.Count > 0);
        }

        [TestMethod]
        public void BotSaveCheckPostTest() {
            TestBot bot = new TestBot();
            Assert.IsTrue(bot.SaveCheckPostTest());
        }

        [TestMethod]
        public void MiscTest() {
        }
    }

    [TestClass]
    class TestBot : AbstractBot {
        public override string Run() {
            throw new NotImplementedException();
        }
        public Boolean SaveCheckPostTest() {
            SavePost(0, "testBot", "test");
            return CheckIfPostSaved(0, "test"); 
        }
    }
}
