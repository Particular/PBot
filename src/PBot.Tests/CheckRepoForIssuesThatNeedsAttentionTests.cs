namespace PBot.Tests
{
    using NUnit.Framework;
    using PBot.Issues;

    [TestFixture]
    public class CheckRepoForIssuesThatNeedsAttentionTests : BotCommandFixture<CheckRepoForIssuesThatNeedsAttention>
    {
        [Test]
        public async void CheckIssuesInNServiceBus()
        {
            await Execute("check repo", "NServiceBus");
        }

        [Test]
        public async void CheckIssuesInServiceControl()
        {
            await Execute("check repo", "ServiceControl");
        }
    }
}
