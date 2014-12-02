namespace PBot.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;

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


            var toRemove = allRepos.SingleOrDefault(r => r.Name == repoName);

            if (toRemove == null)
            {
                await response.Send(repoName + " doesn't exists").IgnoreWaitContext();
                return;
            }

            allRepos.Remove(toRemove);

            await response.Send(repoName + " is now removed").IgnoreWaitContext();

            Brain.Set(allRepos);
        }

    }
}