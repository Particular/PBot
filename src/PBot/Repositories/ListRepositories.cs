namespace IssueButler.Mmbot.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using PBot;

    public class ListRepositories : BotCommand
    {
        public ListRepositories()
            : base("list repo(s?)$",
                "pbot list repos - List all the repositories")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var allRepos = Brain.Get<AvailableRepositories>() ?? new AvailableRepositories();

            foreach (var repo in allRepos.Select(r => r.Name).OrderBy(n => n))
            {
                await response.Send(string.Format("<https://github.com/Particular/{0}|{0}>", repo)).IgnoreWaitContext();
            }
        }
    }
}