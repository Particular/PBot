namespace IssueButler.Tests
{
    using IssueButler.Mmbot.Issues;
    using NUnit.Framework;

    [TestFixture]
    public class CheckRepoForIssuesThatNeedsAttentionTests : BotCommandFixture<CheckRepoForIssuesThatNeedsAttention>
    {
        [Test]
        public void CheckIssuesInNServiceBus()
        {
            Execute("check repo","NServiceBus");
        }

        [Test]
        public void CheckIssuesInServiceControl()
        {
            Execute("check repo", "ServiceControl");
        }
    }
}
