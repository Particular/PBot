namespace PBot.Tests
{
    using System;
    using NUnit.Framework;
    using PBot.SyncOMatic;

    [TestFixture]
    public class SyncronizeRepoTest : BotCommandFixture<SyncronizeRepo>
    {
        [Test,Explicit]
        public void SyncTestRepo()
        {
            Execute("sync", "PBot.testrepo", "target branch", "master");
        }

        [Test]
        public void BadRepoName()
        {
            Assert.Throws<AggregateException>(() => Execute("sync", "PBot.dddddd", "target branch", "master"));
        }
    }
}