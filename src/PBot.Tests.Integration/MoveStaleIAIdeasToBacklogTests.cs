using System;
using NUnit.Framework;
using PBot;
using PBot.Requirements;

public class MoveStaleIAIdeasToBacklogTests : GitHubIntegrationTest
{

    [Test]
    public async void Should_move_stale_cards_to_backlog()
    {
        var staleNewCard = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("Stale new idea"));
        var reviewCard = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("Review", state: RequirementStates.Review));


        var chore = new MoveStaleRequirementIdeasToBacklog(GitHubClient, Repository, TimeSpan.FromDays(-2)); // -2 in order to make it in the future
        await chore.Perform();

        staleNewCard = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, staleNewCard.Number);
        reviewCard = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, reviewCard.Number);

        Assert.True(staleNewCard.IsInState(RequirementStates.Backlogged), "Stale card should be moved to backlog");
        Assert.True(reviewCard.IsInState(RequirementStates.Review), "Stale card should be moved to backlog");

        Assert.AreEqual(1, staleNewCard.Comments, "The bot should comment on the issue");
    }

    [Test]
    public async void Should_leave_non_stale_cards_untouched()
    {
        var nonStaleIACard = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("IA", state: RequirementStates.ImpactAssessment));


        var chore = new MoveStaleRequirementIdeasToBacklog(GitHubClient, Repository, TimeSpan.FromDays(14));
        await chore.Perform();

        nonStaleIACard = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, nonStaleIACard.Number);

        Assert.True(nonStaleIACard.IsInState(RequirementStates.ImpactAssessment), "Non stale card should be untouched");
        Assert.AreEqual(0, nonStaleIACard.Comments);
    }
}