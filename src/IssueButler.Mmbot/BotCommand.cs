namespace IssueButler.Mmbot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MMBot;
    using MMBot.Brains;
    using MMBot.Scripts;

    public abstract class BotCommand : IMMBotScript
    {
        public BotCommand(string command, string helpText)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new Exception("You need to specify a regex for this command");
            }

            if (string.IsNullOrEmpty(helpText))
            {
                throw new Exception("Help text is required so that people can figure out how to use this command!");
            }

            this.command = command;
            this.helpText = helpText;
        }

        public abstract Task Execute(string[] parameters, IResponse response);

        protected IBrain Brain;

        public void Register(Robot robot)
        {
            Brain = robot.Brain;

            robot.Respond(command, msg => Execute(msg.Match, new MmbotResponseAdapter(msg)));
        }

        public void Register(IBrain brain)
        {
            Brain = brain;
        }

        public IEnumerable<string> GetHelp()
        {
            return new[] { helpText };
        }

        readonly string command;
        readonly string helpText;

        public interface IResponse
        {
            User User { get; }

            Task Send(params string[] message);
        }
    }

    public class MmbotResponseAdapter : BotCommand.IResponse
    {
        readonly IResponse<TextMessage> msg;

        public MmbotResponseAdapter(IResponse<TextMessage> msg)
        {
            this.msg = msg;
        }

        public User User { get { return msg.Message.User; } }

        public Task Send(params string[] message)
        {
            return msg.Send(message);
        }
    }
}