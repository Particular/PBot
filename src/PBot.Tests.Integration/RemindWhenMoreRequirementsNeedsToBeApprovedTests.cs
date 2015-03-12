using NUnit.Framework;
using PBot.Requirements;
using PBot.Tests;

public class RemindWhenMoreRequirementsNeedsToBeApprovedTests : GitHubIntegrationTest
{

    [Test]
    public async void Should_remind_when_less_than_2_requirements_are_approved()
    {
        var response = new TestResponder();

        await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("Approved1", state: RequirementStates.Approved));

        var chore = new RemindWhenMoreRequirementsNeedsToBeApproved(GitHubClient, Repository, response);


      
        await chore.Perform();

        Assert.AreEqual(1, response.Messages.Count);
    }

    [Test]
    public async void Should_not_remind_when_2_or_more_requirements_are_approved()
    {
        var response = new TestResponder();


        await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("Approved1", state: RequirementStates.Approved));
        await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, Requirements.NewFeature("Approved2", state: RequirementStates.Approved));


        var chore = new RemindWhenMoreRequirementsNeedsToBeApproved(GitHubClient, Repository, response);

        await chore.Perform();

        Assert.AreEqual(0, response.Messages.Count);
    }
}