namespace IssueButler.Mmbot.Caretakers
{
    using System;
    using System.Linq;
    using IssueButler.Mmbot.Repositories;
    using Octokit;

    public class AddRepository : BotCommand
    {
        public AddRepository(): base("add repo (.*)$", 
            "mmbot add repo <name of repo> -  Adds the given repository to the list of active repos. Repo must exist under the configured organization")
        {
        }

        public override void Execute(string[] parameters, IResponse response)
        {
            var repoName = parameters[1];

            Repository repo;

            if (!TryFetchRepoDetails(repoName, out repo))
            {
                response.Send(repoName + " doesn't exist");
                return;
                
            }

            var allRepos = Brain.Get<AvailableRepositories>() ?? new AvailableRepositories();

            if (allRepos.Any(r => r.Name == repo.Name))
            {
                response.Send(repo.Name + " already exists");
                return;
            }

            allRepos.Add(new AvailableRepositories.Repository
            {
                Name = repo.Name
            });

            Brain.Set(allRepos);

            response.Send(repo.Name + " is now added!");
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