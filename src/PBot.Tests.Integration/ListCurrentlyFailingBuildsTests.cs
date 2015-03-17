using System.Linq;
using IssueButler.Mmbot.Repositories;
using NUnit.Framework;
using PBot;
using PBot.Buildserver;
using PBot.Tests;

class ListCurrentlyFailingBuildsTests : BotCommandFixture<ListCurrentlyFailingBuilds>
{
    [Test]
    public void ListSpecificProject()
    {
        Execute(new[] { "", "NServiceBus.NHibernate" });
    }

    [Test]
    public void ListNonExistingProject()
    {
        Execute(new[] { "", "xyz" });

        Assert.True(Messages.First().Contains("`xyz`"));
    }

    [Test, Explicit("Long running")]
    public void ListAllBuilds()
    {
        Execute(new[] { "", "all projects" });
    }


    [Test]
    public void ListMyBuilds()
    {
        var username = "testuser";
        var repos = new AvailableRepositories
        {
            new AvailableRepositories.Repository
            {
                Name = "NServiceBus",
                Caretaker = username
            },

            new AvailableRepositories.Repository
            {
                Name = "ServiceControl",
                Caretaker = username
            }
        };

        brain.Set(repos);
        AsUser(username);
        Execute(new[] { "", "my repos" });
    }
}