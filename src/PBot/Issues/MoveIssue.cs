namespace PBot.Issues
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Octokit;

    public class MoveIssue : BotCommand
    {
        public MoveIssue()
            : base("move issue (.*)#(.*) to (.*)$",
            "pbot move issue <repository>#<issue number> to <target repository> - Moves an issue from one repository to the other.")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            if (parameters.Length < 4)
            {
                await response.Send(string.Format("I don't understand '{0}'.", string.Join(" ", parameters.Select(x => "[" + x + "]"))));
                return;
            }

            var repo = parameters[1];
            var issueNumberString = parameters[2];
            var targetRepo = parameters[3];

            int issueNumber;
            if (!int.TryParse(issueNumberString, out issueNumber))
            {
                await response.Send("Issue number should be a valid number dude!");
                return;
            }

            var src = new RepoInfo { Owner = "Particular", Name = repo };
            var dst = new RepoInfo { Owner = "Particular", Name = targetRepo };

            await response.Send(string.Format("Moving issue https://github.com/Particular/{0}/issues/{1}", repo, issueNumber)).IgnoreWaitContext();

            var newIssue = await Transfer(src, issueNumber, dst).IgnoreWaitContext();

            await response.Send(string.Format("Issue moved to https://github.com/Particular/{0}/issues/{1}", targetRepo, newIssue.Number)).IgnoreWaitContext();
        }

        private async Task<Issue> Transfer(RepoInfo sourceRepository, int sourceIssueNumber, RepoInfo targetRepository)
        {
            var client = GitHubClientBuilder.Build();

            var sourceIssue = await client.Issue.Get(sourceRepository.Owner, sourceRepository.Name, sourceIssueNumber);
            var sourceComments = await client.Issue.Comment.GetForIssue(sourceRepository.Owner, sourceRepository.Name, sourceIssueNumber);

            var newBody = string.Format(
                @"<a href=""{0}""><img src=""{1}"" align=""left"" width=""96"" height=""96"" hspace=""10""></img></a> **Issue by [{2}]({0})**
_{3}_

_Originally opened as {4}_

----

{5}", sourceIssue.User.HtmlUrl, sourceIssue.User.AvatarUrl, sourceIssue.User.Login, sourceIssue.CreatedAt, sourceIssue.HtmlUrl, sourceIssue.Body);

            var createIssue = new NewIssue(sourceIssue.Title)
            {
                Assignee = sourceIssue.GetAssignee(),
                Body = newBody
            };
            var targetIssue = await client.Issue.Create(targetRepository.Owner, targetRepository.Name, createIssue);

            foreach (var sourceComment in sourceComments)
            {
                var targetCommentBody = string.Format(
                    @"<a href=""{0}""><img src=""{1}"" align=""left"" width=""96"" height=""96"" hspace=""10""></img></a> **Comment by [{2}]({0})**
<br />
_{3}_

----

{4}", sourceComment.User.HtmlUrl, sourceComment.User.AvatarUrl, sourceComment.User.Login, sourceComment.HtmlUrl, sourceComment.Body);
                await client.Issue.Comment.Create(targetRepository.Owner, targetRepository.Name, targetIssue.Number, targetCommentBody);
            }

            await client.Issue.Comment.Create(sourceRepository.Owner, sourceRepository.Name, sourceIssueNumber, "moved to " + targetIssue.HtmlUrl);

            if (sourceIssue.ClosedAt == null)
            {
                var issueUpdate = new IssueUpdate
                {
                    State = ItemState.Closed
                };
                await client.Issue.Update(sourceRepository.Owner, sourceRepository.Name, sourceIssueNumber, issueUpdate);
            }
            else
            {
                var issueUpdate = new IssueUpdate
                {
                    State = ItemState.Closed
                };
                await client.Issue.Update(targetRepository.Owner, targetRepository.Name, targetIssue.Number, issueUpdate);
            }
            return await client.Issue.Get(targetRepository.Owner, targetRepository.Name, targetIssue.Number);
        }

        private class RepoInfo
        {
            public string Owner;
            public string Name;
        }
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
            if (issue.Assignee != null)
            {
                return issue.Assignee.Login;
            }
            return null;
        }

        internal static int? GetMilestone(this Issue issue)
        {
            if (issue.Milestone != null)
            {
                return issue.Milestone.Number;
            }
            return null;
        }
    }
}