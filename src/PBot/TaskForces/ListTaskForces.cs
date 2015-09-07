namespace PBot.TaskForces
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using PBot.Users;

    public class ListTaskForces : BotCommand
    {
        public ListTaskForces()
            : base(
                "pig (.*)$",
                "pbot pig <me:username> - Show issues that the nominated user is a PIG for") { }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var username = parameters[1];
            var self = false;
            if (username.ToLower() == "me")
            {
                username = response.User.Name;
                self = true;
            }

            UserCredentials userCredentials;
            if (Brain.Get<CredentialStore>().TryGetValue(username, out userCredentials))
            {
                username = userCredentials
                    .Credentials
                    .ToLookup(x => x.Name, x => x.Value)["github-username"]
                    .FirstOrDefault() ?? username;
            }

            username = username.TrimStart('@');

            await response.Send(string.Format("### {0} as Pig", username)).IgnoreWaitContext();

            var stopwatch = Stopwatch.StartNew();
            var client = GitHubClientBuilder.Build();
            var query = new InvolvedIssueQuery(client);
            var results = ((await query.Perform(username))
                .OrderBy(issue => issue.Repo.Name)
                .ThenBy(issue => issue.Issue.CreatedAt)).ToList();

            if (!results.Any())
            {
                await response.Send("No results found");

                if (self)
                {
                    await response.Send(string.Format("If `{0}` is not your github username then use the following command to tell me what it is:", username));
                }
                else
                {
                    await response.Send(string.Format("`{0}` may not be a valid github username. Tell the owner of this account to use the following command to tell me their github username:", username));
                }

                await response.Send("`pbot register credential github-username=<github username>`");
                await response.Send(string.Format("Alternatively, `{0}` may not be on any task forces. See https://github.com/Particular/Strategy/blob/master/definitions/taskforces.md", username));
            }

            await response.Send(results.SelectMany(GetMessages).ToArray());
            await response.Send(string.Format(
                    "_{0:N0} issues/PRs found in {1:N2} seconds. To view all issues/PRs which mention `{2}` go to https://github.com/issues?q=is%3Aopen+mentions%3A{0}+user%3AParticular ._",
                    results.Count,
                    stopwatch.Elapsed.TotalSeconds,
                    username));
        }

        private static IEnumerable<string> GetMessages(InvolvedIssue issue)
        {
            yield return string.Format("*{0}*", issue.Issue.Title);

            var items = new List<string> { issue.Issue.HtmlUrl.ToString(), };
            yield return string.Join(
                " ", items.Concat(issue.Issue.Labels.Select(x => string.Format("`{0}`", x.Name.Trim()))));
        }
    }
}
