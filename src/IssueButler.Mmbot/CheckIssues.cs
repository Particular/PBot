
namespace IssueButler.Mmbot
{
    using System.Collections.Generic;
    using System.Text;
    using MMBot;
    using MMBot.Scripts;
    using System.Linq;

    public class CheckIssues : IMMBotScript
    {
        public void Register(Robot robot)
        {
            robot.Respond(@"check my repo$", msg => msg.Send("Check my repo"));
            robot.Respond(@"check repo (.*)$", msg =>
            {
                var repoName = msg.Match[1];
                var client = GitHubClientBuilder.Build();
                var repo = client.Repository.Get("Particular", repoName).Result;

                var validationErrors = new CheckIssuesForRepository(repo, client)
                    .Execute();

                if (!validationErrors.Any())
                {
                    msg.Send("Nice job citizen, the repo is clean!");
                    return;
                }
         
                msg.Send(FormatErrors(repoName,validationErrors));
            });
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



        public IEnumerable<string> GetHelp()
        {
            return new List<string>
            {
                "mmbot check repo <name of repo> - Checks all issues in the specified repository and reports needed actions",
                "mmbot check my repo - Checks all issues in repositories you're cartaking and reports needed actions"
            };
        }
    }
}