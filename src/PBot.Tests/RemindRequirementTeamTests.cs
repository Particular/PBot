namespace PBot.Tests
{
    using NUnit.Framework;
    using PBot.Reminders;

    [TestFixture]
    public class RemindRequirementTeamTests : BotCommandFixture<RemindRequirementsTeam>
    {
        [Test]
        public void IntegrationTest()
        {
            Execute("remind requirement team");
        }

    }
}