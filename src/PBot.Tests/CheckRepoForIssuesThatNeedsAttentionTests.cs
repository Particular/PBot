namespace PBot.Tests
{
    using NUnit.Framework;
    using PBot.Issues;

    [TestFixture]
    public class CheckRepoForIssuesThatNeedsAttentionTests : BotCommandFixture<CheckRepoForIssuesThatNeedsAttention>
    {
        [Test]
        public async System.Threading.Tasks.Task CheckIssuesInNServiceBus()
        {
            await Execute("check repo", "NServiceBus");
        }

        [Test]
        public async System.Threading.Tasks.Task CheckIssuesInServiceControl()
        {
            await Execute("check repo", "ServiceControl");
        }
    }
}
