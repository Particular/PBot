namespace PBot.Tests
{
    using System.Linq;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;
    using PBot.Reminders;

    [TestFixture]
    public class RemindCaretakersOfIssuesToBeHandledTests : BotCommandFixture<RemindCaretakersOfIssuesToBeHandled>
    {
        [Test]
        public async void MakeSureOnlyReposWithACaretakerIsChecked()
        {
            var username = "testuser";
            var repoName = "PBot.TestRepo";
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

            await Execute("remind caretakers of issues if needed");

            Assert.NotNull(Messages.Single(m => m.Contains("PBot.TestRepo")));
        }
    }
}