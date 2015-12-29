namespace PBot.Issues
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public static class IssueUtility
    {
        public static async Task<Issue> Transfer(RepoInfo sourceRepository, int sourceIssueNumber, RepoInfo targetRepository, bool closeOriginal)
        {
            var client = GitHubClientBuilder.Build();

            var issue = client.Issue;
            var sourceIssue = await issue.Get(sourceRepository.Owner, sourceRepository.Name, sourceIssueNumber);
            var sourceComments = await issue.Comment.GetForIssue(sourceRepository.Owner, sourceRepository.Name, sourceIssueNumber);
            var sourceLabels = await client.Issue.Labels.GetForIssue(sourceRepository.Owner, sourceRepository.Name, sourceIssueNumber);

            var newBody = string.Format(
                @"**Issue by [{1}]({0})** _{2}_ _Originally opened as {3}_

----

{4}", sourceIssue.User.HtmlUrl, sourceIssue.User.Login, sourceIssue.CreatedAt, sourceIssue.HtmlUrl, sourceIssue.Body);

            var createIssue = new NewIssue(sourceIssue.Title)
            {
                Assignee = sourceIssue.GetAssignee(),
                Body = newBody
            };
            var targetIssue = await issue.Create(targetRepository.Owner, targetRepository.Name, createIssue);

            var comment = sourceComments.FirstOrDefault();
            if (comment != null)
            {
                var body = string.Format(
                    @" **Comment by [{1}]({0})** _{2}_

----

{3}", comment.User.HtmlUrl, comment.User.Login, comment.HtmlUrl, comment.Body);
                await issue.Comment.Create(targetRepository.Owner, targetRepository.Name, targetIssue.Number, body);
            }

            await issue.Comment.Create(sourceRepository.Owner, sourceRepository.Name, sourceIssueNumber, (closeOriginal ? "moved to " : "copied to ") + targetIssue.HtmlUrl);

            if (sourceIssue.ClosedAt == null)
            {
                if (closeOriginal)
                {
                    var issueUpdate = new IssueUpdate
                        {
                            State = ItemState.Closed
                        };
                    await issue.Update(sourceRepository.Owner, sourceRepository.Name, sourceIssueNumber, issueUpdate);
                }
            }
            else
            {
                var issueUpdate = new IssueUpdate
                {
                    State = ItemState.Closed
                };
                await issue.Update(targetRepository.Owner, targetRepository.Name, targetIssue.Number, issueUpdate);
            }

            if (sourceLabels.Any())
            {
                var targetLabels = await client.Issue.Labels.GetForRepository(targetRepository.Owner, targetRepository.Name);
                foreach (var sourceLabel in sourceLabels)
                {
                    if (!targetLabels.Any(targetLabel => targetLabel.Name == sourceLabel.Name))
                    {
                        await client.Issue.Labels.Create(
                            targetRepository.Owner,
                            targetRepository.Name,
                            new NewLabel(sourceLabel.Name, sourceLabel.Color));
                    }
                }

                await client.Issue.Labels.AddToIssue(
                    targetRepository.Owner,
                    targetRepository.Name,
                    targetIssue.Number,
                    sourceLabels.Select(x => x.Name).ToArray());
            }

            return await client.Issue.Get(targetRepository.Owner, targetRepository.Name, targetIssue.Number);
        }
    }

    public class RepoInfo
    {
        public string Owner;
        public string Name;
    }

    public static class Extensions
    {
        public static string ReadToEnd(this Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        internal static string GetAssignee(this Issue issue)
        {
            return issue.Assignee?.Login;
        }

        internal static int? GetMilestone(this Issue issue)
        {
            return issue.Milestone?.Number;
        }
    }
}