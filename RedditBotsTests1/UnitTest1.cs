using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedditBots;
using System.Collections.Generic;

namespace RedditBotsTests1 {
    [TestClass]
    public class AbstractBotTests {
        [TestMethod]
        public void BooruTagsTest() {
            TestBot testBot = new TestBot();
            List<string> testTags = testBot.getBooruTags("http://cdn.awwni.me/qvm2.jpg");
            Assert.IsTrue(testTags.Count > 0);
        }
    }

    [TestClass]
    class TestBot : AbstractBot {
        public override string Run() {
            throw new NotImplementedException();
        }
    }
}
