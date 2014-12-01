namespace IssueButler.Tests
{
    using System.Linq;

    using IssueButler.Mmbot;
    using IssueButler.Mmbot.Caretakers;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;

    [TestFixture]
    public class RemoveRepositoryTests : BotCommandFixture<RemoveRepository>
    {
        [Test]
        public void RemoveValidRepo()
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


            Execute("remove", repoName);

            Assert.Null(brain.Get<AvailableRepositories>().SingleOrDefault(r => r.Name == repoName));
        }
      
     

        [Test]
        public void RemoveInvalidRepo()
        {

            var repoName = "NonExistingRepo";
            var repos = new AvailableRepositories();

            brain.Set(repos);

            Execute("remove", repoName);


            Assert.False(repos.Any());
            Assert.NotNull(Messages.SingleOrDefault(m => m.Contains("doesn't exist")));
        }

        [Test]
        public void RemoveValidButNonExistingRepo()
        {

            var repoName = "nservicebus";
            var repos = new AvailableRepositories();

            brain.Set(repos);

            Execute("remove", repoName);

            Assert.NotNull(Messages.Single(m => m.Contains("doesn't exist")));
        }

            

    }
}