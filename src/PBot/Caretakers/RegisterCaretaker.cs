namespace PBot.Caretakers
{
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;

    public class RegisterCaretaker : BotCommand
    {
        public RegisterCaretaker()
            : base(
                "register (.*) (as caretaker for) (.*)$",
                "`register <username:my self> as caretaker for <repository>` - registers the given user/your self as the caretaker for the repo. The repo must exist. If not register is using mmbot register repository <name of repo>") { }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var username = parameters[1];
            var repoName = parameters[3];

            var activeRepositories = Brain.Get<AvailableRepositories>();

            var repo = activeRepositories.SingleOrDefault(r => r.Name == repoName);

            if (repo == null)
            {
                await response.Send("Repository not found, please add it using: mmbot add repo " + repoName).IgnoreWaitContext();
                return;
            }

            if (username.ToLower() == "my self")
            {
                repo.Caretaker = CurrentUser.Name;
            }
            else
            {
                repo.Caretaker = username;
            }

            Brain.Set(activeRepositories);

            await response.Send(username + " is now caretaker for " + repo.Name);
        }
    }
}
