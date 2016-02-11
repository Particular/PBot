namespace PBot.Tests
{
    using NUnit.Framework;
    using PBot.Issues;

    [TestFixture]
    public class MoveIssueTests : BotCommandFixture<MoveIssue>
    {
        [Test]
        [Explicit]
        public async System.Threading.Tasks.Task CanMoveIssue()
        {
            await  Execute("move issue", "PBot.TestRepo#20", "PBot.TestRepo");
        }
    }
}