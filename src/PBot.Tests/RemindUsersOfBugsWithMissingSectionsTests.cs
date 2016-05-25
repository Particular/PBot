namespace PBot.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IssueButler.Mmbot.Repositories;
    using NUnit.Framework;
    using Reminders;
    using Users;

    [TestFixture]
    public class RemindUsersOfBugsWithMissingSectionsTests : BotCommandFixture<RemindUsersOfBugsWithMissingSections>
    {
        [Test]
        public async Task CheckWithNonMappedUser()
        {
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = "PBot.TestRepo"
                }
            };

            brain.Set(repos);
            await Execute("remind users of mandatory bug sections");


            Assert.True(Messages.First().Contains("https://github.com/Particular/PBot.TestRepo/issues/15"));
            Assert.True(Messages.First().Contains("andreasohlund"));
            Assert.True(Messages.First().Contains("@channel"));
        }

        [Test]
        public Task CheckRaven()
        {
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = "NServiceBus.RavenDB"
                }
            };

            brain.Set(repos);
            return Execute("remind users of mandatory bug sections");
        }


        [Test]
        public async Task CheckWithMappedUser()
        {
            var repos = new AvailableRepositories
            {
                new AvailableRepositories.Repository
                {
                    Name = "PBot.TestRepo"
                }
            };

            brain.Set(repos);


            var store = new CredentialStore
            {
                new UserCredentials
                {
                    Username = "andreas",
                    Credentials = new List<Credential>{new Credential{ Name = "github-username", Value ="andreasohlund"}}
                }
            };


            brain.Set(store);

            await Execute("remind users of mandatory bug sections");

            var message = Messages.Single(m => m.Contains("https://github.com/Particular/PBot.TestRepo/issues/15"));

            Assert.True(message.Contains("@andreas!"));

        }

    }
}