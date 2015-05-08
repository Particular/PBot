namespace PBot.Issues
{
    using System.Linq;
    using System.Threading.Tasks;

    public class MoveIssue : BotCommand
    {
        public MoveIssue()
            : base("move issue (.*) to (.*)$",
            "pbot move issue <repository>#<issue number>(or https://github.com/Particular/repository/issues/issuenumber) to <target repository>(or https://github.com/Particular/repository) - Moves an issue from one repository to the other.")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            string sourceRepo;
            string issueNumberString;
            if (!RepoParser.ParseRepoAndIssueNumber(parameters[1], out sourceRepo, out issueNumberString))
            {
                await response.Send(string.Format("I could not parse the source repository and issue number from '{0}'. Are you using the correct syntax?", parameters[1]));
                return;
            }

            string targetRepo;
            if (!RepoParser.ParseRepo(parameters[2], out targetRepo))
            {
                await response.Send(string.Format("I could not parse the target repository from '{0}'. Are you using the correct syntax?", parameters[2]));
                return;
            }
            
            int issueNumber;
            if (!int.TryParse(issueNumberString, out issueNumber))
            {
                await response.Send("Issue number should be a valid number dude!");
                return;
            }

            var src = new RepoInfo { Owner = "Particular", Name = sourceRepo };
            var dst = new RepoInfo { Owner = "Particular", Name = targetRepo };

            await response.Send(string.Format("Moving issue https://github.com/Particular/{0}/issues/{1}", sourceRepo, issueNumber)).IgnoreWaitContext();

            var client = GitHubClientBuilder.Build();
            var labels = await client.Issue.Labels.GetForIssue(src.Owner, src.Name, issueNumber);
            var labelsExist = await LabelUtility.RepositoryHasLabels(dst, labels.ToArray());
            if (!labelsExist)
            {
                await response.Send(string.Format("Could not move issue https://github.com/Particular/{0}/issues/{1}. Destination repo {2} doesn't have issue labels. Please create labels in target repo first.", sourceRepo, issueNumber, targetRepo)).IgnoreWaitContext();
            }

            var newIssue = await IssueUtility.Transfer(src, issueNumber, dst, true).IgnoreWaitContext();

            await response.Send(string.Format("Issue moved to https://github.com/Particular/{0}/issues/{1}", targetRepo, newIssue.Number)).IgnoreWaitContext();
        }
    }
}