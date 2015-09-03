namespace PBot.Requirements
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class MoveStaleRequirementIdeasToBacklog
    {
        readonly IGitHubClient client;
        readonly Repository repository;
        readonly TimeSpan maxStaleness;

        public MoveStaleRequirementIdeasToBacklog(IGitHubClient client, Repository repository, TimeSpan maxStaleness)
        {
            this.client = client;
            this.repository = repository;
            this.maxStaleness = maxStaleness;
        }

        public async Task Perform()
        {
            var cutoffDate = DateTime.UtcNow - maxStaleness;

            var issues = await client.Issue.GetForRepository(repository.Owner.Login, repository.Name);

            foreach (var issue in issues.Where(i => i.IsInInitialState<RequirementStates>()))
            {
                if (issue.UpdatedAt < cutoffDate)
                {
                    await client.Issue.Labels.AddToIssue(repository.Owner.Login, repository.Name, issue.Number, new[] { (string)RequirementStates.Backlogged });

                    var message = string.Format("There hasn't been any progress on this one in the last {0} days so I've backlogged it. Don't worry, just move this one to `ImpactAssessment` if/when you start trying to get this one approved again", maxStaleness.TotalDays);

                    await client.Issue.Comment.Create(repository.Owner.Login, repository.Name, issue.Number, message);
                }
            }

        }
    }
}