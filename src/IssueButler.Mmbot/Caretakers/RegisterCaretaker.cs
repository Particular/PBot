namespace IssueButler.Mmbot.Caretakers
{
    using MMBot;

    public class RegisterCaretaker : BotCommand
    {
        public RegisterCaretaker()
        {
            Command = @"register (.*) (as caretaker for) (.*)$";

            HelpText = "mmbot register <user> as caretaker for <repository> - registers the given user as the caretaker for the repo. The repo must exist. If not register is using mmbot register repository <name of repo>";
        }

        public override void Execute(IResponse<TextMessage> reponse)
        {
            var user = reponse.Match[1];
            var repo = reponse.Match[2];

            new ManageCaretakers(Robot.Brain)
            .AddCaretaker(user, repo);

            reponse.Send(user + " is now caretaker for " + repo);
        }
    }
}