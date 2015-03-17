namespace PBot.Tests
{
    using System.Linq;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;
    using PBot.Buildserver;

    class NotifyCaretakersOfCurrentlyFailedBuildsTests : BotCommandFixture<NotifyCaretakersOfCurrentlyFailedBuilds>
    {
        [Test]
        public void RepoWithExistingBuild()
        {
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = "ServiceControl",
                    Caretaker = "johndoe"
                },
                 new AvailableRepositories.Repository
                {
                    Name = "ServiceInsight",
                    Caretaker = "johndoe"
                }

                
            };

            brain.Set(repos);

            Execute("notify caretakers of failed builds");
        }

        [Test]
        public void RepoNoBuild()
        {
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = "NoBuild",
                    Caretaker = "johndoe"
                },
            
            };

            brain.Set(repos);

            Execute("notify caretakers of failed builds");

            Assert.True(Messages.First().Contains("help create"));
        }
    }
}