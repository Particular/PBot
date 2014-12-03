namespace PBot.SyncOMatic
{
    using System.Linq;
    using System.Threading.Tasks;
    using global::SyncOMatic;

    public class SyncronizeRepo : BotCommand
    {
        public SyncronizeRepo()
            : base(
                "sync (.*) (target branch) (.*)$",
                "pbot sync <name of repo> target branch <name of target branch> - Performs a syncomatic run against the given repo and branch")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {

            var branchName = parameters[3];



            var ghRepo = await GitHubClientBuilder.Build().Repository.Get("Particular", parameters[1]);

            //we need the exact name since the sync is case sensitive for eg. the .dotSettings file
            var repoName = ghRepo.Name;

            await response.Send(string.Format("Got it! Initiating a sync for {0}/{1}", repoName, branchName));


            using (var som = new Syncer(GitHubHelper.Credentials, GitHubHelper.Proxy, logEntry =>
            {
                //no-op logging for low, until we figure out log channels
            }))
            {

                var toSync = new RepoToSync
                {
                    Name = repoName,
                    Branch = branchName,
                    SolutionName = repoName,
                    SrcRoot = "src"
                };

                var diff = som.Diff(toSync.GetMapper(DefaultTemplateRepo.ItemsToSync));

                var createdSyncBranch = som.Sync(diff, SyncOutput.CreatePullRequest, new[]
                {
                    "Internal refactoring"
                }).FirstOrDefault();


                if (string.IsNullOrEmpty(createdSyncBranch))
                {
                    await response.Send(string.Format("Repo {0} is already in sync, nothing for me to do!", repoName));
                }
                else
                {
                    await response.Send(string.Format("Pull created for {0}, click here to review and pull: {1}", repoName, createdSyncBranch));
                }
                await response.Send(string.Format("Want to know more about repo syncing? Go here: {0}", @"https://github.com/Particular/Housekeeping/wiki/Repository%20synchronisation"));
            }
        }
    }
}