namespace PBot.Tests
{
    using NUnit.Framework;

    class BotCommandTests
    {
        [Test]
        public void Should_remove_slack_auto_formatted_urls()
        {
            Assert.AreEqual("docs.particular.net", BotCommand.CleanupLinkFormattedMatches("<http://docs.particular.net|docs.particular.net>"));
            Assert.AreEqual("<abc>", BotCommand.CleanupLinkFormattedMatches("<abc>"));
        }

      
    }
}