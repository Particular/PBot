namespace PBot.Tests
{
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;
    using Issues;

    [TestFixture]
    public class ListIssueStatsTest : BotCommandFixture<ListIssueStats>
    {
        [Test]
        public Task ShowStats()
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
            return Execute("list issue stats");
        }
    }
}