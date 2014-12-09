namespace PBot.Reminders
{
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class RemindProductRoadmapTeam : BotCommand
    {
        public RemindProductRoadmapTeam()
            : base(
                "remind product roadmap team",
                "pbot remind product roadmap team - Reminds the product roadmap team when more features needs to be approved")
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

            if (issues.Count(i => i.Labels.Any(l => l.Name == "Requirements: Approved")) < 2)
            {
                await response.Send(string.Format("@channel Hi there! I've noticed that there isn't enough approved requirements, please move more to the `approved` state. You can read more about the requirement process here: {0}",
                    @"https://github.com/Particular/Requirements/wiki/Requirements-process")).IgnoreWaitContext();

            }

            if (issues.Count(i => i.Labels.Any(l => l.Name == "Needs: Customer impact")) < 5)
            {
                await response.Send(string.Format("@channel Hi all! I've noticed that there isn't enough requirements in the `customer impact assessment` state (5 seems like a good number), let's get more high prio issues over there?. You can read more about the requirement process here: {0}",
                    @"https://github.com/Particular/Requirements/wiki/Requirements-process")).IgnoreWaitContext();

            }

        }
    }
}