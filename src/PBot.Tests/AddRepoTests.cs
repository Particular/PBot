namespace PBot.Tests
{
    using System;
    using System.Linq;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;
    using PBot.Repositories;

    [TestFixture]
    public class AddRepoTests : BotCommandFixture<AddRepository>
    {
        [Test]
        public void AddNewValidRepo()
        {
         
            var repoName = "nservicebus";
            
            brain.Set(new AvailableRepositories());

            Execute("add", repoName);

            Assert.NotNull(brain.Get<AvailableRepositories>().Single(r => r.Name == repoName));
        }
      
        [Test]
        public void AddByWildcard()
        {
            var repoName = "nservicebus*";

            brain.Set(new AvailableRepositories());

            Execute("add", repoName);

            Console.Out.WriteLine(string.Join(";", brain.Get<AvailableRepositories>()));
            Assert.True(brain.Get<AvailableRepositories>().Count() > 2);
        }

        [Test]
        public void AddInvalidRepo()
        {

            var repoName = "NonExistingRepo";
            var repos = new AvailableRepositories();

            brain.Set(repos);

            Execute("add", repoName);


            Assert.False(repos.Any());
            Assert.NotNull(Messages.SingleOrDefault(m => m.Contains("doesn't exist")));
        }

        [Test]
        public void AddDuplicateValidRepo()
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

            Execute("add", repoName);

            Assert.NotNull(repos.Single(r => r.Name == repoName));
            Assert.NotNull(Messages.Single(m=>m.Contains("already exists")));
        }

            

    }
}