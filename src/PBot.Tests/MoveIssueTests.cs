namespace PBot.Tests
{
    using NUnit.Framework;
    using PBot.Issues;

    [TestFixture]
    public class MoveIssueTests : BotCommandFixture<MoveIssue>
    {
        [Test]
        [Explicit]
        public void CanMoveIssue()
        {
            Execute("move issue", "PBot.TestRepo#20", "PBot.TestRepo");
        }
    }
}