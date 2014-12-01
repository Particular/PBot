namespace IssueButler.Mmbot.Caretakers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;
    using Octokit;

    public class RemoveRepository : BotCommand
    {
        public RemoveRepository()
            : base("remove repo (.*)$",
                "pbot remove repo <name of repo> -  Removes the given repository")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var repoName = parameters[1];

            var allRepos = Brain.Get<AvailableRepositories>() ?? new AvailableRepositories();

            Repository repo;

            if (!TryFetchRepoDetails(repoName, out repo))
            {
                await response.Send(repoName + " doesn't exist").IgnoreWaitContext();
                return;
            }

            var toRemove = allRepos.SingleOrDefault(r => r.Name == repo.Name);

            if (toRemove == null)
            {
                await response.Send(repo.Name + " doesn't exists").IgnoreWaitContext();
                return;
            }

            allRepos.Remove(toRemove);

            await response.Send(repo.Name + " is now removed").IgnoreWaitContext();

            Brain.Set(allRepos);
        }

        static bool TryFetchRepoDetails(string repoName, out Repository result)
        {
            try
            {
                result = GitHubClientBuilder.Build().Repository.Get("Particular", repoName).Result;

                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }
    }
}