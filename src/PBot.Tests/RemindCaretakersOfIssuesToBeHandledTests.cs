namespace PBot.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using PBot.Reminders;
    using PBot.Repositories;

    [TestFixture]
    public class RemindCaretakersOfIssuesToBeHandledTests : BotCommandFixture<RemindCaretakersOfIssuesToBeHandled>
    {
        [Test]
        public void MakeSureOnlyReposWithACaretakerIsChecked()
        {
            var username = "testuser";
            var repoName = "nservicebus";
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = repoName,
                    Caretaker = username
                },

                new AvailableRepositories.Repository
                {
                    Name = "RepoWithNoCaretaker"
                }
            };

            brain.Set(repos);

            Execute("remind caretakers of issues if needed");

            Assert.AreEqual(1,Messages.Count());
            Assert.True(Messages.First().Contains(repoName));
            Assert.True(Messages.First().Contains(username));
        }
    }
}