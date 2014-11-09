namespace IssueButler.Mmbot.Caretakers
{
    using System.Collections.Generic;
    using MMBot;
    using MMBot.Scripts;

    public class ManageCaretakerCommands : IMMBotScript
    {
        public void Register(Robot robot)
        {
            robot.Respond(@"register (.*) (as caretaker for) (.*)$", msg =>
            {
                var user = msg.Match[1];
                var repo = msg.Match[2];

                new ManageCaretakers(robot.Brain)
                .AddCaretaker(user,repo);

                msg.Send(user+" is now caretaker for " + repo);
            });
        }

        public IEnumerable<string> GetHelp()
        {

            return new[]
            {
                "mmbot register <user> as caretaker for <repository> - registers the given user as the caretaker for the repo. The repo must exist. If not register is using mmbot register repository <name of repo>"
            };
        }
    }
}