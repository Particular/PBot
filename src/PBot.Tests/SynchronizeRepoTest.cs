namespace PBot.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Octokit;
    using SyncOMatic;

    [TestFixture]
    public class SynchronizeRepoTest : BotCommandFixture<SynchronizeRepo>
    {
        [Test,Explicit]
        public Task SyncTestRepo()
        {
            return Execute("sync", "PBot.testrepo", "target branch", "master");
        }

        [Test]
        public void BadRepoName()
        {
            Assert.Throws<NotFoundException>(async () => await Execute("sync", "PBot.dddddd", "target branch", "master"));
        }
    }
}