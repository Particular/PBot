namespace IssueButler.Tests
{
    using System.Linq;
    using IssueButler.Mmbot;
    using IssueButler.Mmbot.Caretakers;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;

    [TestFixture]
    public class RegisterCaretakerTests:BotCommandFixture<RegisterCaretaker>
    {
        [Test]
        public void AddCaretakerToExistingRepo()
        {
            var username = "testuser";
            var repoName = "nservicebus";
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = repoName
                }
            };

            brain.Set(repos);

            Execute("register", username,"as caretaker for",repoName);

            Assert.AreEqual(repos.Single(r=>r.Name == repoName).Caretaker,username);
        }

        [Test]
        public void AddCaretakerToUnknownRepo()
        {
            brain.Set(new AvailableRepositories());


            Execute("register", "someuser", "as caretaker for", "unknownrepo");

            Assert.True(Messages.Any(m=>m.StartsWith("Repository not found")));
        }
    }
}