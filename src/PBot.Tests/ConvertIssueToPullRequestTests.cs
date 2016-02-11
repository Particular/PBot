namespace PBot.Tests
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using PBot.Issues;
    using PBot.Users;

    [TestFixture]
    public class ConvertIssueToPullRequestTests : BotCommandFixture<ConvertIssueToPullRequest>
    {
        [Test]
        [Explicit]
        public async System.Threading.Tasks.Task CanConvertAnExistingIssue()
        {
            var token = Environment.GetEnvironmentVariable("PBOT_GH_ACCESSTOKEN");
            
            Assert.NotNull(token);

            var credentials = new UserCredentials{Username = "testuser"};

            credentials.AddCredential("github-accesstoken",token);

            WithCredentials(credentials);

            await Execute("convert", "PBot.TestRepo", "19", "failed-pull-1", "master");
        }

        [Test]
        public async System.Threading.Tasks.Task ShouldAskForGHTokenIfNotPresent()
        {
            await Execute("convert", "PBot.TestRepo", "#", "19", "to pull from", "failed-pull-1", "to", "master");


            Assert.True(Messages.First().Contains("github access token"));
        }
    }
}