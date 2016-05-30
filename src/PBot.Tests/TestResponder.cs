namespace PBot.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MMBot;
    using PBot;
    using Users;

    public class TestResponder : BotCommand.IResponse
    {
        public TestResponder()
        {
            Credentials = new UserCredentials
            {
                Username = "testuser"
            };
        }
        public List<string> Messages = new List<string>();

        public UserCredentials Credentials { get; set; }

        public Task Send(params string[] messages)
        {
            Messages.AddRange(messages);
            return Task.FromResult(0);
        }

        public void AsUserName(string userName)
        {
            User = new User("x", userName, null, "myRoom", "test");
        }

        public User User { get; private set; }
    }
}