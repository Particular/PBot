namespace IssueButler.Tests
{
    using IssueButler.Mmbot.SyncOMatic;
    using NUnit.Framework;

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