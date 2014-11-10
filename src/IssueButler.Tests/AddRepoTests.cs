namespace IssueButler.Tests
{
    using System.Linq;

    using IssueButler.Mmbot;
    using IssueButler.Mmbot.Caretakers;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;

    [TestFixture]
    public class AddRepoTests : BotCommandFixture<AddRepository>
    {
        [Test]
        public void AddNewValidRepo()
        {
         
            var repoName = "nservicebus";
            var repos = new AvailableRepositories();

            brain.Set(typeof(AvailableRepositories).FullName, repos);

            Execute("add", repoName);

            Assert.NotNull(brain.Get<AvailableRepositories>().Single(r => r.Name == repoName));
        }

        [Test]
        public void AddInvalidRepo()
        {

            var repoName = "NonExistingRepo";
            var repos = new AvailableRepositories();

            brain.Set(typeof(AvailableRepositories).FullName, repos);

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

            brain.Set(typeof(AvailableRepositories).FullName, repos);

            Execute("add", repoName);

            Assert.NotNull(repos.Single(r => r.Name == repoName));
            Assert.NotNull(Messages.Single(m=>m.Contains("already exists")));
        }

            

    }
}