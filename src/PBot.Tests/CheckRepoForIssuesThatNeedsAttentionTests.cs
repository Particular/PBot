namespace PBot.Tests
{
    using NUnit.Framework;
    using PBot.Issues;

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
