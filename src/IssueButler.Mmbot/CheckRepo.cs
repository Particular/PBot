namespace IssueButler.Mmbot
{
    using System.Collections.Generic;
    using MMBot;
    using MMBot.Scripts;

    public class CheckRepo:IMMBotScript
    {
        public void Register(Robot robot)
        {
            robot.Respond(@"check$", msg => msg.Send("Hello from IssueButler"));
        }

        public IEnumerable<string> GetHelp()
        {
            yield return "Checks a repo for issue in needs of some TLC";
        }
    }
}