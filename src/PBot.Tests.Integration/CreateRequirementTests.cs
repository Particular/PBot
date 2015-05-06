using System;
using System.Linq;
using MMBot;
using MMBot.Brains;
using NUnit.Framework;
using PBot;
using PBot.Requirements;
using PBot.Tests;
using PBot.Users;

public class CreateRequirementTests : GitHubIntegrationTest
{
    [Test]
    public async void ShouldCreateIssueUsingTemplate()
    {
        var command = CreateCommand(Environment.GetEnvironmentVariable("PBOT_OAUTHTOKEN"));
        var responder = new TestResponder();

        await command.Execute(new[]
        {
            "pbot create requirement",
            "Some title"
        }, responder);

        var allIssues = await GitHubClient.Issue.GetForRepository(RepositoryOwner, RepositoryName);

        var issue = allIssues.Single();

        Assert.AreEqual("Some title",issue.Title);
        Assert.True(responder.Messages.Single().Contains(issue.HtmlUrl.ToString()));
    }

    [Test]
    public async void ShouldRequireToken()
    {
        var command = CreateCommand(null);
        var responder = new TestResponder();

        await command.Execute(new[]
        {
            "pbot create requirement",
            "Some title"
        }, responder);

        var allIssues = await GitHubClient.Issue.GetForRepository(RepositoryOwner, RepositoryName);

        Assert.False(allIssues.Any());
        Assert.True(responder.Messages.Single().Contains("github access token"));
    }

    CreateRequirement CreateCommand(string token)
    {
        var command = new CreateRequirement(RepositoryOwner, RepositoryName);
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