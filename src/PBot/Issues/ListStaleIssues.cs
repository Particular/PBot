namespace PBot.Issues
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class ListStaleIssues : BotCommand
    {
        public ListStaleIssues() : base(
            "stale issues (.*)$", 
            "pbot stale issues <tag> - lists issues which are 'State: In Progress' that are stale")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var client = GitHubClientBuilder.Build();

            await response.Send("Looking for stale issues I am ...");

            var searchRequest = new SearchIssuesRequest
            {
                User = "Particular",
                Labels = new[] { parameters[1], $"\"{InProgressLabel}\"" },
                SortField = IssueSearchSort.Updated,
                State = ItemState.Open
            };
            var searchResults = await client.Search.SearchIssues(searchRequest);

            if (searchResults.TotalCount == 0)
            {
                await response.Send($"No stale issues found for tag `{parameters[1]}`.");
                return;
            }

            foreach (var issue in searchResults.Items)
            {
                var repositoryName = issue.CommentsUrl.AbsolutePath.Split('/')[3];

                var comments = await client.Issue.Comment.GetAllForIssue("Particular", repositoryName, issue.Number);
                var latestComment = comments.OrderByDescending(c => c.CreatedAt).FirstOrDefault();

                var events = await client.Issue.Events.GetAllForIssue("Particular", repositoryName, issue.Number);
                var movedToInProgress = events.Last(e => e.Label != null && e.Label.Name == InProgressLabel);

                var stalenessThreshold = DateTime.UtcNow.Subtract(StalenessPeriod);

                if (issue.CreatedAt > stalenessThreshold)
                {
                    continue;
                }

                if (latestComment != null && latestComment.CreatedAt > stalenessThreshold)
                {
                    continue;
                }

                if (movedToInProgress.CreatedAt > stalenessThreshold)
                {
                    continue;
                }

                await response.Send(issue.HtmlUrl.ToString());
            }

            await response.Send("Done.");
        }

        string InProgressLabel = "State: In Progress";
        TimeSpan StalenessPeriod = TimeSpan.FromDays(7);
    }
}