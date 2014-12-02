namespace PBot.Issues
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;
    using PBot.Repositories;

    public class ListIssueStats : BotCommand
    {
        public ListIssueStats()
            : base(
                "list issue stats$",
                "pbot list issue stats - Shows various issue stats") { }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var stats = Brain.Get<AvailableRepositories>()
                .Select(r => GetRepoStats(r.Name))
                .ToList();

            await response.Send("This is the current issue stats: total(trend) bugs").IgnoreWaitContext();
            await response.Send(string.Format("*All repos* - {0}({2}) B:{1}", stats.Sum(s => s.NumIssues), stats.Sum(s => s.NumBugs), stats.Sum(s => s.Trend))).IgnoreWaitContext();

            foreach (var stat in stats.Where(s => s.NumIssues > 0)
                .OrderByDescending(s => s.NumIssues))
            {
                await response.Send(string.Format("*{2}* - T:{0}({3}), Bugs:{1}", stat.NumIssues, stat.NumBugs, stat.RepoName,stat.Trend)).IgnoreWaitContext();
            }

            await response.Send("Hey, why don't you go ahead and fix some! Use `pbot check repo {name of repo above}` to find a few").IgnoreWaitContext();
        }

        RepoStats GetRepoStats(string name)
        {
            var cutOffDate = DateTimeOffset.UtcNow.AddDays(-7);
            var client = GitHubClientBuilder.Build();

            var issues = client.Issue.GetForRepository("Particular", name, new RepositoryIssueRequest { State = ItemState.Open })
                .Result;

            var trend = client.Issue.GetForRepository("Particular", name, new RepositoryIssueRequest { State = ItemState.All, Since = cutOffDate })
              .Result
              .Where(i => i.CreatedAt >= cutOffDate || i.ClosedAt >= cutOffDate);

            return new RepoStats
            {
                RepoName = name,
                NumBugs = issues.Count(i => i.Labels.Any(l => l.Name == "Bug")),
                NumIssues = issues.Count,
                Trend = trend.Count(i => i.State == ItemState.Open) - trend.Count(i => i.State == ItemState.Closed)

            };
        }
    }

    class RepoStats
    {
        public string RepoName;
        public int NumIssues;
        public int NumBugs;
        public int Trend;
    }
}