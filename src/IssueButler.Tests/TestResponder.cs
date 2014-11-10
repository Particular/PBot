namespace IssueButler.Tests
{
    using System.Collections.Generic;
    using IssueButler.Mmbot;

    public class TestResponder:BotCommand.IResponse
    {
        public void Send(string message)
        {
            Messages.Add(message);
        }

        public List<string> Messages = new List<string>(); 
    }
}