namespace PBot.Tests
{
    using NUnit.Framework;
    using PBot.Issues;

    [TestFixture]
    public class ConvertIssueToPullRequestTests : BotCommandFixture<ConvertIssueToPullRequest>
    {
        [Test]
        [Explicit]
        public void CanConvertAnExistingIssue()
        {
            Execute("convert issue", "64", "from repository", "NServiceBus.SqlServer", "into pull request from", "semaphore-based-throttling", "to", "develop");
        }
    }
}