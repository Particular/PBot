namespace PBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MMBot;
    using MMBot.Brains;
    using MMBot.Scripts;
    using PBot.Users;

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

            robot.Respond(command, msg =>
            {
                var adapter = new MmbotResponseAdapter(msg);
                try
                {
                    CurrentUser = msg.Message.User;
                    Execute(msg.Match, adapter)
                         .Wait();
                }
                catch (Exception ex)
                {
                    adapter.Send(ex.ToString());
                    throw;
                }
            });
        }

        public void Register(IBrain brain)
        {
            Brain = brain;
        }

        public User CurrentUser { get; set; }

        public bool TryGetCredential(string credential, out string value)
        {
            value = null;

            if (CurrentUser == null)
            {
                return false;   
            }

            var store = Brain.Get<CredentialStore>();

            if (store == null)
            {
                return false;
            }

            UserCredentials userCredentials;

            if (!store.TryGetValue(CurrentUser.Name, out userCredentials))
            {
                return false;         
            }

            var existingCredential = userCredentials.Credentials.SingleOrDefault(c=>c.Name == credential);

            if (existingCredential == null)
            {
                return false;
            }
            value = existingCredential.Value;

            return true;
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

        public MmbotResponseAdapter(IResponse<TextMessage> msg)
        {
            this.msg = msg;
        }

        public User User { get { return msg.Message.User; } }


        public Task Send(params string[] message)
        {
            return msg.Send(message);
        }

        readonly IResponse<TextMessage> msg;

    }
}