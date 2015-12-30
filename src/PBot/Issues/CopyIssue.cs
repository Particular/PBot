namespace PBot.Issues
{
    using System.Threading.Tasks;

    public class CopyIssue : BotCommand
    {
        public CopyIssue()
            : base("copy issue (.*) to (.*)$",
            "`pbot copy issue <repository>#<issue number>(or https://github.com/Particular/repository/issues/issuenumber) to <target repository>(or https://github.com/Particular/repository)` -  Copies an issue from one repository to the other.")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            string sourceRepo;
            string issueNumberString;
            if (!RepoParser.ParseRepoAndIssueNumber(parameters[1], out sourceRepo, out issueNumberString))
            {
                await response.Send($"I could not parse the source repository and issue number from '{parameters[1]}'. Are you using the correct syntax?");
                return;
            }

            string targetRepo;
            if (!RepoParser.ParseRepo(parameters[2], out targetRepo))
            {
                await response.Send($"I could not parse the target repository from '{parameters[2]}'. Are you using the correct syntax?");
                return;
            }

            int issueNumber;
            if (!int.TryParse(issueNumberString, out issueNumber))
            {
                await response.Send("Issue number should be a valid number dude!");
                return;
            }

            var owner = "Particular";

            await response.Send($"Copying issue https://github.com/{owner}/{sourceRepo}/issues/{issueNumber}").IgnoreWaitContext();

            var newIssue = await IssueUtility.Transfer(owner, sourceRepo, issueNumber, owner, targetRepo, false).IgnoreWaitContext();

            await response.Send($"Issue copied to https://github.com/{owner}/{targetRepo}/issues/{newIssue.Number}").IgnoreWaitContext();
        }
    }
}
