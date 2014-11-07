using System.Linq;
using Octokit;

namespace IssueButler.Mmbot
{
    using System.Collections.Generic;
    using MMBot;
    using MMBot.Scripts;

    public class CheckIssues:IMMBotScript
    {
        public void Register(Robot robot)
        {
            robot.Respond(@"check my repo$", msg => msg.Send("Check my repo"));
            robot.Respond(@"check repo (.*)$", msg =>
            {
                var validationErrors = new CheckIssuesForRepository(msg.Match[1], GitHubClientBuilder.Build())
                    .Execute();

                var message = string.Join(";", validationErrors.Select(v => v.Reason));

                msg.Send(message);
            });
        }

        public IEnumerable<string> GetHelp()
        {
            yield return "Checks a repositories for issue in need of some TLC";
        }
    }

    public class CheckIssuesForRepository
    {
        public CheckIssuesForRepository(string name
            , GitHubClient client)
        {
        }

        public IEnumerable<ValidationError> Execute()
        {
            return new List<ValidationError> {new ValidationError {Reason = "Test reason"}};
        }
    }
}