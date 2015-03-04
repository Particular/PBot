namespace PBot.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class RepoParserTests 
    {
        
        [Test]
        public void ParseSlackFormattedIssueLink()
        {
            const string input = "<http://docs.particular.net#578|docs.particular.net#578>";
            string repo;
            string issue;
            Assert.True(RepoParser.ParseRepoAndIssueNumber(input, out repo, out issue));
            Assert.AreEqual("docs.particular.net", repo);
            Assert.AreEqual("578", issue);

        }
        
        [Test]
        public void ParseSlackFormattedNoIssueLink()
        {
            const string input = "<http://docs.particular.net|docs.particular.net>";
            string repo;
            Assert.True(RepoParser.ParseRepo(input, out repo));
            Assert.AreEqual("docs.particular.net", repo);
        }

        [Test]
        public void ParseManualWithIssue()
        {
            const string input = "docs.particular.net#578";
            string repo;
            string issue;
            Assert.True(RepoParser.ParseRepoAndIssueNumber(input, out repo, out issue));
            Assert.AreEqual("docs.particular.net", repo);
            Assert.AreEqual("578", issue);

        }

        [Test]
        public void ParseManualWithIncorrectIssueSyntaxSpace()
        {
            const string input = "docs.particular.net #578";
            string repo;
            string issue;
            Assert.False(RepoParser.ParseRepoAndIssueNumber(input, out repo, out issue));
        }


        [Test]
        public void ParseManualWithIncorrectIssueSyntaxNoHash()
        {
            const string input = "docs.particular.net 578";
            string repo;
            string issue;
            Assert.False(RepoParser.ParseRepoAndIssueNumber(input, out repo, out issue));
        }
        
        [Test]
        public void ParseManualWithNoIssue()
        {
            const string input = "docs.particular.net";
            string repo;
            Assert.True(RepoParser.ParseRepo(input, out repo));
            Assert.AreEqual("docs.particular.net", repo);
        }

        [Test]
        public void ParseManualWithEmptyInput()
        {
            const string input = "";
            string repo;
            Assert.False(RepoParser.ParseRepo(input, out repo));
        }

        [Test]
        public void ParseUrlWithIssue()
        {
            const string input = "https://github.com/Particular/docs.particular.net/issues/578";
            string repo;
            string issue;
            Assert.True(RepoParser.ParseRepoAndIssueNumber(input, out repo, out issue));
            Assert.AreEqual("docs.particular.net", repo);
            Assert.AreEqual("578", issue);
        }


        [Test]
        public void ParseUrlWithoutIssue()
        {
            const string input = "https://github.com/Particular/docs.particular.net";
            string repo;
            Assert.True(RepoParser.ParseRepo(input, out repo));
            Assert.AreEqual("docs.particular.net", repo);
        }

        [Test]
        public void ParseUrlWithoutIssueTrailingSlash()
        {
            const string input = "https://github.com/Particular/docs.particular.net/";
            string repo;
            Assert.True(RepoParser.ParseRepo(input, out repo));
            Assert.AreEqual("docs.particular.net", repo);
        }

        [Test]
        public void ParseUrlWithDifferentCasing()
        {
            const string input = "https://GITHUB.com/parTICUlar/docs.particular.net/";
            string repo;
            Assert.True(RepoParser.ParseRepo(input, out repo));
            Assert.AreEqual("docs.particular.net", repo);
        }

        [Test]
        public void ParseUrlWithIssueAndDifferentCasing()
        {
            const string input = "hTtPs://GiThUb.CoM/PaRtIcUlAr/docs.particular.net/iSsUeS/578";
            string repo;
            string issue;
            Assert.True(RepoParser.ParseRepoAndIssueNumber(input, out repo, out issue));
            Assert.AreEqual("docs.particular.net", repo);
            Assert.AreEqual("578", issue);
        }

        [Test]
        public void ParseUrlWithIssueSurroundedByTags()
        {
            const string input = "<https://github.com/Particular/docs.particular.net/issues/578>";
            string repo;
            string issue;
            Assert.True(RepoParser.ParseRepoAndIssueNumber(input, out repo, out issue));
            Assert.AreEqual("docs.particular.net", repo);
            Assert.AreEqual("578", issue);
        }

        [Test]
        public void ParseUrlWithoutIssueSurroundedByTags()
        {
            const string input = "<https://github.com/Particular/docs.particular.net>";
            string repo;
            
            Assert.True(RepoParser.ParseRepo(input, out repo));
            Assert.AreEqual("docs.particular.net", repo);
        }

        [Test]
        public void ParseManualWithIncorrectIssueSyntaxHashInWrongPlace()
        {
            const string input = "docs.particular.net578#";
            string repo;
            string issue;
            Assert.False(RepoParser.ParseRepoAndIssueNumber(input, out repo, out issue)); 
        }
    }
}