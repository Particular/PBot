namespace PBot.Tests
{
    using System.Linq;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;
    using PBot.Caretakers;

    [TestFixture]
    public class ListCaretakersTests : BotCommandFixture<ListCaretakers>
    {
        [Test]
        public async System.Threading.Tasks.Task ShouldGroupTheRepos()
        {
            var username = "testuser";
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = "Repo1",
                    Caretaker = username
                },

                   new AvailableRepositories.Repository
                {
                    Name = "Repo2",
                    Caretaker = username
                },

                new AvailableRepositories.Repository
                {
                    Name = "RepoWithNoCaretaker"
                }
            };

            brain.Set(repos);

            await Execute("list caretakers");

            var caretaker = Messages.Single(l=>l.Contains(username));

            Assert.True(caretaker.Contains("Repo1"));
            Assert.True(caretaker.Contains("Repo2"));
        }
    }
}