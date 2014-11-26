
namespace IssueButler.Mmbot.Issues
{
    using System.Linq;
    using System.Text;

    public class CheckRepoForIssuesThatNeedsAttention : BotCommand
    {
        public CheckRepoForIssuesThatNeedsAttention() : base("check repo (.*)$", "mmbot check repo <name of repo> - Checks all issues in the specified repository and reports needed actions") { }

        public override void Execute(string[] parameters, IResponse response)
        {
            var repoName = parameters[1];
            var client = GitHubClientBuilder.Build();
            var repo = client.Repository.Get("Particular", repoName).Result;

            var validationErrors = new CheckIssuesForRepository(repo, client)
                .Execute();

            if (!validationErrors.Any())
            {
                response.Send("Nice job citizen, the repo is clean!");
                return;
            }

            response.Send(FormatErrors(repoName, validationErrors));
        }

        string FormatErrors(string repoName, ValidationErrors validationErrors)
        {
            var maxNumIssuesToShow = 10;

            var sb = new StringBuilder();

            sb.AppendLine("The following issues for " + repoName + " needs attention");


            foreach (var error in validationErrors.Take(maxNumIssuesToShow)) //nsb is to big for now
            {
                sb.AppendLine(string.Format("{0} - {1}",error.Issue.HtmlUrl, error.Reason));
            }

            if (validationErrors.Count() > maxNumIssuesToShow)
            {
                sb.AppendLine(string.Format("There are {0} more issues as well. I'll soon be able to show you them using: `pbot check repo {1} detailed`",validationErrors.Count()-maxNumIssuesToShow,repoName));
            }


            sb.AppendLine("Unsure how to go about doing this? Please read more here: https://github.com/Particular/Housekeeping/wiki/Issue-management");
            return sb.ToString();
        }
    }
}