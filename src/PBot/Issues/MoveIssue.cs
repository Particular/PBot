namespace PBot.Issues
{
    using System.Threading.Tasks;

    public class MoveIssue : BotCommand
    {
        public MoveIssue()
            : base("move issue (.*) to (.*)$",
            "`pbot move issue <repository>#<issue number>(or https://github.com/Particular/repository/issues/issuenumber) to <target repository>(or https://github.com/Particular/repository)` - Moves an issue from one repository to the other. Comments aren't moved, so revising description of new issue is advised.")
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

            await response.Send($"Moving issue https://github.com/{owner}/{sourceRepo}/issues/{issueNumber}").IgnoreWaitContext();

            var newIssue = await IssueUtility.Transfer(owner, sourceRepo, issueNumber, owner, targetRepo, true).IgnoreWaitContext();

            await response.Send($"Issue moved to https://github.com/{owner}/{targetRepo}/issues/{newIssue.Number}").IgnoreWaitContext();
        }
    }
}
