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
            Execute("convert", "PBot.TestRepo", "18", "to pull from", "failed-pull-1", "to", "master");
        }
    }
}