namespace PBot.Tests.Integration
{
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Octokit;
    using PBot.Issues;

    public class When_transfering_an_issue_between_repos : GitHubIntegrationTest
    {
        protected Repository TargetRepository { get; private set; }

        private Repository SourceRepository
        {
            get { return Repository; }
        }

        [SetUp]
        public void Setup()
        {
            var newRepository = new NewRepository { Name = $"{Repository.Name}-destination" };
            TargetRepository = GitHubClient.Repository.Create(newRepository).Result;
        }

        [TearDown]
        public void TearDown()
        {
            Helper.DeleteRepo(TargetRepository);
        }

        [Test]
        [Explicit]
        public async Task Should_assign_label_to_match_original_issue()
        {
            // arrange
            var newLabel = new NewLabel("Test-Label", "123456");
            await GitHubClient.Issue.Labels.Create(SourceRepository.Owner.Login, SourceRepository.Name, newLabel);

            var newIssue = new NewIssue("test issue with label") { Body = "Issue should have a label" };
            newIssue.Labels.Add(newLabel.Name);
            var sourceIssue = await GitHubClient.Issue.Create(
                SourceRepository.Owner.Login, SourceRepository.Name, newIssue);

            // act
            var targetIssue = await IssueUtility.Transfer(
                new RepoInfo { Owner = SourceRepository.Owner.Login, Name = SourceRepository.Name },
                sourceIssue.Number,
                new RepoInfo { Owner = TargetRepository.Owner.Login, Name = TargetRepository.Name },
                true);

            // assert
            Assert.AreEqual(1, targetIssue.Labels.Count);
            Assert.AreEqual(sourceIssue.Labels.Single().Name, targetIssue.Labels.Single().Name);
            Assert.AreEqual(sourceIssue.Labels.Single().Color, targetIssue.Labels.Single().Color);
        }
    }
}
