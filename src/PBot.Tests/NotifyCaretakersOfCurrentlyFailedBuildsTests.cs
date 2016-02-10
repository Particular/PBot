namespace PBot.Tests
{
    using System.Linq;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;
    using PBot.Buildserver;

    class NotifyCaretakersOfCurrentlyFailedBuildsTests : BotCommandFixture<NotifyCaretakersOfCurrentlyFailedBuilds>
    {
        [Test, Ignore("Guest access to build server disabled for security reasons")]
        public async void RepoWithExistingBuild()
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

            await Execute("notify caretakers of failed builds");
        }

        [Test, Ignore("Until we have a better way excluding repos that doesn't need a build")]
        public async void RepoNoBuild()
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

            await Execute("notify caretakers of failed builds");

            Assert.True(Messages.First().Contains("help create"));
        }
    }
}