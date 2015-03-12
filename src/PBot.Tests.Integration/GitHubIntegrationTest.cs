using NUnit.Framework;
using Octokit;

public class GitHubIntegrationTest
{
    protected IGitHubClient GitHubClient;
    protected Repository Repository;
    protected string RepositoryOwner;
    protected string RepositoryName;

    [SetUp]
    public void FixtureSetup()
    {
        GitHubClient = Helper.GetAuthenticatedClient();

        var repoName = Helper.MakeNameWithTimestamp("requirements-test");

        Repository = GitHubClient.Repository.Create(new NewRepository { Name = repoName }).Result;
        RepositoryOwner = Repository.Owner.Login;
        RepositoryName = Repository.Name;
    }

    [TearDown]
    public void FixtureTearDown()
    {
        Helper.DeleteRepo(Repository);
    }

}