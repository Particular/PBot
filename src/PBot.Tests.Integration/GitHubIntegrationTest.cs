using NUnit.Framework;
using Octokit;

public class GitHubIntegrationTest
{
    protected IGitHubClient GitHubClient { get; private set; }

    protected Repository Repository { get; private set; }

    protected string RepositoryOwner => Repository.Owner.Login;

    protected string RepositoryName => Repository.Name;

    [SetUp]
    public void FixtureSetup() =>
        Repository = (GitHubClient = Helper.GetAuthenticatedClient())
            .Repository.Create(new NewRepository(Helper.MakeNameWithTimestamp("requirements-test"))).Result;

    [TearDown]
    public void FixtureTearDown() => Helper.DeleteRepo(Repository);
}