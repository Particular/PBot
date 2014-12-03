namespace PBot.Tests
{
    using System;
    using System.Collections.Generic;
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
        }

        protected IBrain brain;
        TCommand command;
        TestResponder testResponder;
        
        protected void Execute(params string[] parameters)
        {
            command.Register(brain);
            command.Execute(parameters, testResponder)
                .Wait();

            Console.Out.WriteLine(string.Join(Environment.NewLine, Messages));
        }

        protected void AsUser(string userName)
        {
            testResponder.AsUserName(userName);
        }


        protected void WithCredentials(UserCredentials credentials)
        {
            testResponder.Credentials = credentials;

        }

        public IEnumerable<string> Messages
        {
            get { return testResponder.Messages; }
        }
    }
}