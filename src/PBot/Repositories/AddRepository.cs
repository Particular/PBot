namespace PBot.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;
    using Octokit;

    public class AddRepository : BotCommand
    {
        public AddRepository()
            : base("add repo (.*)$",
                "pbot add repo <name of repo> -  Adds the given repository to the list of active repos. Repo must exist under the configured organization. Wildcard is supported")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var repoName = parameters[1];

            var allRepos = Brain.Get<AvailableRepositories>() ?? new AvailableRepositories();

            if (repoName.EndsWith("*"))
            {
                await response.Send("Oh, wildcard add, you have guts!").IgnoreWaitContext();

                var reposForOrg = GitHubClientBuilder.Build().Repository.GetAllForOrg("Particular").Result;

                var repoNameWithoutStar = repoName.Replace("*", "");

                foreach (var matchingRepo in reposForOrg.Where(r => r.Name.StartsWith(repoNameWithoutStar)))
                {
                    if (allRepos.Any(r => r.Name == matchingRepo.Name))
                    {
                        continue;
                    }

                    allRepos.Add(new AvailableRepositories.Repository
                    {
                        Name = matchingRepo.Name
                    });
                    await response.Send(matchingRepo.Name + " is now added!").IgnoreWaitContext();
                }
            }
            else
            {
                Repository repo;

                if (!TryFetchRepoDetails(repoName, out repo))
                {
                    await response.Send(repoName + " doesn't exist").IgnoreWaitContext();
                    return;
                }

                if (allRepos.Any(r => r.Name == repo.Name))
                {
                    await response.Send(repo.Name + " already exists").IgnoreWaitContext();
                    return;
                }

                allRepos.Add(new AvailableRepositories.Repository
                {
                    Name = repo.Name
                });
                await response.Send(repo.Name + " is now added!").IgnoreWaitContext();
            }

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