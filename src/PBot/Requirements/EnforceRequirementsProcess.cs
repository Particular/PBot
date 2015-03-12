namespace PBot.Requirements
{
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

            await new RemindWhenMoreRequirementsNeedsToBeApproved(client, repo, response)
                .Perform();

        }
    }
}