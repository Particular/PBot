using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace PBot.Requirements
{
    using System.Text;

    public class RemindWhenMoreRequirementsNeedsToBeReviewed
    {
        readonly IGitHubClient client;
        readonly Repository repository;
        readonly BotCommand.IResponse response;

        public RemindWhenMoreRequirementsNeedsToBeReviewed(IGitHubClient client, Repository repository, BotCommand.IResponse response)
        {
            this.client = client;
            this.repository = repository;
            this.response = response;
        }

        public async Task Perform()
        {

            var allIssues = await client.Issue.GetForRepository(repository.Owner.Login, repository.Name);
            var issuesToBeReviewed = allIssues.Where(i => i.Labels.Any(l => l.Name == RequirementStates.Review)).ToList();

            if (issuesToBeReviewed.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"There are {issuesToBeReviewed.Count} issue(s) waiting to be approved in requirements. Please review the following issues:");
                foreach (var issue in issuesToBeReviewed)
                {
                    sb.AppendLine(issue.HtmlUrl.ToString());
                }
                await response.Send(sb.ToString());
            }
        }
    }
}