﻿namespace IssueButler.Tests
{
    using IssueButler.Mmbot;
    using IssueButler.Mmbot.Issues;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;

    [TestFixture]
    public class CheckIssuesInMyReposTests : BotCommandFixture<CheckIssuesInMyRepos>
    {
        [Test]
        public void CheckIssuesInNServiceBus()
        {
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = "NServiceBus.RabbitMq",
                    Caretaker = "andreas"
                },

                  new AvailableRepositories.Repository
                {
                    Name = "NServiceBus.SqlServer",
                    Caretaker = "john"
                }
            };

            brain.Set(repos);

            Execute("check my repos");
        }
    }
}