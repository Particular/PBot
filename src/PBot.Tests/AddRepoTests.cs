namespace PBot.Tests
{
    using System;
    using System.Linq;
    using IssueButler.Mmbot.Caretakers;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;

    [TestFixture]
    public class AddRepoTests : BotCommandFixture<AddRepository>
    {
        [Test]
        public async void AddNewValidRepo()
        {
         
            var repoName = "nservicebus";
            
            brain.Set(new AvailableRepositories());

            await Execute("add", repoName);

            Assert.NotNull(brain.Get<AvailableRepositories>().Single(r => r.Name == repoName));
        }
      
        [Test]
        public async void AddByWildcard()
        {
            var repoName = "nservicebus*";

            brain.Set(new AvailableRepositories());

            await Execute("add", repoName);

            Console.Out.WriteLine(string.Join(";", brain.Get<AvailableRepositories>()));
            Assert.True(brain.Get<AvailableRepositories>().Count() > 2);
        }

        [Test]
        public async void AddInvalidRepo()
        {

            var repoName = "NonExistingRepo";
            var repos = new AvailableRepositories();

            brain.Set(repos);

            await Execute("add", repoName);


            Assert.False(repos.Any());
            Assert.NotNull(Messages.SingleOrDefault(m => m.Contains("doesn't exist")));
        }

        [Test]
        public async void AddDuplicateValidRepo()
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
            Assert.NotNull(Messages.Single(m=>m.Contains("already exists")));
        }
    }
}