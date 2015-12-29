using NUnit.Framework;
using Octokit;

public class GitHubIntegrationTest
{
    protected IGitHubClient GitHubClient { get; private set; }

    protected Repository Repository { get; private set; }

    protected string RepositoryOwner { get; private set; }

    protected string RepositoryName { get; private set; }

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
