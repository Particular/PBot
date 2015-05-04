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

                    var message = @"
During the last week I haven't noticed any updates either as comment or as items in the `Plan of attack` section beeing changed or completed. I've therefor decided to moved it back to `Approved`.

* Do you think this requirement is stuck and need help? 
* Has priorities changed? 
* Is this one still relevant?

Move it back to `In progress` when work on this one resumes again!

Ping @particular/requirements or the #requirements slack channel if you need a hand to reevalutate this requirement!
";

                    await client.Issue.Comment.Create(repository.Owner.Login, repository.Name, issue.Number, message);
                }
            }

        }
    }
}