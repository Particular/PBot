namespace PBot.Requirements
{
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class MoveConcernsToIAStateAutomatically
    {
        IGitHubClient client;
        Repository repository;

        public MoveConcernsToIAStateAutomatically(IGitHubClient client, Repository repository)
        {
            this.client = client;
            this.repository = repository;
        }

        public async Task Perform()
        {
            var issues = await client.Issue.GetAllForRepository(repository.Owner.Login, repository.Name);

            foreach (var issue in issues.Where(i => i.Labels.Any(l => l.Name == RequirementTypes.Concern)))
            {
                if (issue.IsInInitialState<RequirementStates>())
                {
                    await client.Issue.Labels.AddToIssue(repository.Owner.Login, repository.Name, issue.Number, new[] { (string)RequirementStates.ImpactAssessment });
                }
            }
        }
    }
}