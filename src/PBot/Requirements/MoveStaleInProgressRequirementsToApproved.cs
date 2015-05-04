namespace PBot.Requirements
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class MoveStaleInProgressRequirementsToApproved
    {
        readonly IGitHubClient client;
        readonly Repository repository;
        readonly TimeSpan maxStaleness;

        public MoveStaleInProgressRequirementsToApproved(IGitHubClient client, Repository repository, TimeSpan maxStaleness)
        {
            this.client = client;
            this.repository = repository;
            this.maxStaleness = maxStaleness;
        }

        public async Task Perform()
        {

            var cutoffDate = DateTime.UtcNow - maxStaleness;

            var issues = await client.Issue.GetForRepository(repository.Owner.Login, repository.Name);

            foreach (var issue in issues.Where(i => i.IsInState(RequirementStates.InProgress)))
            {
                if (issue.UpdatedAt < cutoffDate)
                {
                    await client.Issue.Labels.RemoveFromIssue(repository.Owner.Login, repository.Name, issue.Number, RequirementStates.InProgress);
                    await client.Issue.Labels.AddToIssue(repository.Owner.Login, repository.Name, issue.Number, new[] { (string)RequirementStates.Approved });

                    var message = string.Format(
@"
There hasn't been any progress on this one in the last {0} days so I've moved it back to `Approved`.

* Do you think this requirement is stuck and need help? 
* Was there progress and you think I did the wrong thing? Sorry but you're wrong :) During a week there should be at least a few updates either as comments or as items in the `Plan of attack` section?
* Has priorities changed? Is this one still relevant?

Move it back to inprogress when work on this one resumes again!


Ping @particular/requriments or the #requirements slack channel if you need a hand to reevalutate this requirement!
", maxStaleness.TotalDays);

                    await client.Issue.Comment.Create(repository.Owner.Login, repository.Name, issue.Number, message);
                }
            }

        }
    }
}