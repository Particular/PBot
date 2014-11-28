namespace IssueButler.Mmbot.Issues
{
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;
    using Octokit;

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

            await response.Send("This is the current issue stats: total(bugs)").IgnoreWaitContext();
            await response.Send(string.Format("*All repos* - {0}({1})", stats.Sum(s => s.NumIssues), stats.Sum(s => s.NumBugs))).IgnoreWaitContext();

            foreach (var stat in stats.Where(s => s.NumIssues > 0)
                .OrderByDescending(s => s.NumIssues))
            {
                await response.Send(string.Format("*{2}* - {0}({1})", stat.NumIssues, stat.NumBugs, stat.RepoName)).IgnoreWaitContext();
            }

            await response.Send("Hey, go a fix some! Use `pbot check repo {name of repo above}` to find a few").IgnoreWaitContext();
        }

        RepoStats GetRepoStats(string name)
        {
            var client = GitHubClientBuilder.Build();

            var issues = client.Issue.GetForRepository("Particular", name, new RepositoryIssueRequest { State = ItemState.Open })
                .Result;

            return new RepoStats
            {
                RepoName = name,
                NumBugs = issues.Count(i => i.Labels.Any(l => l.Name == "Bug")),
                NumIssues = issues.Count
            };
        }
    }

    class RepoStats
    {
        public string RepoName;
        public int NumIssues;
        public int NumBugs;
    }
}