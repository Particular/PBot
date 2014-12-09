namespace PBot.Tests
{
    using NUnit.Framework;
    using PBot.Reminders;

    [TestFixture]
    public class RemindProductRoadmapTeamTests : BotCommandFixture<RemindProductRoadmapTeam>
    {
        [Test]
        public void IntegrationTest()
        {
            Execute("remind product roadmap team");
        }

    }
}