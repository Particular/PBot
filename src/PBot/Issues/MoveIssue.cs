﻿namespace PBot.Issues
{
    using System.Linq;
    using System.Threading.Tasks;

    public class MoveIssue : BotCommand
    {
        public MoveIssue()
            : base("move issue (.*)#(.*) to (.*)$",
            "pbot move issue <repository>#<issue number> to <target repository> - Moves an issue from one repository to the other.")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            if (parameters.Length < 4)
            {
                await response.Send(string.Format("I don't understand '{0}'.", string.Join(" ", parameters.Select(x => "[" + x + "]"))));
                return;
            }

            var repo = parameters[1];
            var issueNumberString = parameters[2];
            var targetRepo = parameters[3];

            int issueNumber;
            if (!int.TryParse(issueNumberString, out issueNumber))
            {
                await response.Send("Issue number should be a valid number dude!");
                return;
            }

            var src = new RepoInfo { Owner = "Particular", Name = repo };
            var dst = new RepoInfo { Owner = "Particular", Name = targetRepo };

            await response.Send(string.Format("Moving issue https://github.com/Particular/{0}/issues/{1}", repo, issueNumber)).IgnoreWaitContext();

            var newIssue = await IssueUtility.Transfer(src, issueNumber, dst, true).IgnoreWaitContext();

            await response.Send(string.Format("Issue moved to https://github.com/Particular/{0}/issues/{1}", targetRepo, newIssue.Number)).IgnoreWaitContext();
        }
    }
}