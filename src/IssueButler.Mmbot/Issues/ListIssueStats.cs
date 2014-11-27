namespace IssueButler.Mmbot.Issues
{
    using System.Linq;
    using System.Text;
    using IssueButler.Mmbot.Repositories;
    using Octokit;

    public class ListIssueStats : BotCommand
    {
        public ListIssueStats()
            : base(
                "list issue stats$",
                "pbot list issue stats - Shows various issue stats") { }

        public override void Execute(string[] parameters, IResponse response)
        {

            var stats = Brain.Get<AvailableRepositories>()
                .Select(r => GetRepoStats(r.Name))
                .ToList();


            var message = new StringBuilder();

            message.AppendLine("This is the current issue stats: total(bugs)");
            message.AppendLine(string.Format("*All repos* - {0}({1})", stats.Sum(s => s.NumIssues), stats.Sum(s => s.NumBugs)));
           
            foreach (var stat in stats.Where(s=>s.NumIssues > 0)
                .OrderByDescending(s=>s.NumIssues))
            {
                message.AppendLine(string.Format("*{2}* - {0}({1})",stat.NumIssues,stat.NumBugs,stat.RepoName));  
            }

            response.Send(message.ToString());
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