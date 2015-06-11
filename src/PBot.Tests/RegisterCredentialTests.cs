namespace PBot.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using PBot.Users;

    [TestFixture]
    public class RegisterCredentialTests : BotCommandFixture<RegisterCredential>
    {
        [Test]
        public async void AddForNewUser()
        {
            AsUser("testuser");
            await Execute("", "credential=value");

            var credential = brain.Get<CredentialStore>().Single(c=>c.Username == "testuser").Credentials.First();
            Assert.AreEqual("credential",credential.Name);
            Assert.AreEqual("value", credential.Value);
        }

        [Test]
        public async void UpdateCredential()
        {
            AsUser("testuser");
            await Execute("", "credential=value");
            await Execute("", "credential=value2");

            var credential = brain.Get<CredentialStore>().Single(c=>c.Username == "testuser").Credentials.First();
            Assert.AreEqual("credential", credential.Name);
            Assert.AreEqual("value2", credential.Value);
        }
    }
}