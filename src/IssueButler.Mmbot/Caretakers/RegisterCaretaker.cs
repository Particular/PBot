namespace IssueButler.Mmbot.Caretakers
{
    public class RegisterCaretaker : BotCommand
    {
        public RegisterCaretaker(): base(
            "register (.*) (as caretaker for) (.*)$", 
            "mmbot register <user> as caretaker for <repository> - registers the given user as the caretaker for the repo. The repo must exist. If not register is using mmbot register repository <name of repo>"){}

        public override void Execute(string[] parameters, IResponse response)
        {
            var user = parameters[1];
            var repo = parameters[2];

            new ManageCaretakers(Brain)
            .AddCaretaker(user, repo);

            response.Send(user + " is now caretaker for " + repo);
        }
    }
}