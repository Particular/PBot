namespace PBot.Tests.Integration
{
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Octokit;
    using Issues;

    public class When_transfering_an_issue_between_repos : GitHubIntegrationTest
    {
        protected Repository TargetRepository { get; private set; }

        private Repository SourceRepository => Repository;

        [SetUp]
        public void Setup() => TargetRepository =
            GitHubClient.Repository.Create(new NewRepository($"{Repository.Name}-destination")).Result;

        [TearDown]
        public void TearDown() => Helper.DeleteRepo(TargetRepository);

        [Test]
        [Explicit]
        public async Task Should_assign_label_to_match_original_issue()
        {
            // arrange
            var sourceLabel = await GitHubClient.Issue.Labels.Create(
                SourceRepository.Owner.Login, SourceRepository.Name, new NewLabel("Test-Label", "123456"));

            var newSourceIssue = new NewIssue("test issue with label") { Body = "Issue should have a label" };
            newSourceIssue.Labels.Add(sourceLabel.Name);
            var sourceIssue = await GitHubClient.Issue.Create(
                SourceRepository.Owner.Login, SourceRepository.Name, newSourceIssue);

            // act
            var targetIssue = await IssueUtility.Transfer(
                SourceRepository.Owner.Login,
                SourceRepository.Name,
                sourceIssue.Number,
                TargetRepository.Owner.Login,
                TargetRepository.Name,
                true);

            // assert
            Assert.AreEqual(1, targetIssue.Labels.Count);
            Assert.AreEqual(sourceIssue.Labels.Single().Name, targetIssue.Labels.Single().Name);
            Assert.AreEqual(sourceIssue.Labels.Single().Color, targetIssue.Labels.Single().Color);
        }
    }
}