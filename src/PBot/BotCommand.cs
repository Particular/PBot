namespace PBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MMBot;
    using MMBot.Brains;
    using MMBot.Scripts;
    using Users;

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
                    Execute(msg.Match.Select(CleanupLinkFormattedMatches).ToArray(), adapter)
                         .Wait();
                }
                catch (Exception ex)
                {
                    adapter.Send(ex.ToString());
                    throw;
                }
            });
        }

        public static string CleanupLinkFormattedMatches(string match)
        {
            if (match.StartsWith("<") && match.EndsWith(">") && match.Contains("|"))
            {
                return match.Substring(1, match.Length - 2)
                    .Split('|').Last();
            }

            return match;
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

            var existingCredential = userCredentials.Credentials.SingleOrDefault(c => c.Name == credential);

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

        public User User => msg.Message.User;


        public Task Send(params string[] message)
        {
            return msg.Send(message);
        }

        readonly IResponse<TextMessage> msg;

    }
}