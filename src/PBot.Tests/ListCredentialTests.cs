namespace PBot.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using PBot.Users;

    [TestFixture]
    public class ListCredentialTests : BotCommandFixture<ListCredentials>
    {
        [Test]
        public async System.Threading.Tasks.Task ListUserWithExistingCreds()
        {

            var store = new CredentialStore
            {
                new UserCredentials
                {
                    Username = "testuser",
                    Credentials = new List<Credential>{new Credential{ Name = "github-username", Value ="xyz"}}
                }
            };

            brain.Set(store);

            AsUser("testuser");
            await Execute("list credentials");

            Assert.True(Messages.Single(m => m.Contains("github-username")).EndsWith("xyz"));

        }

     
    }
}