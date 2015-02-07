using NUnit.Framework;
using Octokit;

public class BacklogStaleRequirementIssuesTests : GitHubIntegrationTest
{
   
    [Test]
    public void Should_backlog_all_discuss_issues_with_no_activity_in_the_last_week()
    {
        //var newIssue = new NewIssue("a test issue") { Body = "A new unassigned issue" };
        //var issue = await _issuesClient.Create(_repositoryOwner, _repositoryName, newIssue);

        //var issueEventInfo = await _issuesEventsClientClient.GetForIssue(_repositoryOwner, _repositoryName, issue.Number);
        //Assert.Empty(issueEventInfo);

        //var closed = _issuesClient.Update(_repositoryOwner, _repository.Name, issue.Number, new IssueUpdate { State = ItemState.Closed })
        //    .Result;
        //Assert.NotNull(closed);
        //issueEventInfo = await _issuesEventsClientClient.GetForIssue(_repositoryOwner, _repositoryName, issue.Number);

        //Assert.Equal(1, issueEventInfo.Count);
        //Assert.Equal(EventInfoState.Closed, issueEventInfo[0].Event);
    }

}

public class GitHubIntegrationTest
{
    protected IGitHubClient GitHubClient;
    //readonly IIssuesEventsClient _issuesEventsClientClient;
    //readonly IIssuesClient _issuesClient;
    protected Repository Repository;
    //readonly string _repositoryOwner;
    //readonly string _repositoryName;

    [TestFixtureSetUp]
    public void FixtureSetup()
    {
        GitHubClient = Helper.GetAuthenticatedClient();

       // _issuesClient = _gitHubClient.Issue;
        var repoName = Helper.MakeNameWithTimestamp("requirements-test");

        Repository = GitHubClient.Repository.Create(new NewRepository { Name = repoName }).Result;
        //_repositoryOwner = _repository.Owner.Login;
        //_repositoryName = _repository.Name;
    }

    [TestFixtureTearDown]
    public void FixtureTearDown()
    {
        Helper.DeleteRepo(Repository);
    }

}