using System;
using System.Linq;
using NUnit.Framework;
using PBot;
using PBot.Buildserver;

class TeamCityTests
{
    [Test]
    public void Should_list_projects()
    {
        var projects = GetClient().ListProjects()
            .ToList();

        foreach (var project in projects)
        {
            Console.Out.WriteLine(project);
        }

        Assert.Greater(projects.Count(), 0);
    }

    [Test]
    public void Should_get_projects_details()
    {
        var project = GetClient().GetProject("NServiceBus");

        Assert.AreEqual("NServiceBus", project.Name);
        Assert.AreEqual($"{Constants.BuildServerRoot}project.html?projectId=NServiceBus", project.Url);
        Assert.Greater(project.BuildTypes.Count, 0);
        Assert.NotNull(project.BuildTypes.First().Id);
    }

    [Test]
    public void Should_detect_current_build_status()
    {
        var client = GetClient();
        var branch = "master";

        Console.Out.WriteLine(client.IsBuildCurrentlyFailed("NServiceBus", "NServiceBusCore_Build", branch));
        Console.Out.WriteLine(client.IsBuildCurrentlyFailed("ServiceControl", "bt58", branch));
    }

    [Test]
    public void Should_list_currently_failed_builds_for_project()
    {
        var client = GetClient();

        var failedBuilds = client.ListCurrentBuilds("ServiceControl", new[] { "master", "develop" })
            .Where(b => b.Status == BuildStatus.Failed)
            .ToList();

        foreach (var failedBuild in failedBuilds)
        {
            Console.Out.WriteLine(failedBuild.BuildType + ": " + failedBuild);
        }
    }

    [Test]
    public void Should_get_build_details()
    {
        var client = GetClient();

        var build = client.ListCurrentBuilds("ServiceControl", new[]
        {
            "master"
        }).First();

        var details = client.GetBuild(build.Id);

        Assert.AreEqual(build.Id, details.Id);

        Assert.GreaterOrEqual(DateTime.UtcNow, details.FinishedAt);
    }

    TeamCity GetClient()
    {
        return new TeamCity(Constants.BuildServerRoot);
    }
}