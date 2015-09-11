namespace PBot.Buildserver
{
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;

    public class NotifyCaretakersOfCurrentlyFailedBuilds : BotCommand
    {
        public NotifyCaretakersOfCurrentlyFailedBuilds()
            : base(
                "notify caretakers of failed builds$",
                "`notify caretakers of failed builds` - Send a private message to each caretaker if there are currently failed builds")
        {
            client = new TeamCity("http://builds.particular.net");
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var reposGroupedByCaretaker = Brain.Get<AvailableRepositories>()
                .Where(r => r.Caretaker != null)
                .GroupBy(r => r.Caretaker)
                .ToList();

            var allBuilds = client.ListProjects()
                .Select(p=>p.Name)
                .ToList();

            foreach (var repoGroup in reposGroupedByCaretaker)
            {
                var reposWithNoBuild = repoGroup.Where(r => !allBuilds.Contains(r.Name)).ToList();

                if (reposWithNoBuild.Any())
                {
                    //Until we have a better way excluding repos that doesn't need a build
                    //await response.Send(string.Format("Hi there @{0}! It seems that there is no build setup for {1} Just type `help create` if you want to help setting it up?", repoGroup.Key, string.Join(", ", reposWithNoBuild))).IgnoreWaitContext();
                }

                var reposWithABuild = repoGroup.Where(r => allBuilds.Contains(r.Name)).ToList();


                var reposWithFailedBuilds = reposWithABuild.Where(r => HasFailedBuilds(r.Name))
                    .ToList();

                if (reposWithFailedBuilds.Any())
                {
                    await response.Send(string.Format("Hi there @{0}! It seems that the build is broken for {1} and since you're the caretaker I thought you would like to be aware. Just type `list failing builds for my repos` to get the details!", repoGroup.Key, string.Join(", ", reposWithFailedBuilds))).IgnoreWaitContext();
                }
            }
        }

        bool HasFailedBuilds(string name)
        {

            var tcProj = client.GetProject(name);

            return client.ListCurrentBuilds(tcProj.Id, new[]
            {
                "master",
                "develop"
            }).Any(b => b.Status == BuildStatus.Failed);


        }

        TeamCity client;
    }
}
