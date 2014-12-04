namespace PBot.Issues
{
    using System.Threading.Tasks;
    using Octokit;

    public class ConvertIssueToPullRequest : BotCommand
    {
        public ConvertIssueToPullRequest()
            : base("(convert issue) (.*) (from repository) (.*) (into pull request from) (.*) to (.*)$",
            "pbot convert issue <issue number> from repository <name of repo> into pull request from <PR branch> to <target branch> - Converts an issue into a pull request")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var client = GitHubClientBuilder.Build();
            client.Credentials = GitHubHelper.Credentials;
            var apiConnection = new ApiConnection(client.Connection);

            int issueNumber;
            if (!int.TryParse(parameters[1], out issueNumber))
            {
                await response.Send("Issue number should be a valid number dude!");
                return;
            }
            try
            {
                var result = await apiConnection.Post<PullRequest>(ApiUrls.PullRequests("Particular", parameters[3]), new ConvertedPullRequest
                {
                    Issue = issueNumber.ToString(),
                    Head = parameters[5],
                    Base = parameters[7]
                });
                await response.Send(string.Format("Issue {0} has been converted into a pull request {1}.", issueNumber, result.HtmlUrl));
            }
            catch (NotFoundException)
            {
                response.Send("Sorry, GitHub could not find the issue or repository you are talking about.").ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (ApiValidationException)
            {
                response.Send("Sorry, your request was rejected by GitHub as invalid.").ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public class ConvertedPullRequest
        {
            public string Issue { get; set; }
            public string Head { get; set; }
            public string Base { get; set; }
        }
    }
}