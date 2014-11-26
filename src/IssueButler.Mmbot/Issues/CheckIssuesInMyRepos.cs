namespace IssueButler.Mmbot.Issues
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using IssueButler.Mmbot.Repositories;

    public class CheckIssuesInMyRepos : BotCommand
    {
        public CheckIssuesInMyRepos() : base("check my repos", "List the issue that needs attention for repos that you are the caretaker for")
        {

          
        }

        public override void Execute(string[] parameters, IResponse response)
        {
            var username = "andreas";//until we can find a way to get this

            var repoNames = Brain.Get<AvailableRepositories>()
                .Where(r => r.Caretaker == username)
                .Select(r=>r.Name)
                .ToList();

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
                response.Send(string.Format("Nice job citizen, your repos are squeaky clean!! (Checked {0})",string.Join(", ",repoNames)));
                return;
            }

            response.Send(FormatErrors(validationErrors));
        }

        string FormatErrors(List<ValidationError> validationErrors)
        {
            var maxNumIssuesToShow = 20;

            var sb = new StringBuilder();

            sb.AppendLine("The following issues needs attention");


            foreach (var error in validationErrors.Take(maxNumIssuesToShow)) //nsb is to big for now
            {
                sb.AppendLine(string.Format("{0} - {1}", error.Issue.HtmlUrl, error.Reason));
            }

            if (validationErrors.Count() > maxNumIssuesToShow)
            {
                sb.AppendLine(string.Format("There are {0} more issues as well.", validationErrors.Count() - maxNumIssuesToShow));
            }


            sb.AppendLine("Unsure how to go about doing this? Please read more here: https://github.com/Particular/Housekeeping/wiki/Issue-management");
            return sb.ToString();
        }
    }
}