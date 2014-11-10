namespace IssueButler.Mmbot
{
    using System;
    using System.Collections.Generic;
    using MMBot;
    using MMBot.Scripts;

    public abstract class BotCommand:IMMBotScript
    {
        
        public abstract void Execute(IResponse<TextMessage> reponse);

        protected string Command;
        protected string HelpText;
        protected Robot Robot;


        public void Register(Robot robot)
        {
            if (string.IsNullOrEmpty(Command))
            {
                throw new Exception("You need to specify a regex for this command");
            }

            Robot = robot;

            robot.Respond(Command,Execute);
        }

        public IEnumerable<string> GetHelp()
        {
            if (string.IsNullOrEmpty(HelpText))
            {
                throw new Exception("Help text is required so that people can figure out how to use this command!");
            }

            return new[]{HelpText};
        }
    }
}