namespace IssueButler.Tests
{
    using System;
    using System.Linq;
    using IssueButler.Mmbot.Caretakers;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;

    [TestFixture]
    public class ManageCaretakersTests
    {
        [Test]
        public void AddCaretakerToExistingRepo()
        {
            var username = "testuser";
            var repoName = "nservicebus";
            var brain = new TestBrain();
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = repoName
                }
            };

            brain.Set(typeof(AvailableRepositories).FullName,repos);


            new ManageCaretakers(brain)
                .AddCaretaker(username,repoName);

            Assert.AreEqual(repos.Single(r=>r.Name == repoName).Caretaker,username);
        }

        [Test]
        public void AddCaretakerToUnknownRepo()
        {
            var username = "testuser";
            var repoName = "nservicebus";
            var brain = new TestBrain();
            var repos = new AvailableRepositories();

            brain.Set(typeof(AvailableRepositories).FullName, repos);


            var ex = Assert.Throws<Exception>(()=> new ManageCaretakers(brain)
                .AddCaretaker(username, repoName));

            Assert.True(ex.Message.StartsWith("Repository not found"));
        }
    }
}