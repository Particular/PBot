using System;
using System.Linq;
using NUnit.Framework;
using PBot;
using PBot.Requirements;

public class MoveStaleInProgressRequirementsToApprovedTests : GitHubIntegrationTest
{
    [Test]
    public async void Should_move_stale_InProgress_requirements_back_to_approved()
    {
        var newStaleIssue = Requirements.NewFeature("InProgress Stale", state: RequirementStates.InProgress);

        newStaleIssue.Assignee = RepositoryOwner;

        var staleInProgressCard = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, newStaleIssue);
        var reviewCard = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("Review", state: RequirementStates.Review));

        var chore = new MoveStaleInProgressRequirementsToApproved(GitHubClient, Repository, TimeSpan.FromDays(-2)); // -2 in order to make it in the future
        await chore.Perform();

        staleInProgressCard = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, staleInProgressCard.Number);

        reviewCard = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, reviewCard.Number);

        Assert.True(staleInProgressCard.IsInState(RequirementStates.Approved), "Stale card should be moved to approved");
        Assert.False(staleInProgressCard.IsInState(RequirementStates.InProgress), "Stale card should be moved to approved");
        Assert.True(reviewCard.IsInState(RequirementStates.Review), "Card not inprogress should be ontouched");

        Assert.AreEqual(1, staleInProgressCard.Comments, "The bot should comment on the issue");

        var comments = await GitHubClient.Issue.Comment.GetAllForIssue(RepositoryOwner, RepositoryName, staleInProgressCard.Number);

        var comment = comments.Single().Body;

        Assert.True(comment.Contains("@" + RepositoryOwner), "Comment doesn't ping the task lead: " + comment);
    }

    [Test]
    public async void Should_leave_non_stale_items_untouched()
    {
        var nonStaleInProgressCard = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("InProgress", state: RequirementStates.InProgress));

        var chore = new MoveStaleInProgressRequirementsToApproved(GitHubClient, Repository, TimeSpan.FromDays(7));
        await chore.Perform();

        nonStaleInProgressCard = await GitHubClient.Issue.Get(RepositoryOwner, RepositoryName, nonStaleInProgressCard.Number);

        Assert.True(nonStaleInProgressCard.IsInState(RequirementStates.InProgress), "Non stale card should be untouched");
        Assert.AreEqual(0, nonStaleInProgressCard.Comments);
    }
}