
namespace IssueButler.Mmbot.Issues
{
    using System.Linq;
    using System.Text;

    public class CheckRepo : BotCommand
    {
        public CheckRepo() : base("check repo (.*)$", "mmbot check repo <name of repo> - Checks all issues in the specified repository and reports needed actions") { }

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
            var maxNumIssuesToShow = 30;

            var sb = new StringBuilder();

            sb.AppendLine("The following issues for " + repoName + " needs attention <br/>");

            if (validationErrors.Count() > maxNumIssuesToShow)
            {
                sb.AppendLine(string.Format("Showing top {0} our of {1} invalid issues <br/>", maxNumIssuesToShow, validationErrors.Count()));
            }


            foreach (var error in validationErrors.Take(maxNumIssuesToShow)) //nsb is to big for now
            {
                sb.AppendLine(string.Format("<a href=\"{0}\">{1} - {2}<a/><br/>", error.Issue.HtmlUrl, error.Issue.Number, error.Reason));
            }


            return sb.ToString();
        }
    }
}