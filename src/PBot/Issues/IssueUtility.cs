namespace PBot.Issues
{
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
            bool closeSourceIssue)
        {
            var client = GitHubClientBuilder.Build();
            var issuesClient = client.Issue;

            var sourceIssue = await issuesClient.Get(sourceRepoOwner, sourceRepoName, sourceIssueNumber);
            var sourceComments = await issuesClient.Comment.GetForIssue(sourceRepoOwner, sourceRepoName, sourceIssueNumber);
            var sourceLabels = await client.Issue.Labels.GetForIssue(sourceRepoOwner, sourceRepoName, sourceIssueNumber);

            var targetBody =
$@"**Issue by [{sourceIssue.User.Login}]({sourceIssue.User.HtmlUrl})** _{sourceIssue.CreatedAt}_ _Originally opened as {sourceIssue.HtmlUrl}_

----

{sourceIssue.Body}";

            var targetIssue = await issuesClient.Create(
                targetRepoOwner,
                targetRepoName,
                new NewIssue(sourceIssue.Title) { Assignee = sourceIssue.Assignee?.Login, Body = targetBody, });

            var sourceComment = sourceComments.FirstOrDefault();
            if (sourceComment != null)
            {
                var targetCommentBody =
$@" **Comment by [{sourceComment.User.Login}]({sourceComment.User.HtmlUrl})** _{sourceComment.HtmlUrl}_

----

{sourceComment.Body}";

                await issuesClient.Comment.Create(targetRepoOwner, targetRepoName, targetIssue.Number, targetCommentBody);
            }

            await issuesClient.Comment.Create(
                sourceRepoOwner,
                sourceRepoName,
                sourceIssueNumber,
                (closeSourceIssue ? "moved to " : "copied to ") + targetIssue.HtmlUrl);

            if (sourceIssue.ClosedAt == null)
            {
                if (closeSourceIssue)
                {
                    await issuesClient.Update(
                        sourceRepoOwner, sourceRepoName, sourceIssueNumber, new IssueUpdate { State = ItemState.Closed });
                }
            }
            else
            {
                await issuesClient.Update(
                    targetRepoOwner, targetRepoName, targetIssue.Number, new IssueUpdate { State = ItemState.Closed, });
            }

            if (sourceLabels.Any())
            {
                var targetRepoLabels = await client.Issue.Labels.GetForRepository(targetRepoOwner, targetRepoName);
                foreach (var sourceLabel in sourceLabels)
                {
                    if (!targetRepoLabels.Any(targetRepoLabel => targetRepoLabel.Name == sourceLabel.Name))
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
