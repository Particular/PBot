namespace IssueButler.Tests
{
    using System.Linq;
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

            Execute("add", "nservicebus");

            Assert.NotNull(repos.Single(r=>r.Name == repoName));
        }

            

    }
}