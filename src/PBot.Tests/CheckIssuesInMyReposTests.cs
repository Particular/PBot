namespace PBot.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;
    using Issues;

    [TestFixture]
    public class CheckIssuesInMyReposTests : BotCommandFixture<CheckIssuesInMyRepos>
    {
        [Test]
        public Task CheckIssuesForExistingUser()
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
            AsUser("andreas");
            return Execute("check my repos");
        }

        [Test]
        public async Task CheckIssuesForUserWithNoRepos()
        {
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = "NServiceBus.RabbitMq",
                    Caretaker = "andreas"
                }
            };

            brain.Set(repos);
            AsUser("some user");
            await Execute("check my repos");

            Assert.True(Messages.Any(m => m.Contains("couldn't find any repos")));
        }
    }
}