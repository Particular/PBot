using NUnit.Framework;
using PBot;
using PBot.Requirements;

public class When_adding_a_new_repo_to_pbot : GitHubIntegrationTest
{
    [Test]
    [Explicit("Requires a repo that PBot has no access to")]
    public void Should_fail_in_case_pbot_has_no_access_to_repo()
    {
        var pbotHasAccessToRepo = new PBotHasAccessToRepositoryValidator(GitHubRepoCollaboratorsClientBuilder.Build(), RepositoryName).Perform().Result;
        Assert.IsFalse(pbotHasAccessToRepo, "PBot should not have access to a dynamically generated repo");
    }
}