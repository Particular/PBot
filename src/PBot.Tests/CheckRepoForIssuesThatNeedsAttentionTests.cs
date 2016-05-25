namespace PBot.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Issues;

    [TestFixture]
    public class CheckRepoForIssuesThatNeedsAttentionTests : BotCommandFixture<CheckRepoForIssuesThatNeedsAttention>
    {
        [Test]
        public Task CheckIssuesInNServiceBus()
        {
            return Execute("check repo", "NServiceBus");
        }

        [Test]
        public Task CheckIssuesInServiceControl()
        {
            return Execute("check repo", "ServiceControl");
        }
    }
}
