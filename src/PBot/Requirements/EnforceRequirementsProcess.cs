namespace PBot.Requirements
{
    using System;
    using System.Threading.Tasks;

    public class EnforceRequirementsProcess : BotCommand
    {
        public EnforceRequirementsProcess()
            : base(
                "enforce requirements process",
                "pbot enforce requirements process - Makes sure that the requirements process is followed")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var client = GitHubClientBuilder.Build();

            var repo = await client.Repository.Get("Particular", "Requirements");

            await new MoveConcernsToIAStateAutomatically(client, repo)
                .Perform();

            await new MoveStaleIARequirementsToBacklog(client, repo, TimeSpan.FromDays(14))
                .Perform();
            await new RemindWhenMoreRequirementsNeedsToBeApproved(client, repo, response)
                .Perform();

            await new RemindWhenMoreRequirementsNeedsToBeReviewed(client, repo, response)
                .Perform();

            await new MoveStaleInProgressRequirementsToApproved(client, repo, TimeSpan.FromDays(7))
                .Perform();

            await new MoveStaleRequirementIdeasToBacklog(client, repo, TimeSpan.FromDays(14))
                .Perform();

            
        }
    }
}