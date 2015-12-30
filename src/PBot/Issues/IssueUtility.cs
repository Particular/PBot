namespace PBot.Issues
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public static class IssueUtility
    {
        public static async Task<Issue> Transfer(
            string sourceRepoOwner,
            string sourceRepoName,
            int sourceIssueNumber,
            string targetRepoOwner,
            string targetRepoName,
            bool closeOriginal)
        {
            var client = GitHubClientBuilder.Build();

            var issue = client.Issue;
            var sourceIssue = await issue.Get(sourceRepoOwner, sourceRepoName, sourceIssueNumber);
            var sourceComments = await issue.Comment.GetForIssue(sourceRepoOwner, sourceRepoName, sourceIssueNumber);
            var sourceLabels = await client.Issue.Labels.GetForIssue(sourceRepoOwner, sourceRepoName, sourceIssueNumber);

            var newBody = string.Format(
                @"**Issue by [{1}]({0})** _{2}_ _Originally opened as {3}_

----

{4}", sourceIssue.User.HtmlUrl, sourceIssue.User.Login, sourceIssue.CreatedAt, sourceIssue.HtmlUrl, sourceIssue.Body);

            var createIssue = new NewIssue(sourceIssue.Title)
            {
                Assignee = sourceIssue.Assignee?.Login,
                Body = newBody
            };
            var targetIssue = await issue.Create(targetRepoOwner, targetRepoName, createIssue);

            var comment = sourceComments.FirstOrDefault();
            if (comment != null)
            {
                var body = string.Format(
                    @" **Comment by [{1}]({0})** _{2}_

----

{3}", comment.User.HtmlUrl, comment.User.Login, comment.HtmlUrl, comment.Body);
                await issue.Comment.Create(targetRepoOwner, targetRepoName, targetIssue.Number, body);
            }

            await issue.Comment.Create(sourceRepoOwner, sourceRepoName, sourceIssueNumber, (closeOriginal ? "moved to " : "copied to ") + targetIssue.HtmlUrl);

            if (sourceIssue.ClosedAt == null)
            {
                if (closeOriginal)
                {
                    var issueUpdate = new IssueUpdate
                        {
                            State = ItemState.Closed
                        };
                    await issue.Update(sourceRepoOwner, sourceRepoName, sourceIssueNumber, issueUpdate);
                }
            }
            else
            {
                var issueUpdate = new IssueUpdate
                {
                    State = ItemState.Closed
                };
                await issue.Update(targetRepoOwner, targetRepoName, targetIssue.Number, issueUpdate);
            }

            if (sourceLabels.Any())
            {
                var targetLabels = await client.Issue.Labels.GetForRepository(targetRepoOwner, targetRepoName);
                foreach (var sourceLabel in sourceLabels)
                {
                    if (!targetLabels.Any(targetLabel => targetLabel.Name == sourceLabel.Name))
                    {
                        await client.Issue.Labels.Create(
                            targetRepoOwner,
                            targetRepoName,
                            new NewLabel(sourceLabel.Name, sourceLabel.Color));
                    }
                }

                await client.Issue.Labels.AddToIssue(
                    targetRepoOwner,
                    targetRepoName,
                    targetIssue.Number,
                    sourceLabels.Select(x => x.Name).ToArray());
            }

            return await client.Issue.Get(targetRepoOwner, targetRepoName, targetIssue.Number);
        }
    }
}
