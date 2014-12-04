namespace PBot.Issues
{
    using System.Threading.Tasks;
    using Octokit;

    public class ConvertIssueToPullRequest : BotCommand
    {
        public ConvertIssueToPullRequest()
            : base("convert (.*)#(.*) (to pull from) (.*) to (.*)$",
            "pbot convert <repository>#<issue number> to pull from <PR branch> to <target branch> - Converts an issue into a pull request")
        {
        }

        public override async Task Execute(string[] parameters, IResponse response)
        {
            var client = GitHubClientBuilder.Build();
            client.Credentials = GitHubHelper.Credentials;
            var apiConnection = new ApiConnection(client.Connection);

            int issueNumber;
            if (!int.TryParse(parameters[2], out issueNumber))
            {
                await response.Send("Issue number should be a valid number dude!");
                return;
            }
            try
            {
                var result = await apiConnection.Post<PullRequest>(ApiUrls.PullRequests("Particular", parameters[1]), new ConvertedPullRequest
                {
                    Issue = issueNumber.ToString(),
                    Head = parameters[4],
                    Base = parameters[6]
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