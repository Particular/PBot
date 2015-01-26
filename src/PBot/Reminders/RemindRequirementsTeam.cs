namespace PBot.Reminders
{
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class RemindRequirementsTeam : BotCommand
    {
        public RemindRequirementsTeam()
            : base(
                "remind requirement team",
                "pbot remind requirement team - Reminds the requirements team when there is issues in https://github.com/Particular/Requirements that needs attention")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var client = GitHubClientBuilder.Build();

            var issueFilter = new RepositoryIssueRequest
            {
                State = ItemState.Open
            };

            var issues = await client.Issue.GetForRepository("Particular", "Requirements", issueFilter);

            var numApproved = issues.Count(i => i.Labels.Any(l => l.Name == "State: Prioritized"));

            if (numApproved < 2)
            {
                await response.Send(string.Format("@channel Hi there! I've noticed that there is only {1} approved requirements, please move more to the `Approved` swimlane! You can read more about the requirement process here: {0}",
                    @"https://github.com/Particular/Requirements/wiki/Requirements-process",numApproved)).IgnoreWaitContext();

            }
            
            var numImpactAssesement = issues.Count(i => i.Labels.Any(l => l.Name == "Needs: Impact Assessment"));

            if (numImpactAssesement < 5)
            {
                await response.Send(string.Format("@channel Hi all! I've noticed that there is only {1} in the `Define: Customer Impact & Importance` swimlane, let's get more high prio requirements over there?. You can read more about the requirement process here: {0}",
                    @"https://github.com/Particular/Requirements/wiki/Requirements-process",numImpactAssesement)).IgnoreWaitContext();

            }

        }
    }
}