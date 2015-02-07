using NUnit.Framework;
using Octokit;

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