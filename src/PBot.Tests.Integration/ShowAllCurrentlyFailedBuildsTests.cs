using System;
using System.Linq;
using NUnit.Framework;
using PBot;
using PBot.Buildserver;

class ShowAllCurrentlyFailedBuildsTests
{
    [Test, Explicit("Long running")]
    public void Run()
    {
        var client = new TeamCity(Constants.BuildServerRoot);

        var projects = client.ListProjects();

        var projectWithFailures = projects.SelectMany(p => client.ListCurrentBuilds(p.Id, new[]
        {
            "master",
            "develop"
        }))
            .Where(b => b.Status == BuildStatus.Failed)
            .GroupBy(b => b.Project)
            .ToList();

        Console.Out.WriteLine("Currently failed builds");
        foreach (var project in projectWithFailures)
        {
            Console.Out.WriteLine("");
            Console.Out.WriteLine("### " + project.Key.Name);
            Console.Out.WriteLine("");
            var buildTypes = project.GroupBy(b => b.BuildType);

            foreach (var buildType in buildTypes)
            {
                Console.Out.WriteLine("* `" + buildType.Key + "`");

                foreach (var failedBuild in buildType)
                {
                    Console.Out.WriteLine("     - [{0} - {1}]({2})", failedBuild.Number, failedBuild.Branch, failedBuild.Url);
                }
            }
        }
    }
}