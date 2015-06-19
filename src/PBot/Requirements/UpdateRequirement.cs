using System;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using PBot;
using PBot.Requirements;

public class UpdateRequirement : BotCommand
{
    string owner;
    string repoName;

    public UpdateRequirement()
        : this("Particular", "Requirements")
    {

    }

    public UpdateRequirement(string owner, string repoName)
        : base("update requirement (.*)$",
            "pbot update requirement <Number> - Updates an existing requirement issue with the specified number")
    {
        this.owner = owner;
        this.repoName = repoName;
    }

    public override async Task Execute(string[] parameters, IResponse response)
    {
        if (parameters.Length < 2)
        {
            await response.Send(string.Format("I don't understand '{0}'.", string.Join(" ", parameters.Select(x => "[" + x + "]"))));
            return;
        }
        var issueNumber = Convert.ToInt32(parameters[1]);

        string accessToken;

        if (!TryGetCredential("github-accesstoken", out accessToken))
        {
            await response.Send("I couldn\'t find a github access token in your credentials. You need to add one so that I can create the requirement on your behalf. Does it sound good? Here are the instructions https://github.com/Particular/Housekeeping/wiki/Generate-GitHub-access-token-for-PBot");

            return;
        }

        var client = GitHubClientBuilder.Build(accessToken);

        var issue = await client.Issue.Get(owner, repoName, issueNumber);

        // Let's be super dump for now
        var newBody = string.Format(bodyTemplate, CreateRequirement.BodyTemplate, issue.Body);

        issue = await client.Issue.Update(owner, repoName, issueNumber, new IssueUpdate
        {
            Body = newBody
        });

        await response.Send("Requirement updated " + issue.HtmlUrl);
    }

    const string bodyTemplate = @"{0}

# Previous Content
{1}";
}