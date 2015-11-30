namespace PBot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MMBot;
    using MMBot.Brains;
    using NUnit.Framework;
    using PBot;
    using PBot.Users;

    public class BotCommandFixture<TCommand> where TCommand : BotCommand
    {
        [SetUp]
        public void SetUp()
        {
            brain = new TestBrain();
            brain.Set(new CredentialStore());

            command = Activator.CreateInstance<TCommand>();

            testResponder = new TestResponder();

            user = "testuser";
        }

        protected IBrain brain;
        TCommand command;
        TestResponder testResponder;
        string user;
        
        protected async Task Execute(params string[] parameters)
        {
            command.Register(brain);
            command.CurrentUser = new User("x", user, null, "myRoom", "test");

            await command.Execute(parameters, testResponder)
                .ConfigureAwait(false);
            
            if (Messages.Any())
            {
                Console.Out.WriteLine(string.Join(Environment.NewLine, Messages));             
            }
        }

        protected void AsUser(string userName)
        {
            user = userName;
            testResponder.AsUserName(userName);
        }


        protected void WithCredentials(UserCredentials credentials)
        {
            testResponder.Credentials = credentials;
            brain.Get<CredentialStore>().Add(credentials);
        }

        public IEnumerable<string> Messages => testResponder.Messages;
    }
}