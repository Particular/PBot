namespace PBot.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using PBot.Caretakers;
    using PBot.Repositories;

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