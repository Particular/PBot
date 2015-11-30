namespace PBot.Issues
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;

    public class CheckIssuesInMyRepos : BotCommand
    {
        public CheckIssuesInMyRepos()
            : base("check my repos$", "`pbot check my repos` - List the issue that needs attention for repos that you are the caretaker for")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var username = response.User.Name;

            var repoNames = Brain.Get<AvailableRepositories>()
                .Where(r => r.Caretaker == username)
                .Select(r => r.Name)
                .ToList();

            if (!repoNames.Any())
            {
                await response.Send($"Hey {username} I couldn't find any repos where you're the caretaker? Please type `pbot list caretakers` for instructions on how to join the fun!").IgnoreWaitContext();
                return;
            }

            var client = GitHubClientBuilder.Build();

            var validationErrors = repoNames.SelectMany(repoName =>
            {
                var repo = client.Repository.Get("Particular", repoName).Result;

                return new CheckIssuesForRepository(repo, client)
                    .Execute();
            })
                .ToList();

            if (!validationErrors.Any())
            {
                await response.Send($"Nice job citizen, your repos are squeaky clean!! (Checked {string.Join(", ", repoNames)})").IgnoreWaitContext();
                return;
            }

            await response.Send(FormatErrors(validationErrors)).IgnoreWaitContext();
        }

        string FormatErrors(List<ValidationError> validationErrors)
        {
            var maxNumIssuesToShow = 20;

            var sb = new StringBuilder();

            sb.AppendLine("The following issues needs attention");

            foreach (var error in validationErrors.Take(maxNumIssuesToShow)) //nsb is to big for now
            {
                sb.AppendLine($"{error.Issue.HtmlUrl} - {error.Reason}");
            }

            if (validationErrors.Count() > maxNumIssuesToShow)
            {
                sb.AppendLine($"There are {validationErrors.Count() - maxNumIssuesToShow} more issues as well.");
            }

            sb.AppendLine("Unsure how to go about doing this? Please read more here: https://github.com/Particular/Housekeeping/wiki/Issue-management");
            return sb.ToString();
        }
    }
}
