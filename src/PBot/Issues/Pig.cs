namespace PBot.Issues
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Humanizer;
    using PBot.Users;

    public class Pig : BotCommand
    {
        public Pig()
            : base(
                "pig (.*)$",
                "`pbot pig <me:username>` - Show issues that the nominated user is a PIG for") { }

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

            await response.Send($"### {username} as Pig").IgnoreWaitContext();

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
                    await response.Send($"If `{username}` is not your github username then use the following command to tell me what it is:");
                }
                else
                {
                    await response.Send($"`{username}` may not be a valid github username. Tell the owner of this account to use the following command to tell me their github username:");
                }

                await response.Send("`pbot register credential github-username=<github username>`");
                await response.Send($"Alternatively, `{username}` may not be on any task forces. See https://github.com/Particular/Strategy/blob/master/definitions/taskforces.md");
            }

            await response.Send(results.SelectMany(GetMessages).ToArray());
            await response.Send($"_{results.Count:N0} issues/PRs found in {stopwatch.Elapsed.Humanize()}. All issues/PRs which mention `{username}`: https://github.com/issues?q=is%3Aopen+mentions%3A{2}+user%3AParticular ._");
        }

        private static IEnumerable<string> GetMessages(InvolvedIssue issue)
        {
            yield return $"*{issue.Issue.Title}*";

            var items = new List<string> { issue.Issue.HtmlUrl.ToString(), };
            yield return string.Join(
                " ", items.Concat(issue.Issue.Labels.Select(x => $"`{x.Name.Trim()}`")));
        }
    }
}
