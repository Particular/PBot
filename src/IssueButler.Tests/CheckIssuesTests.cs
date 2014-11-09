namespace IssueButler.Tests
{
    using System;
    using IssueButler.Mmbot;
    using IssueButler.Mmbot.Issues;
    using NUnit.Framework;

    [TestFixture]
    public class CheckIssuesTests
    {
        [Test]
        public void CheckNServiceBus()
        {
            var client = GitHubClientBuilder.Build();
            var repo = client.Repository.Get("Particular", "NServiceBus").Result;

            var errors = new CheckIssuesForRepository(repo, GitHubClientBuilder.Build())
                .Execute();

            foreach (var validationError in errors)
            {
                Console.Out.WriteLine("{0} - {1}",validationError.Issue.HtmlUrl,validationError.Reason);
            }
        }
    }
}
