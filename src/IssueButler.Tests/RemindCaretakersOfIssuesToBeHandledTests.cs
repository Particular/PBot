namespace IssueButler.Tests
{
    using System.Linq;
    using IssueButler.Mmbot;
    using IssueButler.Mmbot.Reminders;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;

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