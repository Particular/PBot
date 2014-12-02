namespace PBot.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using PBot.Issues;
    using PBot.Repositories;

    [TestFixture]
    public class CheckIssuesInMyReposTests : BotCommandFixture<CheckIssuesInMyRepos>
    {
        [Test]
        public void CheckIssuesForExistingUser()
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
            AsUserName("andreas");
            Execute("check my repos");
        }

        [Test]
        public void CheckIssuesForUserWitnNoRepos()
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
            AsUserName("some user");
            Execute("check my repos");

            Assert.True(Messages.Any(m => m.Contains("couldn't find any repos")));
        }
    }
}