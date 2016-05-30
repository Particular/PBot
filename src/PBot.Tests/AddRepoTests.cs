namespace PBot.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Caretakers;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;

    [TestFixture]
    public class AddRepoTests : BotCommandFixture<AddRepository>
    {
        [Test]
        public async Task AddNewValidRepo()
        {
            var repoName = "nservicebus";

            brain.Set(new AvailableRepositories());

            await Execute("add", repoName);

            Assert.NotNull(brain.Get<AvailableRepositories>().Single(r => r.Name == repoName));
        }

        [Test]
        public async Task AddByWildcard()
        {
            var repoName = "nservicebus.rabb*";

            brain.Set(new AvailableRepositories());

            await Execute("add", repoName);

            Console.WriteLine(string.Join(";", brain.Get<AvailableRepositories>()));
            Assert.True(brain.Get<AvailableRepositories>().Any());
        }

        [Test]
        public async Task AddInvalidRepo()
        {
            var repoName = "NonExistingRepo";
            var repos = new AvailableRepositories();

            brain.Set(repos);

            await Execute("add", repoName);

            Assert.False(repos.Any());
            Assert.NotNull(Messages.SingleOrDefault(m => m.Contains("doesn't exist")));
        }

        [Test]
        public async Task AddDuplicateValidRepo()
        {
            var repoName = "nservicebus";
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = repoName
                }
            };

            brain.Set(repos);

            await Execute("add", repoName);

            Assert.NotNull(repos.Single(r => r.Name == repoName));
            Assert.NotNull(Messages.Single(m => m.Contains("already exists")));
        }
    }
}