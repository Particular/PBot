using System.Threading.Tasks;
using Octokit;
using PBot;
using System.Linq;
using PBot.Requirements;

public class RemindWhenMoreRequirementsNeedsToBeApproved
{
    readonly IGitHubClient client;
    readonly Repository repository;
    readonly BotCommand.IResponse response;

    public RemindWhenMoreRequirementsNeedsToBeApproved(IGitHubClient client, Repository repository, BotCommand.IResponse response)
    {
        this.client = client;
        this.repository = repository;
        this.response = response;
    }

    public async Task Perform()
    {
        var issues = await client.Issue.GetForRepository(repository.Owner.Login, repository.Name);


        var numApproved = issues.Count(i => i.Labels.Any(l => l.Name == RequirementStates.Approved));

        if (numApproved < 2)
        {
            await response.Send(string.Format("There is only {0} approved requirements, please move more to the `Approved` swimlane!", numApproved));
        }
    }
}