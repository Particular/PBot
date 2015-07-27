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
        static Regex TaskForceRx = new Regex(@"Task[\s-]?Force:\s*(.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex Mentions = new Regex(@"@[a-z0-9.-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        GitHubClient client;

        public InvolvedIssueQuery(GitHubClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<InvolvedIssue>> Perform(string username)
        {
            var repos = (await client.Repository.GetAllForOrg("Particular")).ToList();

            var tasks = repos.Select(x => GetInvolvedIssuesForRepo(x, username)).ToArray();

            await Task.WhenAll(tasks);

            return from task in tasks
                from issue in task.Result
                orderby issue.Involvement descending, issue.Repo.Name ascending
                select issue;
        }

        private async Task<IEnumerable<InvolvedIssue>> GetInvolvedIssuesForRepo(Repository repo, string username)
        {
            var results = new List<InvolvedIssue>();

            var filter = new RepositoryIssueRequest
            {
                State = ItemState.Open,
                Mentioned = username
            };

            var issues = await client.Issue.GetForRepository("Particular", repo.Name, filter);

            foreach (var issue in issues)
            {
                var team = ExtractTeam(issue.Body).ToArray();
                var isOwner = issue.Assignee != null && string.Equals(issue.Assignee.Login, username, StringComparison.InvariantCultureIgnoreCase);
                var isOnTeam = team.Contains(username, StringComparer.InvariantCultureIgnoreCase);

                results.Add(new InvolvedIssue
                {
                    Repo = repo,
                    Issue = issue,
                    Involvement = isOwner || isOnTeam ? IssueInvolvement.Pig : IssueInvolvement.Chicken,
                });
            }

            return results;
        }

        private static IEnumerable<string> ExtractTeam(string issueBody)
        {
            if (issueBody == null)
                return Enumerable.Empty<string>();

            var match = TaskForceRx.Match(issueBody);

            if (!match.Success)
                return Enumerable.Empty<string>();

            return Mentions.Matches(match.Result("$1"))
                .Cast<Match>()
                .Select(x => x.Value.TrimStart('@'));
        }
    }
}
