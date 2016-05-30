namespace PBot.Tests
{
    using NUnit.Framework;
    using Octokit;
    using SyncOMatic;

    [TestFixture]
    public class SynchronizeRepoTest : BotCommandFixture<SynchronizeRepo>
    {
        [Test,Explicit]
        public async System.Threading.Tasks.Task SyncTestRepo()
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