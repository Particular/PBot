namespace PBot.Issues
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Octokit;

    public class ListIssuesFromExternalContributors : BotCommand
    {
        public ListIssuesFromExternalContributors()
            : base("`show external issues from (*.)$", "show external issues <Period>` - Shows issue stats for the specified period, period syntax yyy-mm-dd to yyyy-MM-dd")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var period = parameters[1];

            var parts = period.Split(new[] { "to" },StringSplitOptions.RemoveEmptyEntries);

            var start = DateTimeOffset.ParseExact(parts[0].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var end = DateTimeOffset.ParseExact(parts[1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);


            var client = GitHubClientBuilder.Build();

            var request = new RepositoryIssueRequest
            {
                Since = start,
                State = ItemState.All
            };


            var organisation = "Particular";

            var members = await client.Organization.Member.GetAll(organisation);

            var repos = await client.Repository.GetAllForOrg(organisation);

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Found issues:");

            foreach (var repo in repos.Where(r => !r.Private))
            {
                var issues = await client.Issue.GetForRepository(organisation, repo.Name, request);

                foreach (var issue in issues.Where(i => i.CreatedAt >= start && i.CreatedAt < end))
                {
                    var createdByExternalUser = members.All(m => m.Login != issue.User.Login);
                    var isBug = issue.Labels.Any(l => l.Name.ToLower() == "bug");

                    if (!createdByExternalUser)
                    {
                        continue;
                    }

                    stringBuilder.AppendLine(string.Join(";", new List<string>
                                {
                                    issue.HtmlUrl.ToString(),
                                    issue.CreatedAt.ToString("d"),
                                    issue.User.Login,
                                    isBug.ToString()
                                }));

                }
            }

            await response.Send(stringBuilder.ToString());
        }
    }
}
