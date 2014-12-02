namespace PBot.Tests
{
    using NUnit.Framework;
    using PBot.SyncOMatic;

    [TestFixture]
    public class SyncronizeRepoTest : BotCommandFixture<SyncronizeRepo>
    {
        [Test,Explicit]
        public void SyncTestRepo()
        {
            Execute("sync", "IssueButler.TestRepo", "target branch", "master");
        }
    }
}