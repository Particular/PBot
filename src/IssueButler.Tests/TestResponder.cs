namespace IssueButler.Tests
{
    using System.Collections.Generic;
    using IssueButler.Mmbot;

    public class TestResponder:BotCommand.IResponse
    {
        public List<string> Messages = new List<string>();
        public void Send(params string[] messages)
        {
            Messages.AddRange(messages);
        }
    }
}