namespace PBot.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Issues;
    using Users;

    [TestFixture]
    public class ConvertIssueToPullRequestTests : BotCommandFixture<ConvertIssueToPullRequest>
    {
        [Test]
        [Explicit]
        public Task CanConvertAnExistingIssue()
        {
            var token = Environment.GetEnvironmentVariable("PBOT_GH_ACCESSTOKEN");

            Assert.NotNull(token);

            var credentials = new UserCredentials{Username = "testuser"};

            credentials.AddCredential("github-accesstoken",token);

            WithCredentials(credentials);

            return Execute("convert", "PBot.TestRepo", "19", "failed-pull-1", "master");
        }

        [Test]
        public async Task ShouldAskForGHTokenIfNotPresent()
        {
            await Execute("convert", "PBot.TestRepo", "#", "19", "to pull from", "failed-pull-1", "to", "master");


            Assert.True(Messages.First().Contains("github access token"));
        }
    }
}