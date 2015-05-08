using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Octokit;
using PBot.Issues;

public class When_transfering_an_issue_between_repos : GitHubIntegrationTest
{
    [Test]
    [Explicit]
    public async Task Should_assign_label_to_match_original_issue()
    {
        var srcRepoInfo = new RepoInfo { Name = RepositoryName, Owner = RepositoryOwner };
        var dstRepoInfo = new RepoInfo { Name = RepositoryName + "-copy", Owner = RepositoryOwner };
        var dstRepo = await GitHubClient.Repository.Create(new NewRepository { Name = dstRepoInfo.Name });

        var labelTemplate = new NewLabel("Test-Label", "123456");
        var label = await GitHubClient.Issue.Labels.Create(srcRepoInfo.Owner, srcRepoInfo.Name, labelTemplate);
        await GitHubClient.Issue.Labels.Create(dstRepoInfo.Owner, dstRepoInfo.Name, labelTemplate);
        var issue = new NewIssue("test issue with label") { Body = "Issue should have a label" };
        issue.Labels.Add(label.Name);
        var originalIssue = await GitHubClient.Issue.Create(srcRepoInfo.Owner, srcRepoInfo.Name, issue);

        var copiedIssue = IssueUtility.Transfer(srcRepoInfo, originalIssue.Number, dstRepoInfo, true).Result;

        Helper.DeleteRepo(dstRepo);

        Assert.AreEqual(originalIssue.Labels.Count, copiedIssue.Labels.Count);
        Assert.AreEqual(originalIssue.Labels.First().Name, copiedIssue.Labels.First().Name);
        Assert.AreEqual(originalIssue.Labels.First().Color, copiedIssue.Labels.First().Color);
    }
}