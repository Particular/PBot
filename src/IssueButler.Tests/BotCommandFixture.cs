﻿namespace IssueButler.Tests
{
    using System;
    using IssueButler.Mmbot;
    using MMBot;
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
        }

    }
}