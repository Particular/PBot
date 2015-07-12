namespace PBot.Tests
{
    using NUnit.Framework;
    using Octokit;
    using PBot.SyncOMatic;

    [TestFixture]
    public class SynchronizeRepoTest : BotCommandFixture<SynchronizeRepo>
    {
        [Test,Explicit]
        public async void SyncTestRepo()
        {
            await Execute("sync", "PBot.testrepo", "target branch", "master");
        }

        [Test]
        public void BadRepoName()
        {
            Assert.Throws<NotFoundException>(async () => await Execute("sync", "PBot.dddddd", "target branch", "master"));
        }
    }
}