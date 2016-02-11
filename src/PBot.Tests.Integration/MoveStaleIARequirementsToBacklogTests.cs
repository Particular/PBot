using System;
using NUnit.Framework;
using PBot;
using PBot.Requirements;

public class MoveStaleIARequirementsToBacklogTests : GitHubIntegrationTest
{

    [Test]
    public async System.Threading.Tasks.Task Should_move_stale_IA_requirements_to_backlog()
    {
        var staleIACard = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("IA Stale", state: RequirementStates.ImpactAssessment));
        var reviewCard = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("Review", state: RequirementStates.Review));


        var chore = new MoveStaleIARequirementsToBacklog(GitHubClient, Repository, TimeSpan.FromDays(-2)); // -2 in order to make it in the future
         await chore.Perform();

        staleIACard = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, staleIACard.Number);
        reviewCard = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, reviewCard.Number);

        Assert.True(staleIACard.IsInState(RequirementStates.Backlogged), "Stale card should be moved to backlog");
        Assert.False(staleIACard.IsInState(RequirementStates.ImpactAssessment), "Stale card should be moved to backlog");
        Assert.True(reviewCard.IsInState(RequirementStates.Review), "Stale card should be moved to backlog");

        Assert.AreEqual(1,staleIACard.Comments,"The bot should comment on the issue");
    }

    [Test]
    public async System.Threading.Tasks.Task Should_leave_non_stale_items_untouched()
    {
        var nonStaleIACard = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("IA", state: RequirementStates.ImpactAssessment));
     

        var chore = new MoveStaleIARequirementsToBacklog(GitHubClient, Repository, TimeSpan.FromDays(14));
        await chore.Perform();

        nonStaleIACard = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, nonStaleIACard.Number);

        Assert.True(nonStaleIACard.IsInState(RequirementStates.ImpactAssessment), "Non stale card should be untouched");
        Assert.AreEqual(0, nonStaleIACard.Comments);
    }
}