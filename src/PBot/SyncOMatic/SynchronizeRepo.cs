namespace PBot.SyncOMatic
{
    using System;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using global::SyncOMatic;

    public class SynchronizeRepo : BotCommand
    {
        public SynchronizeRepo()
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
                
                ExceptionDispatchInfo capturedException = null;
                try
                {
                    await diff;
                    var sync = som.Sync(diff.Result, SyncOutput.CreatePullRequest, new[]
                    {
                        "Internal refactoring"
                    });
                    await sync;
                    
                    var createdSyncBranch = sync.Result.FirstOrDefault();

                    if (string.IsNullOrEmpty(createdSyncBranch))
                    {
                        await response.Send(string.Format("Repo {0} is already in sync, nothing for me to do!", repoName));
                    }
                    else
                    {
                        await response.Send(string.Format("Pull created for {0}, click here to review and pull: {1}", repoName, createdSyncBranch));
                    }
                }
                catch (AggregateException aggEx)
                {
                    var ex = aggEx.Flatten().InnerException;

                    if (ex is Octokit.NotFoundException)
                    {
                        // The github api is weird. NotFound is actually a
                        // permission error.
                        capturedException = ExceptionDispatchInfo.Capture(ex);
                    }
                }
                catch (Octokit.NotFoundException ex)
                {
                    // The github api is weird. NotFound is actually a
                    // permission error.
                    capturedException = ExceptionDispatchInfo.Capture(ex);
                }
                if (capturedException != null)
                {
                    await response.Send(string.Format("I do not have commit access to repo {0}. Please add 'particularbot' to the list of contributers.", repoName));

                    //capturedException.Throw();
                }

                await response.Send(string.Format("Want to know more about repo syncing? Go here: {0}", @"https://github.com/Particular/Housekeeping/wiki/Repository%20synchronisation"));
            }
        }
    }
}