// [TODO] Currently this crashes pbot

//namespace IssueButler.Mmbot.Repositories
//{
//    using System.Linq;
//    using System.Threading.Tasks;
//    using PBot;

//    public class ListRepositories : BotCommand
//    {
//        public ListRepositories()
//            : base("list repo(s?)$",
//                "pbot list repos - List all the repositories")
//        {
//        }

//        public override async Task Execute(string[] parameters, IResponse response)
//        {
//            var allRepos = Brain.Get<AvailableRepositories>() ?? new AvailableRepositories();

//            if (allRepos.Any())
//            {
//                await response.Send("Here is all the repositories I know about.").IgnoreWaitContext();
//            }

//            foreach (var repo in allRepos.Select(r => r.Name).OrderBy(n => n))
//            {
//                await response.Send(repo).IgnoreWaitContext();
//            }
//        }
//    }
//}