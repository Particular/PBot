namespace PBot.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using PBot.Users;

    [TestFixture]
    public class RegisterCredentialTests : BotCommandFixture<RegisterCredential>
    {
        [Test]
        public void AddForNewUser()
        {
            AsUser("testuser");
            Execute("","credential=value");

            var credential = brain.Get<CredentialStore>().Single(c=>c.Username == "testuser").Credentials.First();
            Assert.AreEqual("credential",credential.Name);
            Assert.AreEqual("value", credential.Value);
        }

        [Test]
        public void UpdateCredential()
        {
            AsUser("testuser");
            Execute("", "credential=value");
            Execute("", "credential=value2");

            var credential = brain.Get<CredentialStore>().Single(c=>c.Username == "testuser").Credentials.First();
            Assert.AreEqual("credential", credential.Name);
            Assert.AreEqual("value2", credential.Value);
        }
    }
}