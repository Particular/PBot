using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace PBot.Issues
{
    public class Burndown : BotCommand
    {
        public Burndown() : base("burndown (.*)$", "pbot burndown <tag> - Generates a burndown chart for the last month of a given tag.")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var client = GitHubClientBuilder.Build();

            var issue = new SearchIssuesRequest("");
            issue.User = "Particular";
            issue.Labels = new[] { parameters[1] };
            issue.SortField = IssueSearchSort.Updated;
            var searchresults = await client.Search.SearchIssues(issue);

            var createdDates = searchresults.Items.Select(i => i.CreatedAt).ToList();

            var closedDates = searchresults.Items.Where(i => i.State == ItemState.Closed).Select(i => i.ClosedAt.Value).ToList();

            var minAndMaxDates = createdDates.Concat(closedDates)
                .Aggregate(
                    Tuple.Create(DateTimeOffset.MaxValue, DateTimeOffset.MinValue),
                    (acc, date) => Tuple.Create(Min(acc.Item1, date), Max(acc.Item2, date)));

            var chartUrl = new StringBuilder();
            chartUrl.Append("https://chart.googleapis.com/chart");
            chartUrl.Append("?cht=lc"); // Line chart
            chartUrl.Append("&chs=1000x300"); // Chart Size
            chartUrl.Append("&chds=a"); // Autoscale
            chartUrl.Append("&chxt=x,y"); // Show x & y labels

            var values = new List<string>();
            var label = new List<string>();
            for (var i = minAndMaxDates.Item2.AddMonths(-1); i < minAndMaxDates.Item2; i = i.AddDays(1))
            {
                var created = createdDates.Count(d => d <= i);
                var closed = closedDates.Count(d => d <= i);

                label.Add(i.Date.ToShortDateString());
                values.Add((created - closed).ToString());
            }

            chartUrl.Append("&chxl=0:|" + string.Join("|", label.Where((_, idx) => idx % 3 == 0)));
            chartUrl.Append("&chd=t:" + string.Join(",", values));

            await response.Send(chartUrl.ToString());
        }

        public static T Max<T>(T first, T second)
        {
            if (Comparer<T>.Default.Compare(first, second) > 0)
                return first;
            return second;
        }

        public static T Min<T>(T first, T second)
        {
            if (Comparer<T>.Default.Compare(first, second) < 0)
                return first;
            return second;
        }
    }
}