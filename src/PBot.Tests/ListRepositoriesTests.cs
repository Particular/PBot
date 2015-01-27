namespace PBot.Tests
{
    using System.Linq;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;

    [TestFixture]
    public class ListRepositoriesTests : BotCommandFixture<ListRepositories>
    {
        [Test]
        public void ShowRepos()
        {
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = "NServiceBus.SqlServer",
                    Caretaker = "john"
                },

                new AvailableRepositories.Repository
                {
                    Name = "NServiceBus.RabbitMq",
                    Caretaker = "andreas"
                }
            };

            brain.Set(repos);
            Execute("list repos");

            Assert.True(Messages.Contains("<https://github.com/Particular/NServiceBus.RabbitMq|NServiceBus.RabbitMq>"));
            Assert.True(Messages.Contains("<https://github.com/Particular/NServiceBus.SqlServer|NServiceBus.SqlServer>"));
        }
    }
}