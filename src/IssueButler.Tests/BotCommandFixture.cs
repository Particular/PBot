namespace IssueButler.Tests
{
    using System;
    using System.Collections.Generic;
    using IssueButler.Mmbot;
    using MMBot.Brains;
    using NUnit.Framework;

    public class BotCommandFixture<TCommand> where TCommand : BotCommand
    {
        [SetUp]
        public void SetUp()
        {
            brain = new TestBrain();

            command = Activator.CreateInstance<TCommand>();

            testResponder = new TestResponder();
        }

        protected IBrain brain;
        TCommand command;
        TestResponder testResponder;

        protected void Execute(params string[] parameters)
        {
            command.Register(brain);
            command.Execute(parameters, testResponder);

            Console.Out.WriteLine(string.Join(Environment.NewLine,Messages));
        }

        public IEnumerable<string> Messages
        {
            get { return testResponder.Messages; }}
    }
}