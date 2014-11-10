namespace IssueButler.Mmbot.Caretakers
{
    using System.Linq;
    using IssueButler.Mmbot.Repositories;

    public class AddRepository : BotCommand
    {
        public AddRepository(): base("add repo (.*)$", 
            "mmbot add repo <name of repo> -  Adds the given repository to the list of active repos. Repo must exist under the configured organization")
        {
        }

        public override void Execute(string[] parameters, IResponse response)
        {
            var repoName = parameters[1];

            var repo = GitHubClientBuilder.Build().Repository.Get("Particular", repoName).Result;

            var allRepos = Brain.Get<AvailableRepositories>();

            if (allRepos.Any(r => r.Name == repo.Name))
            {
                response.Send(repo.Name + " already exists");
                return;
            }

            allRepos.Add(new AvailableRepositories.Repository
            {
                Name = repo.Name
            });

            response.Send(repo + " is now added!");
        }
    }
}