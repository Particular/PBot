﻿namespace PBot.Issues
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CheckRepoForIssuesThatNeedsAttention : BotCommand
    {
        public CheckRepoForIssuesThatNeedsAttention()
            : base("check repo (.*)$", "`mmbot check repo <name of repo>` - Checks all issues in the specified repository and reports needed actions")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var repoName = parameters[1];
            var client = GitHubClientBuilder.Build();
            var repo = await client.Repository.Get("Particular", repoName);

            var validationErrors = new CheckIssuesForRepository(repo, client)
                .Execute();

            if (!validationErrors.Any())
            {
                await response.Send("Nice job citizen, the repo is clean!").IgnoreWaitContext();
                return;
            }

            await response.Send(FormatErrors(repoName, validationErrors)).IgnoreWaitContext();
        }

        string FormatErrors(string repoName, ValidationErrors validationErrors)
        {
            var maxNumIssuesToShow = 10;

            var sb = new StringBuilder();

            sb.AppendLine("The following issues for " + repoName + " needs attention");

            foreach (var error in validationErrors.Take(maxNumIssuesToShow)) //nsb is to big for now
            {
                sb.AppendLine($"{error.Issue.HtmlUrl} - {error.Reason}");
            }

            if (validationErrors.Count() > maxNumIssuesToShow)
            {
                sb.AppendLine($"There are {validationErrors.Count() - maxNumIssuesToShow} more issues as well. I'll soon be able to show you them using: `pbot check repo {repoName} detailed`");
            }

            sb.AppendLine("Unsure how to go about doing this? Please read more here: https://github.com/Particular/Housekeeping/wiki/Issue-management");
            return sb.ToString();
        }
    }
}
