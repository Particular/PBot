namespace PBot.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Octokit;

    public class List_my_TaskForces //: BotCommandFixture<ListIssuesFromExternalContributors>
    {
        [Test, Explicit]
        public async void All()
        {
            Console.Out.WriteLine("### My Task forces");
            var sw = Stopwatch.StartNew();

            var client = GitHubClientBuilder.Build();

            var query = new InvolvedIssueQuery(client);

            var results = (from issue in await query.Perform("wolfbyte")
                           where issue.Involvement == IssueInvolvement.Pig
                           group issue by issue.Repo.Name
                           into g
                           select g).ToList();

            foreach (var result in results)
            {
                Console.Out.WriteLine("*[{0}]*", result.Key);
                foreach (var issue in result)
                {
                    Console.Out.WriteLine("\t- _{0}_ ({1})", issue.Issue.Title, issue.Issue.HtmlUrl);
                    Console.Out.WriteLine("\tLabels: {0}", string.Join(" ", issue.Issue.Labels.Select(l => "`" + l.Name + "`")));
                    Console.Out.WriteLine("\tTeam: {0}", string.Join(", ", issue.Team));
                }
            }

            sw.Stop();
            Console.Out.WriteLine("Total time (in seconds): {0}", sw.Elapsed.TotalSeconds);
        }
    }

    public enum IssueInvolvement
    {
        Chicken,
        Pig
    }

    public class InvolvedIssue
    {
        public Repository Repo { get; set; }
        public Issue Issue { get; set; }
        public IssueInvolvement Involvement { get; set; }
        public string[] Team { get; set; }
    }

    public class InvolvedIssueQuery
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

            var issues = await client.Issue.GetAllForRepository("Particular", repo.Name, filter);

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
                    Team = team
                });
            }

            return results;
        }

        private IEnumerable<string> ExtractTeam(string issueBody)
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