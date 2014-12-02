namespace PBot.Tests
{
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;
    using PBot.Issues;

    [TestFixture]
    public class ListIssueStatsTest : BotCommandFixture<ListIssueStats>
    {
        [Test]
        public void ShowStats()
        {
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = "NServiceBus.RabbitMq",
                    Caretaker = "andreas"
                },

                new AvailableRepositories.Repository
                {
                    Name = "NServiceBus.SqlServer",
                    Caretaker = "john"
                }
            };

            brain.Set(repos);
            Execute("list issue stats");
        }
    }
}