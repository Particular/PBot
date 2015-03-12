namespace PBot.Requirements
{
    using Octokit;

    public class Requirements
    {
        public static NewIssue NewConcern(string title, string body = null,IssueState state = null)
        {
            var issue = new NewIssue(title);

            if (!string.IsNullOrEmpty(body))
            {
                issue.Body = body;
            }

            if (state != null)
            {
                issue.Labels.Add(state);
            }

            issue.Labels.Add(RequirementTypes.Concern);

            return issue;
        }

        public static NewIssue NewFeature(string title, string body = null, IssueState state = null)
        {
            var issue = new NewIssue(title);

            if (!string.IsNullOrEmpty(body))
            {
                issue.Body = body;
            }

            if (state != null)
            {
                issue.Labels.Add(state);
            }

            return issue;
        }
    }
}