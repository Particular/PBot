namespace IssueButler.Mmbot.Caretakers
{
    using System.Linq;
    using IssueButler.Mmbot.Repositories;

    public class RegisterCaretaker : BotCommand
    {
        public RegisterCaretaker(): base(
            "register (.*) (as caretaker for) (.*)$", 
            "mmbot register <user> as caretaker for <repository> - registers the given user as the caretaker for the repo. The repo must exist. If not register is using mmbot register repository <name of repo>"){}

        public override void Execute(string[] parameters, IResponse response)
        {
            var username = parameters[1];
            var repoName = parameters[3];

            var activeRepositories = Brain.Get<AvailableRepositories>();

            var repo = activeRepositories.SingleOrDefault(r => r.Name == repoName);

            if (repo == null)
            {
                response.Send("Repository not found, please add it using: mmbot add repo " + repoName);
                return;
            }

            repo.Caretaker = username;

            Brain.Set(activeRepositories);

            response.Send(username + " is now caretaker for " + repo.Name);
        }
    }

    public class ListCaretakers : BotCommand
    {
        public ListCaretakers()
            : base(
                "list caretakers$",
                "mmbot list caretakers - Displays the list of caretakers") { }

        public override void Execute(string[] parameters, IResponse response)
        {
            var activeRepositories = Brain.Get<AvailableRepositories>();

            foreach (var repo in activeRepositories.Where(r=>r.Caretaker != null))
            {
                response.Send(string.Format("{0} is caretaker for {1}",repo.Caretaker,repo.Name));
            }

            response.Send(string.Format("Repos that is up for grabs: {0}", string.Join(", ",activeRepositories.Where(r=>r.Caretaker == null).Select(r=>r.Name))));
            
        }
    }
}