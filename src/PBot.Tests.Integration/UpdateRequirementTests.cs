using System;
using System.Linq;
using MMBot.Brains;
using NUnit.Framework;
using Octokit;
using PBot;
using PBot.Requirements;
using PBot.Tests;
using PBot.Users;
using User = MMBot.User;

public class UpdateRequirementTests : GitHubIntegrationTest
{
    [Test]
    public async void ShouldUpdateIssueUsingTemplate()
    {
        var command = CreateCommand(Environment.GetEnvironmentVariable("PBOT_OAUTHTOKEN"));
        var responder = new TestResponder();

        var existingIssue = await GitHubClient.Issue.Create(RepositoryOwner, RepositoryName, new NewIssue("Some title")
        {
            Body = "This is a requirement."
        });

        await command.Execute(new[]
        {
            "pbot update requirement",
            existingIssue.Number.ToString()
        }, responder);

        var allIssues = await GitHubClient.Issue.GetForRepository(RepositoryOwner, RepositoryName);

        var issue = allIssues.Single();

        Assert.AreEqual("Some title", issue.Title);
        StringAssert.StartsWith(CreateRequirement.BodyTemplate, issue.Body);
        StringAssert.EndsWith("This is a requirement.", issue.Body);
        Assert.True(responder.Messages.Single().Contains(issue.HtmlUrl.ToString()));
    }

    [Test]
    public async void ShouldRequireToken()
    {
        var command = CreateCommand(null);
        var responder = new TestResponder();

        await command.Execute(new[]
        {
            "pbot update requirement",
            "1"
        }, responder);

        var allIssues = await GitHubClient.Issue.GetForRepository(RepositoryOwner, RepositoryName);

        Assert.False(allIssues.Any());
        Assert.True(responder.Messages.Single().Contains("github access token"));
    }

    UpdateRequirement CreateCommand(string token)
    {
        var command = new UpdateRequirement(RepositoryOwner, RepositoryName);
        IBrain brain = new TestBrain();

        var store = new CredentialStore();

        if (token != null)
        {
            store.Add("test", "github-accesstoken", token);
        }


        brain.Set(store);
        command.CurrentUser = new User("xyz", "test", null, "", "");


        command.Register(brain);
        return command;
    }
}