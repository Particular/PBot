namespace PBot.Requirements
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class MoveStaleIARequirementsToBacklog
    {
        IGitHubClient client;
        Repository repository;
        TimeSpan maxStaleness;

        public MoveStaleIARequirementsToBacklog(IGitHubClient client, Repository repository, TimeSpan maxStaleness)
        {
            this.client = client;
            this.repository = repository;
            this.maxStaleness = maxStaleness;
        }

        public async Task Perform()
        {
            var cutoffDate = DateTime.UtcNow - maxStaleness;

            var issues = await client.Issue.GetAllForRepository(repository.Owner.Login, repository.Name);

            foreach (var issue in issues.Where(i => i.IsInState(RequirementStates.ImpactAssessment)))
            {
                if (issue.UpdatedAt < cutoffDate)
                {
                    await client.Issue.Labels.RemoveFromIssue(repository.Owner.Login, repository.Name, issue.Number, RequirementStates.ImpactAssessment);
                    await client.Issue.Labels.AddToIssue(repository.Owner.Login, repository.Name, issue.Number, new[] { (string)RequirementStates.Backlogged });

                    var message = $"There hasn't been any progress on this one in the last {maxStaleness.TotalDays} days so I've backlogged it. Please move this one back to `ImpactAssessment` if you still believe this is a high priority";

                    await client.Issue.Comment.Create(repository.Owner.Login, repository.Name, issue.Number, message);
                }
            }
        }
    }
}