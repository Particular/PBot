using NUnit.Framework;
using Octokit;
using PBot;
using PBot.Requirements;

public class MoveConcernsToIAStateAutomaticallyTests : GitHubIntegrationTest
{

    [Test]
    public async System.Threading.Tasks.Task Should_move_new_concerns_to_IA()
    {
        var chore = new MoveConcernsToIAStateAutomatically(GitHubClient, Repository);


        var newConcern = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewConcern("Some concern"));
        var concernAlreadyInIA = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewConcern("Some concern", state: RequirementStates.ImpactAssessment));
        var newRequirement = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, new NewIssue("Some requirement"));
        var closedConcern = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewConcern("Some closed concern"));

        await GitHubClient.Issue.Update(RepositoryOwner, RepositoryName, closedConcern.Number, new IssueUpdate{State = ItemState.Closed});
        
        await chore.Perform();

        newConcern = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, newConcern.Number);

        concernAlreadyInIA = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, concernAlreadyInIA.Number);
        newRequirement = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, newRequirement.Number);


        Assert.True(newConcern.IsInState(RequirementStates.ImpactAssessment), "New concern should be in the IA state");
        Assert.True(concernAlreadyInIA.IsInState(RequirementStates.ImpactAssessment), "Existing IA card still be in the IA state");
        Assert.False(newRequirement.IsInState(RequirementStates.ImpactAssessment), "New requirements should not be moved");
        Assert.False(closedConcern.IsInState(RequirementStates.ImpactAssessment), "Closed concerns should not be moved");
    }
}