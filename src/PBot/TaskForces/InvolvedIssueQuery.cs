namespace PBot.TaskForces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Octokit;

    internal class InvolvedIssueQuery
    {
        private static readonly Regex TaskForceRegex = new Regex(@"Task[\s-]?Force:\s*(.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex MentionRegex = new Regex(@"@[a-z0-9.-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly GitHubClient client;

        public InvolvedIssueQuery(GitHubClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<InvolvedIssue>> Perform(string username)
        {
            var repos = (await client.Repository.GetAllForOrg("Particular")).ToList();
            var tasks = repos.Select(x => GetInvolvedIssuesForRepo(x, username));

            return (await Task.WhenAll(tasks)).SelectMany(issue => issue);
        }

        private async Task<IEnumerable<InvolvedIssue>> GetInvolvedIssuesForRepo(Repository repo, string username)
        {
            var mentionedFilter = new RepositoryIssueRequest { State = ItemState.Open, Mentioned = username };
            var assigneeFilter = new RepositoryIssueRequest { State = ItemState.Open, Assignee = username };
            var creatorFilter = new RepositoryIssueRequest { State = ItemState.Open, Creator = username };

            var issues = (await client.Issue.GetForRepository("Particular", repo.Name, mentionedFilter))
                .Concat(await client.Issue.GetForRepository("Particular", repo.Name, assigneeFilter))
                .Concat(await client.Issue.GetForRepository("Particular", repo.Name, creatorFilter))
                .GroupBy(issue => issue.Url)
                .Select(g => g.First());

            var involvedIssues = new List<InvolvedIssue>();
            foreach (var issue in issues)
            {
                var isOnTeam = ExtractTeam(issue.Body).ToArray().Contains(username, StringComparer.InvariantCultureIgnoreCase);
                var isOwner = issue.Assignee != null && string.Equals(issue.Assignee.Login, username, StringComparison.InvariantCultureIgnoreCase);
                var isCreator = issue.User != null && string.Equals(issue.User.Login, username, StringComparison.InvariantCultureIgnoreCase);

                if (isOwner || isOnTeam || (isCreator && issue.PullRequest != null))
                {
                    involvedIssues.Add(new InvolvedIssue
                    {
                        Repo = repo,
                        Issue = issue,
                    });
                }
            }

            return involvedIssues;
        }

        private static IEnumerable<string> ExtractTeam(string issueBody)
        {
            if (issueBody == null)
            {
                return Enumerable.Empty<string>();
            }

            var taskForceMatch = TaskForceRegex.Match(issueBody);

            if (!taskForceMatch.Success)
            {
                return Enumerable.Empty<string>();
            }

            return MentionRegex.Matches(taskForceMatch.Result("$1"))
                .Cast<Match>()
                .Select(mentionMatch => mentionMatch.Value.TrimStart('@'));
        }
    }
}
