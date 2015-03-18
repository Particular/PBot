using NUnit.Framework;
using PBot.Requirements;
using PBot.Tests;

public class RemindWhenMoreRequirementsNeedsToBeReviewedTests : GitHubIntegrationTest
{

    [Test]
    public async void Should_remind_when_there_is_more_than_1_requirements_to_be_reviewed()
    {
        var response = new TestResponder();
        await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("Approved1", state: RequirementStates.Approved));
        await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("Approved1", state: RequirementStates.Review));
        var chore = new RemindWhenMoreRequirementsNeedsToBeReviewed(GitHubClient, Repository, response);
        await chore.Perform();
        Assert.AreEqual(1, response.Messages.Count);
    }

    [Test]
    public async void Should_not_remind_when_there_are_no_requirements_to_be_reviewed()
    {
        var response = new TestResponder();
        var chore = new RemindWhenMoreRequirementsNeedsToBeReviewed(GitHubClient, Repository, response);
        await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("Approved1", state: RequirementStates.Approved));
        await chore.Perform();
        Assert.AreEqual(0, response.Messages.Count);
    }
}