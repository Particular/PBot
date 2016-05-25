using System.Linq;
using System.Threading.Tasks;
using IssueButler.Mmbot.Repositories;
using NUnit.Framework;
using PBot;
using PBot.Buildserver;
using PBot.Tests;

class ListCurrentlyFailingBuildsTests : BotCommandFixture<ListCurrentlyFailingBuilds>
{
    [Test]
    public Task ListSpecificProject()
    {
        return Execute("", "NServiceBus.NHibernate");
    }

    [Test]
    public async Task ListNonExistingProject()
    {
        await Execute("", "xyz");

        Assert.True(Messages.First().Contains("`xyz`"));
    }

    [Test, Explicit("Long running")]
    public Task ListAllBuilds()
    {
        return Execute("", "all projects");
    }


    [Test]
    public Task ListMyBuilds()
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
        return Execute("", "my repos");
    }
}