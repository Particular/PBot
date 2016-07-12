namespace PBot.Issues
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Octokit;

    class InvolvedIssueQuery
    {
        static Regex TaskForceRegex = new Regex(@"Task[\s-]?Force:\s*(.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex MentionRegex = new Regex("@[a-z0-9.-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        GitHubClient client;

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

        async Task<IEnumerable<InvolvedIssue>> GetInvolvedIssuesForRepo(Repository repo, string username)
        {
            var mentionedFilter = new RepositoryIssueRequest { State = ItemState.Open, Mentioned = username };
            var assigneeFilter = new RepositoryIssueRequest { State = ItemState.Open, Assignee = username };
            var creatorFilter = new RepositoryIssueRequest { State = ItemState.Open, Creator = username };

            var mentionedQuery = Task.Run(
                async () => await client.Issue.GetAllForRepository("Particular", repo.Name, mentionedFilter));

            var assigneeQuery = Task.Run(
                async () => await client.Issue.GetAllForRepository("Particular", repo.Name, assigneeFilter));

            var creatorQuery = Task.Run(
                async () => await client.Issue.GetAllForRepository("Particular", repo.Name, creatorFilter));

            await Task.WhenAll(mentionedQuery, assigneeQuery, creatorQuery);

            return
                    mentionedQuery.Result
                        .Where(issue => ExtractTeam(issue.Body).ToArray()
                        .Contains(username, StringComparer.InvariantCultureIgnoreCase))
                .Concat(
                    assigneeQuery.Result)
                .Concat(
                    creatorQuery.Result
                        .Where(issue => issue.PullRequest != null))
                .GroupBy(issue => issue.Url)
                .Select(g => g.First())
                .Select(issue => new InvolvedIssue { Issue = issue, Repo = repo });
        }

        static IEnumerable<string> ExtractTeam(string issueBody)
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