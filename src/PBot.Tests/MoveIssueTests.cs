namespace PBot.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Issues;

    [TestFixture]
    public class MoveIssueTests : BotCommandFixture<MoveIssue>
    {
        [Test]
        [Explicit]
        public Task CanMoveIssue()
        {
            return Execute("move issue", "PBot.TestRepo#20", "PBot.TestRepo");
        }
    }
}