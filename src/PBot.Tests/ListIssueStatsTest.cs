namespace PBot.Tests
{
    using NUnit.Framework;
    using PBot.Issues;
    using PBot.Repositories;

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