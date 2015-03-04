namespace PBot
{
    using System.Text.RegularExpressions;

    public static class RepoParser
    {
        public static bool ParseRepoAndIssueNumber(string input, out string repo, out string issueNumberString)
        {
            if (ParseFromUrlWithIssue(input, out repo, out issueNumberString))
                return true;

            if (ParseSlackFormattedLinkWithIssue(input, out repo, out issueNumberString))
                return true;

            if (ParseManualRepoAndIssue(input, out repo, out issueNumberString))
                return true;

            repo = null;
            issueNumberString = null;
            return false;
        }

        

        public static bool ParseRepo(string input, out string repo)
        {
            if (ParseFromUrlWithoutIssue(input, out repo))
                return true;

            if (ParseSlackFormattedLink(input, out repo))
                return true;

            if (ParseManualRepo(input, out repo))
                return true;

            repo = null;
            return false;
        }

        private static bool ParseManualRepo(string input, out string repo)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                repo = null;
                return false;
            }

            repo = input;
            return true;
        }

        private static bool ParseManualRepoAndIssue(string input, out string repo, out string issueNumberString)
        {
            var withIssue = new Regex(@"(^[\w\.]*)#(\d+)");
            var match = withIssue.Match(input);
            if (match.Groups.Count == 3)
            {
                repo = match.Groups[1].Value;
                issueNumberString = match.Groups[2].Value;
                return true;
            }
            repo = null;
            issueNumberString = null;
            return false;
        }

        private static bool ParseSlackFormattedLinkWithIssue(string input, out string repo, out string issueNumberString)
        {
            var withIssue = new Regex(@"<(.*)\|([\w\.]*)#(\d+)>");
            var match = withIssue.Match(input);
            if (match.Groups.Count == 4)
            {
                repo = match.Groups[2].Value;
                issueNumberString = match.Groups[3].Value;
                return true;
            }
            repo = null;
            issueNumberString = null;
            return false;
        }

        private static bool ParseSlackFormattedLink(string input, out string repo)
        {
            var noIssue = new Regex(@"<(.*)\|([\w\.]*)>");
            var match = noIssue.Match(input);
            if (match.Groups.Count == 3)
            {
                repo = match.Groups[2].Value;
                return true;
            }
            repo = null;
            return false;
        }


        private static bool ParseFromUrlWithoutIssue(string input, out string repo)
        {
            var inputWithoutTags = input.Replace("<", "").Replace(">", "");
            var noIssue = new Regex(@"https://github.com/Particular/([\w\.]*)", RegexOptions.IgnoreCase);
            var match = noIssue.Match(inputWithoutTags);
            if (match.Groups.Count == 2)
            {
                repo = match.Groups[1].Value;
                return true;
            }
            repo = null;
            return false;
        }

        private static bool ParseFromUrlWithIssue(string input, out string repo, out string issueNumberString)
        {
            var inputWithoutTags = input.Replace("<", "").Replace(">", "");
            var withIssue = new Regex(@"https://github.com/Particular/([\w\.]*)/issues/(\d+)$", RegexOptions.IgnoreCase);
            var withIssueMatch = withIssue.Match(inputWithoutTags);
            if (withIssueMatch.Groups.Count == 3)
            {
                repo = withIssueMatch.Groups[1].Value;
                issueNumberString = withIssueMatch.Groups[2].Value;
                return true;
            }
            repo = null;
            issueNumberString = null;
            return false;
        }
    }
}
