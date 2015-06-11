namespace PBot.Tests
{
    using System.Linq;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;
    using PBot.Caretakers;

    [TestFixture]
    public class RegisterCaretakerTests:BotCommandFixture<RegisterCaretaker>
    {
        [Test]
        public async void ExplicitUserToExistingRepo()
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

            await Execute("register", username, "as caretaker for", repoName);

            Assert.AreEqual(repos.Single(r=>r.Name == repoName).Caretaker,username);
        }
        [Test]
        public async void CurrentUserToExistingRepo()
        {
            var username = "current";
            var repoName = "nservicebus";
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = repoName
                }
            };

            brain.Set(repos);

            AsUser(username);
            await Execute("register", "my self", "as caretaker for", repoName);

            Assert.AreEqual(repos.Single(r => r.Name == repoName).Caretaker, username);
        }

        [Test]
        public async void AddCaretakerToUnknownRepo()
        {
            brain.Set(new AvailableRepositories());


            await Execute("register", "someuser", "as caretaker for", "unknownrepo");

            Assert.True(Messages.Any(m=>m.StartsWith("Repository not found")));
        }
    }
}